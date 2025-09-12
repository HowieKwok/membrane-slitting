using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Interop;

using AntdUI;

using Eholly.Lib.Controls;

using EHolly.FormControl;

using Extensions;

using Frame_Core;

using HalconDotNet;

using HslCommunication.CNC.Fanuc;

using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

using LogTool;

using ScottPlot;

namespace Mef_VisionFrame
{
    public partial class BIZ_DataWorker_WidthLaser : IBizWorkerBase
    {

        #region 方法

        /// <summary>
        /// 根据检测结果生成 DB 类型数组并异步发送给 dbjCardManager。
        /// </summary>
        /// <param name="keyA">板卡 Key，0 表示跳过。</param>
        /// <param name="resultSliceAllResult">全通道检测结果。</param>
        /// <param name="zones">通道分区信息。</param>
        /// <param name="CamID">相机 ID。</param>
        private void SendDBInfoIfNeeded(int keyA, int FrameID,
                                               List<ResultType> resultSliceAllResult,
                                            int CamID = 6)
        {

            HOperatorSet.CountSeconds(out HTuple StartSendDB);
            var zones = ((DBJCardParam)Project.Instance.dbjCardManager.tcpClientParam)?.Zones;


            if (!Project.Instance.dbjCardManager.initState || keyA <= 0)
            {
                // 第一个 EA 不发送
                const string msg = "开始段第一个EA默认不统计不达标";
                if (keyA <= 0)
                {
                    MessageNotify.AddCommLog(msg);
                    MessageNotify.AddDebugLog(msg);
                }
                else
                {
                    MessageNotify.AddErrorLog($"打标机未连接（{Project.Instance.dbjCardManager.initState}）,暂不发送打标，当前EA为{keyA}");
                }

                return;
            }

            // 1. 默认全部 0000
            var dbTypes = Enumerable.Range(0, 12)
                                    .Select(i => (i, "0000"))
                                    .ToArray();

            // 2. 标记 NG 通道
            for (int i = 0; i < resultSliceAllResult.Count; i++)
            {
                var channels = zones[i].Channels;
                if (channels == null) continue;

                foreach (var ch in channels)
                {
                    int index = ch.Id - 1;
                    if (index >= 0 && index < dbTypes.Length &&
                        resultSliceAllResult[index] == ResultType.NG)
                    {
                        dbTypes[index] = (index, (ch.Id).ToString().PadLeft(4, '0'));
                    }
                }
            }

            // 3. 异步发送
            bIZ_ThreadManage.limitThreadPool.RunAction(() =>
            {
                Project.Instance.dbjCardManager.SendDBInfo(FrameID, dbTypes, CamID, out string msg);
                if (Project.Instance.slaveMachineManager.initState)
                {
                    Project.Instance.slaveMachineManager.SendOwnDB(msg);
                }
            });

            HOperatorSet.CountSeconds(out HTuple EndSendDB);
            EndSendDB = (EndSendDB - StartSendDB) * 1000;
            MessageNotify.AddDebugLog($"第{keyA}EA发送DB信息处理时长::{EndSendDB.D}");

        }


        /// <summary>
        /// 分析EA数据
        /// </summary>
        /// <param name="keyA"></param>
        /// <param name="cameraEnum"></param>
        /// <param name="Calipix_x"></param>
        /// <param name="Calipix_y"></param>
        /// <param name="productInfoA"></param>
        /// <param name="productInfoB"></param>
        public void EADataAnalysis(CameraEnum cameraEnum, double Calipix_x, double Calipix_y, PFTCleanEAProductInfo productInfoA, PFTCleanEAProductInfo productInfoB, out List<string> OutDBChannel)
        {
            int keyA = productInfoA.EAID;
            List<string> DBChannel = new List<string>();
            OutDBChannel = DBChannel;
            if ((productInfoA.list2EAShave.Count() | productInfoB.list2EAShave.Count()) != pFTCleanCraft.SliceCount)
            {
                // 报警：切片数量不一致
                MessageNotify.AddErrorLog("ProcessingData_FrameCam2()_切片数量不一致");
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "切片数量不一致", describe = "切片数量不一致" });
            }
            else
            {
                #region 计算EA结果
                HOperatorSet.CountSeconds(out HTuple startTime);
                List<ResultType> resultSliceDefectNg = new List<ResultType>();
                List<ResultType> resultSliceSizeNg = new List<ResultType>();
                List<ResultType> resultSliceAllResult = new List<ResultType>();
                List<ResultType> resultSliceConfigSizeNg = new List<ResultType>();
                List<ResultType> resultSliceGrooveSizeNg = new List<ResultType>();

                ResultType AllDefectNg = ResultType.NG;
                ResultType AllSizeNg = ResultType.NG;
                ResultType AllEANg = ResultType.NG;

                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    resultSliceDefectNg.Add(ResultType.NG);          // 默认 NG
                    resultSliceSizeNg.Add(ResultType.NG);          // 默认 NG
                    resultSliceAllResult.Add(ResultType.NG);          // 默认 NG
                    resultSliceConfigSizeNg.Add(ResultType.NG);          // 默认 NG
                    resultSliceGrooveSizeNg.Add(ResultType.NG);          // 默认 NG
                    DBChannel.Add("NUll");
                }

                // 计算槽位TD&MD尺寸
                MeasureDataDistance(pFTCleanCraft, productInfoA.list2EAShave, productInfoB.list2EAShave, Calipix_x, Calipix_y, out bool bIsSuccess);
                HOperatorSet.CountSeconds(out HTuple endTime);
                endTime = (endTime - startTime) * 1000;
                MessageNotify.AddDebugLog($"第{productInfoA.EAID}EA计算槽位TD&MD尺寸处理时长::{endTime.D}");

                //槽位尺寸判断
                GrooveDataJudge(pFTCleanCraft, productInfoA.list2EAShave, productInfoB.list2EAShave);
                HOperatorSet.CountSeconds(out endTime);
                endTime = (endTime - startTime) * 1000;
                MessageNotify.AddDebugLog($"第{productInfoA.EAID}EA计算槽位尺寸判断处理时长::{endTime.D}");
                //计算配置尺寸项目
                MeasureConfigSize(pFTCleanCraft, productInfoA.list2EAShave, productInfoB.list2EAShave, productInfoA.ListEASizes, productInfoB.ListEASizes, productInfoA.lineInfos, productInfoB.lineInfos, Calipix_x, Calipix_y, out bool bIsSuccess2);
                HOperatorSet.CountSeconds(out endTime);
                endTime = (endTime - startTime) * 1000;
                MessageNotify.AddDebugLog($"第{productInfoA.EAID}EA计算计算配置尺寸项目处理时长::{endTime.D}");
                #region 配置尺寸统计
                // 1. 把 Craft 数据按 Index 做成字典，后面直接查
                var craft = pFTCleanCraft.ListEASizes.ToDictionary(c => c.Index);

                // 3. 处理 A、B 两集合所有尺寸
                productInfoA.ListEASizes
                    .Concat(productInfoB.ListEASizes)
                    .ForEach(item =>
                    {
                        // 3.1 取对应 Craft 的 ArrayBelongSlice 中值为 1 的索引(+1)
                        var slices = craft.TryGetValue(item.Index, out var c)
                            ? c.ArrayBelongSlice
                                .Select((v, i) => new { v, i })
                                .Where(x => x.v == 1)
                                .Select(x => x.i + 1)
                                .ToArray()
                            : Array.Empty<int>();

                        if (keyA > 0)
                        {
                            //// 3.2 按找到的索引累积报警
                            slices.ForEach(i =>
                            AccumulateAlarmCount(
                                item,
                                BIZ_Variable.StatisticalData_EASlices[i - 1]
                                    .sizeItemInfos.Find(s => s.Index == item.Index)));
                        }

                    });

                #endregion

                //尺寸停机报警判断以及统计计数//数据的精细化提取
                List<SlotProduct> slotProducts = GetAllSlots(pFTCleanCraft, productInfoA.list2EAShave, productInfoB.list2EAShave);
                List<List<NgTypeInfo>> ListEADefects = new List<List<NgTypeInfo>>();
                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {

                    #region 缺陷方面判断

                    HOperatorSet.CountSeconds(out HTuple startDeftTime);
                    var defects = productInfoA.ListEADefects.Concat(productInfoB.ListEADefects)
                                                 .Where(d => d.m_iSliceIndex == i)
                                                 .ToList();
                    var defectTJDBBJItemInfos = BIZ_Variable.StatisticalData_EASlices[i].defectTJDBBJItemInfos;
                    //初始化defectTJDBBJItemInfos数据
                    for (int j = 0; j < defectTJDBBJItemInfos.Count; j++)
                    {
                        defectTJDBBJItemInfos[j].CountNGBJ = 0;
                        defectTJDBBJItemInfos[j].CountNGDB = 0;
                        defectTJDBBJItemInfos[j].CountNGTJ = 0;

                    }


                    ListEADefects.Add(defects.Count > 0 ? defects : new List<NgTypeInfo>());
                    ProcessImageDefects(ListEADefects[i], cameraEnum, true, i + 1);
                    resultSliceDefectNg[i] = ListEADefects[i].Count == 0 ? ResultType.OK
                                          : ListEADefects[i].Max(d => d.resultType);
                    if (resultSliceDefectNg[i] == ResultType.NG)
                    {
                        resultSliceDefectNg[i] = ResultType.Warn;
                    }
                    resultSliceDefectNg[i] = ListEADefects[i].Count == 0 ? ResultType.OK
                                         : ListEADefects[i].Max(d => d.resultType);
                    ProductDataDefectStastic(ListEADefects[i], BIZ_Variable.StatisticalData_EASlices[i].defectItemInfos);

                    TJBJDeftInfo_WriteState(cameraEnum, ListEADefects[i], defectTJDBBJItemInfos, pFTCleanCraft.ListDefectInfos, i, resultSliceDefectNg[i]);

                    HOperatorSet.CountSeconds(out HTuple EndDeftTime);
                    EndDeftTime = (EndDeftTime - startDeftTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算缺陷处理时长::{EndDeftTime.D}");
                    #endregion

                    #region 尺寸方面判断
                    HOperatorSet.CountSeconds(out HTuple startSizeTime);
                    //配置尺寸判断 
                    // 配置尺寸判断（一行完成）
                    ResultType result = ResultType.OK;
                    result = pFTCleanCraft.ListEASizes
                        .Where(c => c.ArrayBelongSlice[i] == 1)                                    // 满足切片
                        .Join(productInfoA.ListEASizes.Concat(productInfoB.ListEASizes),           // 与 A+B 连接
                              c => c.Index, s => s.Index, (_, s) => s.resultType) // 同 Index
                        .DefaultIfEmpty(result)                                                    // 万一没有匹配
                        .Max();                                                                    // 取最大
                    HOperatorSet.CountSeconds(out HTuple EndSizeTime);
                    EndSizeTime = (EndSizeTime - startSizeTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算尺寸(配置尺寸判断（一行完成）)处理时长::{EndSizeTime.D}");
                    // 遍历所有尺寸，过滤后一次性处理
                    productInfoA.ListEASizes
                       .Concat(productInfoB.ListEASizes)
                       .Where(s => BIZ_Variable.StatisticalData_EASlices[i].sizeItemInfos.Exists(info => info.Name == s.Name && info.Index == s.Index))
                       .ForEach(s =>
                       {
                           var craftItem = pFTCleanCraft.ListEASizes.Find(c => c.Name == s.Name);
                           var Sizeinfo = BIZ_Variable.StatisticalData_EASlices[i].sizeItemInfos.Find(info => info.Name == s.Name);

                           TJBJSize_WriteState(craftItem.camera, s, Sizeinfo, craftItem, i);
                       });
                    HOperatorSet.CountSeconds(out EndSizeTime);
                    EndSizeTime = (EndSizeTime - startSizeTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算尺寸(遍历所有尺寸)上处理时长::{EndSizeTime.D}");

                    ResultType resultConfigSize = result;
                    result = ResultType.OK;
                    HOperatorSet.CountSeconds(out EndSizeTime);
                    EndSizeTime = (EndSizeTime - startSizeTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算尺寸(遍历所有尺寸)下处理时长::{EndSizeTime.D}");
                    for (int j = 1; j <= pFTCleanCraft.ListSliceSet[i].ShaveNum; j++)
                    {
                        var strSlotName = Enum.GetNames(typeof(MeasureSlotEnum));

                        for (int k = 0; k < strSlotName.Length; k++)
                        {
                            SlotProduct slotProduct = slotProducts.Find(s => s.slotInfo.SliceID == i && s.slotInfo.ShaveID == j);

                            if (slotProduct == null)
                            {
                                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{i + 1}片第{j}个槽位尺寸数据缺失", describe = $"第{i + 1}片第{j + 1}个槽位尺寸数据缺失" });
                                continue;
                            }
                            SizeInfo sizeInfo = slotProduct.SlotSizes[k];
                            SizeItemInfo sizeItemInfo = BIZ_Variable.StatisticalData_EASlices[i].sizeItemInfos.Find(s => s.Description == $"第{i + 1}条第{j}个槽位{strSlotName[k]}");
                            SizeItemInfo CraftSizeItem = CreateSizeItemInfo(pFTCleanCraft.ListSlotSizeLimits[k], sizeInfo);
                            if (keyA > 0)
                            {
                                AccumulateAlarmCount(sizeInfo, sizeItemInfo);
                                TJBJSize_WriteState(cameraEnum, sizeInfo, sizeItemInfo, CraftSizeItem, i);

                            }
                            result = (new ResultType[] { sizeInfo.resultType, result }).Max();

                        }

                    }
                    HOperatorSet.CountSeconds(out EndSizeTime);
                    EndSizeTime = (EndSizeTime - startSizeTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算尺寸(槽位判断)处理时长::{EndSizeTime.D}");

                    resultSliceSizeNg[i] = new ResultType[] { resultConfigSize, result }.Max();
                    resultSliceAllResult[i] = new ResultType[] { resultSliceDefectNg[i], resultSliceSizeNg[i] }.Max();
                    HOperatorSet.CountSeconds(out EndSizeTime);
                    EndSizeTime = (EndSizeTime - startSizeTime) * 1000;
                    MessageNotify.AddDebugLog($"第{productInfoA.EAID}_第{i + 1}条EA计算尺寸处理时长::{EndSizeTime.D}");

                    #endregion

                }
                HOperatorSet.CountSeconds(out endTime);
                endTime = (endTime - startTime) * 1000;
                MessageNotify.AddDebugLog($"第{productInfoA.EAID}EA计算计算总判断处理时长::{endTime.D}");
                AllDefectNg = resultSliceDefectNg.Max();
                AllSizeNg = resultSliceSizeNg.Max();
                AllEANg = resultSliceAllResult.Max();
                // 保存结果到产品信息
                productInfoA.resultSliceDefectNg = resultSliceDefectNg.DeepClone();
                productInfoA.resultSliceSizeNg = resultSliceSizeNg.DeepClone();
                productInfoA.resultSliceAllResult = resultSliceAllResult.DeepClone();
                productInfoA.resultSliceGrooveSizeNg = resultSliceGrooveSizeNg.DeepClone();
                productInfoA.resultSliceConfigSizeNg = resultSliceConfigSizeNg.DeepClone();
                productInfoB.resultSliceDefectNg = resultSliceDefectNg.DeepClone();
                productInfoB.resultSliceSizeNg = resultSliceSizeNg.DeepClone();
                productInfoB.resultSliceAllResult = resultSliceAllResult.DeepClone();
                productInfoB.resultSliceGrooveSizeNg = resultSliceGrooveSizeNg.DeepClone();
                productInfoB.resultSliceConfigSizeNg = resultSliceConfigSizeNg.DeepClone();

                var zones = ((DBJCardParam)Project.Instance.dbjCardManager.tcpClientParam)?.Zones;


                if (zones == null || zones?.Count != 12)
                {
                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "打标机未正确设置条数", describe = "打标机未正确设置条数" });

                }
                else
                {
                    DBChannel = Enumerable.Range(0, pFTCleanCraft.SliceCount)
                    .Select(i => zones?.Count > i
                        ? string.Join(",", zones[i].Channels.Select(c => c.Id.ToString().PadLeft(4, '0')))
                        : "NULL")
                    .ToList();

                }


                if (keyA > 0)
                {
                    for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                    {

                        if (resultSliceAllResult[i] == ResultType.NG)
                        {
                            BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount++;

                        }
                        StatisticalData statisticalData = BIZ_Variable.StatisticalData_EASlices[i].statisticalData;
                        BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);

                    }

                }


                productInfoA.EAFinfished = true;
                productInfoB.EAFinfished = true;

                HOperatorSet.CountSeconds(out endTime);
                endTime = (endTime - startTime) * 1000;
                MessageNotify.AddDebugLog($"第{productInfoA.EAID}EA总数据处理时长::{endTime.D}");
                #endregion
            }
        }


        private void UIAndFileDataSave(int keyA, List<string> DBChannel, PFTCleanEAProductInfo productInfoA, PFTCleanEAProductInfo productInfoB)
        {
            HOperatorSet.CountSeconds(out HTuple StartUIAndFileDataSave);
            //配置尺寸数据保存及显示
            List<RowData> rowDatas = new List<RowData>();
            List<RowData> rowDeftDatas = new List<RowData>();
            List<RowData> rowAllDatas = new List<RowData>();

            rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
            rowDatas.Add(new RowData() { dataStr = $"{productInfoA.EAID}", resultType = ResultType.OK });
            rowDeftDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
            rowDeftDatas.Add(new RowData() { dataStr = $"{productInfoA.EAID}", resultType = productInfoA.resultSliceDefectNg.Max() });
            rowAllDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
            rowAllDatas.Add(new RowData() { dataStr = $"{productInfoA.EAID}", resultType = productInfoA.resultSliceAllResult.Max() });

            foreach (var item in productInfoA.ListEASizes.Concat(productInfoB.ListEASizes))
            {
                SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes.Find(x => x.Index == item.Index);
                if (sizeItemInfo == null)
                {
                    MessageNotify.AddErrorLog($"配置尺寸数据保存异常，找不到对应配置尺寸数据:{item.Name}");
                    continue;
                }
                rowDatas.Add(new RowData() { dataStr = item.DataResult.ToString(), resultType = item.resultType });
                string str = string.Join("_", sizeItemInfo.ArrayBelongSlice
                  .Select((value, index) => new { value, index })
                  .Where(x => x.value == 1.0)
                 .Select(x => x.index)
                     .ToArray());
                rowDatas.Add(new RowData() { dataStr = item.resultType.ToString(), resultType = item.resultType });
                rowDatas.Add(new RowData() { dataStr = str, resultType = item.resultType });
                ChartDataShow(ChartConfigPage[item.Index], item.DataResult);
            }

            NDataPanelViewModel.Add(form.DpModelSizeConfig, BIZ_Variable.strEASizeConfigTitle.ToArray(), rowDatas.ToArray());
            FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizeConfigPath, BIZ_Variable.strEASizeConfigTitle.ToArray(), rowDatas.ToArray(), "");


            //槽位尺寸数据保存及显示
            List<SlotProduct> slotProducts = GetAllSlots(pFTCleanCraft, productInfoA.list2EAShave, productInfoB.list2EAShave);


            for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
            {
                rowDatas?.Clear();
                rowDatas = new List<RowData>();
                rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDatas.Add(new RowData() { dataStr = $"{productInfoA.EAID}", resultType = productInfoA.resultSliceSizeNg.Max() });
                rowDatas.Add(new RowData() { dataStr = $"{i + 1}", resultType = productInfoA.resultSliceSizeNg[i] });
                for (int j = 1; j <= pFTCleanCraft.ListSliceSet[i].ShaveNum; j++)
                {
                    var strSlotName = Enum.GetNames(typeof(MeasureSlotEnum));

                    for (int k = 0; k < strSlotName.Length; k++)
                    {
                        SlotProduct slotProduct = slotProducts.Find(s => s.slotInfo.SliceID == i && s.slotInfo.ShaveID == j);

                        if (slotProduct == null)
                        {
                            MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{i + 1}片第{j}个槽位尺寸数据缺失", describe = $"第{i + 1}片第{j + 1}个槽位尺寸数据缺失" });
                            continue;
                        }
                        SizeInfo sizeInfo = slotProduct.SlotSizes[k];
                        rowDatas.Add(new RowData() { dataStr = sizeInfo.DataResult.ToString(), resultType = sizeInfo.resultType });
                        rowDatas.Add(new RowData() { dataStr = sizeInfo.resultType.ToString(), resultType = sizeInfo.resultType });
                        ChartDataGrooveShow(i, j - 1, k, sizeInfo.DataResult);

                    }

                }
                rowDatas.Add(new RowData() { dataStr = productInfoA.resultSliceGrooveSizeNg[i].ToString(), resultType = productInfoA.resultSliceGrooveSizeNg[i] });
                rowDatas.Add(new RowData() { dataStr = productInfoA.resultSliceConfigSizeNg[i].ToString(), resultType = productInfoA.resultSliceConfigSizeNg[i] });
                rowDatas.Add(new RowData() { dataStr = productInfoA.resultSliceSizeNg[i].ToString(), resultType = productInfoA.resultSliceSizeNg[i] });
                rowDeftDatas.Add(new RowData() { dataStr = productInfoA.resultSliceDefectNg[i].ToString(), resultType = productInfoA.resultSliceDefectNg[i] });
                rowAllDatas.Add(new RowData() { dataStr = productInfoA.resultSliceSizeNg[i].ToString(), resultType = productInfoA.resultSliceSizeNg[i] });
                rowAllDatas.Add(new RowData() { dataStr = productInfoA.resultSliceDefectNg[i].ToString(), resultType = productInfoA.resultSliceDefectNg[i] });
                rowAllDatas.Add(new RowData() { dataStr = productInfoA.resultSliceAllResult[i].ToString(), resultType = productInfoA.resultSliceAllResult[i] });
                rowAllDatas.Add(new RowData() { dataStr = DBChannel[i], resultType = productInfoA.resultSliceAllResult[i] });


                NDataPanelViewModel.Add(form.DpModelEASize, BIZ_Variable.strEASizeTitle.ToArray(), rowDatas.ToArray());
                FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizePath, BIZ_Variable.strEASizeTitle.ToArray(), rowDatas.ToArray(), "");

            }
            rowDeftDatas.Add(new RowData() { dataStr = productInfoA.resultSliceDefectNg.Max().ToString(), resultType = productInfoA.resultSliceDefectNg.Max() });

            NDataPanelViewModel.Add(form.DpModelEADetect, BIZ_Variable.strEADeftTitle.ToArray(), rowDeftDatas.ToArray());
            FileOperationCSV.WriteData_CSV(BIZ_Variable.strEADeftPath, BIZ_Variable.strEADeftTitle.ToArray(), rowDeftDatas.ToArray(), "");
            NDataPanelViewModel.Add(form.DpModelEADetectSize, BIZ_Variable.strEASizeDeftTitle.ToArray(), rowAllDatas.ToArray());
            FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizeDeftPath, BIZ_Variable.strEASizeDeftTitle.ToArray(), rowAllDatas.ToArray(), "");


            //开始统计数据
            UIService.Instance.AddAction(() =>
             {
                 for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                 {
                     StatisticalData statisticalData = BIZ_Variable.StatisticalData_EASlices[i].statisticalData;
                     BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[0], statisticalData.SumupCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[1], $"{statisticalData.Yield}%");
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[2], statisticalData.OKCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[3], statisticalData.NGCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[4], statisticalData.NGCount.ToString());

                 }
             });

            StasticShowAndSave();
            HOperatorSet.CountSeconds(out HTuple EndUIAndFileDataSave);
            EndUIAndFileDataSave = (EndUIAndFileDataSave - StartUIAndFileDataSave) * 1000;
            MessageNotify.AddDebugLog($"UI和文件数据保存时长::{EndUIAndFileDataSave.D}");


        }

        private void FrameMethord(CameraEnum cameraEnum, PFTImageProduct pFTImageProduct, DynamicDataPanel dataPanelViewModelBase, DisplayWindowClass displayWindowClass, string[] strDeftInitCamTitle = null, string strDeftInitCamPath = null)
        {
            ExecuteMethod(() =>
            {
                //【1】工艺处理             
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_StartDataProcessTime);
                ResultType bIsFrameDefectNg = ResultType.NG;

                List<ResultType> resultSliceDefectNg = new List<ResultType>();

                List<List<NgTypeInfo>> ListImageDefects = new List<List<NgTypeInfo>>();

                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    resultSliceDefectNg.Add(ResultType.NG);          // 默认 NG

                    var defects = pFTImageProduct.ListImageDefects
                                                 .Where(d => d.m_iSliceIndex == i)
                                                 .ToList();

                    ListImageDefects.Add(defects.Count > 0 ? defects : new List<NgTypeInfo>());
                }

                //获取分界值
                // 1) 分界索引
                int lastCam1Index = pFTCleanCraft.ListSliceSet
                                        .Select((info, idx) => (info.BelongCam, idx))
                                        .LastOrDefault(x => x.BelongCam == CameraEnum.Cam1.ToString())
                                        .idx;



                //缺陷判断
                for (int i = 0; i < ListImageDefects.Count; i++)
                {
                    string strDeftInfo = "无";
                    ProcessImageDefects(ListImageDefects[i], cameraEnum, false, i + 1);
                    var defects = ListImageDefects[i];

                    // 根据相机及分界索引决定是否显示
                    bool needShow = cameraEnum == CameraEnum.Cam1 ? i <= lastCam1Index
                                                                  : i > lastCam1Index;
                    if (defects.Count == 0)
                    {
                        resultSliceDefectNg[i] = ResultType.OK;
                        strDeftInfo = string.Empty;
                    }
                    else
                    {
                        resultSliceDefectNg[i] = defects.Max(d => d.resultType);

                        strDeftInfo = string.Join("|_|", defects.Select(d =>
                            $"当前打标类型：{d.resultType}, 缺陷类型：{d.m_strNgTypeName}, " +
                            $"面积：{d.m_fNgTypArea}, 高度：{d.m_fNgTypHeight}, 宽度：{d.m_fNgTypWidth}, " +
                            $"位置：({d.NgMidColumn},{d.NgMidRow})"));
                    }
                    if (needShow)
                    {

                        foreach (var item in defects)
                        {
                            List<RowData> strDeftData = new List<RowData>
                        {
                            new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK },
                            new RowData() { dataStr = $"{pFTImageProduct.showHalconWindow.m_nImageFrameID}", resultType =resultSliceDefectNg[i] },
                            new RowData() { dataStr = $"{pFTImageProduct.nCurEAID}", resultType = ResultType.OK },
                            new RowData() { dataStr = $"{i + 1}", resultType =resultSliceDefectNg[i]  },
                            new RowData() { dataStr = item.m_strNgTypeName.ToString(), resultType =  resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.m_fNgTypWidth.ToString("0.00"), resultType = resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.m_fNgTypHeight.ToString("0.00"), resultType = resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.m_fNgTypArea.ToString("0.00"), resultType = resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.bIsSave.ToString(), resultType = resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.bIsDB.ToString(), resultType = resultSliceDefectNg[i] },
                            new RowData() { dataStr =item.strPath.ToString(), resultType = resultSliceDefectNg[i] },
                            };

                            FileOperationCSV.WriteData_CSV(strDeftInitCamPath, strDeftInitCamTitle, strDeftData.ToArray(), cameraEnum.ToString());
                            NDataPanelViewModel.Add(dataPanelViewModelBase, strDeftInitCamTitle, strDeftData.ToArray());
                        }


                        BIZ_General.AddMessageShowWindow(ListImageDefects[i], pFTImageProduct.showHalconWindow, i + 1);
                    }

                }

                bIsFrameDefectNg = resultSliceDefectNg.Max();


                pFTImageProduct.showHalconWindow.m_nMode = 0;
                pFTImageProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTImageProduct.showHalconWindow.m_bIsNG = bIsFrameDefectNg == (ResultType.NG);
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = bIsFrameDefectNg.ToString(), m_stuGetColor = BIZ_General.Color(bIsFrameDefectNg) }); ;
                PrimitiveTransition(cameraEnum, pFTImageProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_DataProcessTime);
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTImageProduct);
                displayWindowClass.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);
            });
        }


        private void EAFrameMethord(CameraEnum cameraEnum, PFTCleanEAProductInfo pFTImageProduct, DisplayWindowClass displayWindowClass, string[] strDeftInitCamTitle = null, string strDeftInitCamPath = null)
        {
            ExecuteMethod(() =>
            {
                if (!pFTImageProduct.EAFinfished)
                {
                    //报警：未完成切片
                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = $"相机{cameraEnum}第{pFTImageProduct.EAID}个产品未完成切片", describe = $"相机{cameraEnum}第{pFTImageProduct.EAID}个产品未完成切片" });
                    MessageNotify.AddErrorLog($"相机{cameraEnum}第{pFTImageProduct.EAID}个产品未完成切片");
                    pFTImageProduct.resultSliceSizeNg?.Clear();
                    pFTImageProduct.resultSliceDefectNg?.Clear();
                    pFTImageProduct.resultSliceAllResult?.Clear();
                    pFTImageProduct.resultSliceSizeNg = new List<ResultType>();
                    pFTImageProduct.resultSliceDefectNg = new List<ResultType>();
                    pFTImageProduct.resultSliceAllResult = new List<ResultType>();
                    for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                    {
                        pFTImageProduct.resultSliceSizeNg.Add(ResultType.NG);          // 默认 NG
                        pFTImageProduct.resultSliceDefectNg.Add(ResultType.NG);          // 默认 NG
                        pFTImageProduct.resultSliceAllResult.Add(ResultType.NG);          // 默认 NG
                    }
                }
                else
                {


                }
                if (pFTImageProduct.resultSliceSizeNg?.Count == 0)
                {

                }


                //【1】工艺处理             
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_StartDataProcessTime);


                List<ResultType> resultSliceDefectNg = new List<ResultType>();

                List<List<NgTypeInfo>> ListImageDefects = new List<List<NgTypeInfo>>();

                //获取分界值
                // 1) 分界索引
                int lastCam1Index = pFTCleanCraft.ListSliceSet
                                        .Select((info, idx) => (info.BelongCam, idx))
                                        .LastOrDefault(x => x.BelongCam == CameraEnum.Cam1.ToString())
                                        .idx;

                // 2) 一次性完成缺陷收集、结果计算及显示
                ResultType resultTypeSize = ResultType.OK;
                ResultType resultTypeDefect = ResultType.OK;
                ResultType resultTypeAll = ResultType.OK;
                resultSliceDefectNg = Enumerable.Range(0, pFTCleanCraft.SliceCount)
                    .Select(i =>
                    {
                        try
                        {
                            // 缺陷判断 
                            List<NgTypeInfo> defects = pFTImageProduct.ListEADefects
                                                         .Where(d => d.m_iSliceIndex == i)
                                                         .ToList();                 // 当前切片缺陷

                            ListImageDefects.Add(defects);                          // 缓存缺陷列表

                            // 缺陷结果：无缺陷->OK，有缺陷->最大级别
                            var result = defects.Count == 0 ? ResultType.OK
                                                            : defects.Max(d => d.resultType);

                            // 根据相机及分界索引决定是否显示
                            bool needShow = cameraEnum == CameraEnum.Cam1 ? i <= lastCam1Index
                                                                          : i > lastCam1Index;
                            if (needShow)
                            {
                                resultTypeSize = (new ResultType[] { resultTypeSize, pFTImageProduct.resultSliceSizeNg[i] }).Max();
                                resultTypeDefect = (new ResultType[] { resultTypeDefect, pFTImageProduct.resultSliceDefectNg[i] }).Max();
                                resultTypeAll = (new ResultType[] { resultTypeAll, pFTImageProduct.resultSliceAllResult[i] }).Max();
                                BIZ_General.AddMessageShowWindow(defects, pFTImageProduct.showHalconWindow, i + 1);
                                pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"第{i + 1}切片尺寸结果：{pFTImageProduct.resultSliceSizeNg[i].ToString()}", m_stuGetColor = BIZ_General.Color(pFTImageProduct.resultSliceSizeNg[i]) });
                                pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"第{i + 1}切片缺陷结果：{pFTImageProduct.resultSliceDefectNg[i].ToString()}", m_stuGetColor = BIZ_General.Color(pFTImageProduct.resultSliceDefectNg[i]) });
                                pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"第{i + 1}切片总结果：{pFTImageProduct.resultSliceAllResult[i].ToString()}", m_stuGetColor = BIZ_General.Color(pFTImageProduct.resultSliceAllResult[i]) });

                            }

                            return result;
                        }
                        catch (Exception ee)
                        {

                            MessageNotify.AddErrorLog($"相机{cameraEnum}第{pFTImageProduct.EAID}个产品第{i + 1}切片缺陷判断异常：{ee.Message}");
                            return ResultType.NG;
                        }



                    })
                    .ToList();

                //配置尺寸显示
                foreach (var size in pFTImageProduct.ListEASizes)
                {
                    pFTImageProduct.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{size.Description}:{size.DataResult}_({size.StandardValue - size.DBInfo.DownTolerance},{size.StandardValue + size.DBInfo.UpTolerance})", m_stuGetColor = BIZ_General.Color(size.resultType) });
                }

                pFTImageProduct.showHalconWindow.m_nMode = 1;
                pFTImageProduct.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTImageProduct.showHalconWindow.m_bIsNG = resultTypeAll == (ResultType.NG);
                if (pFTImageProduct.EAID == 0)
                {
                    pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = "第0个产品默认不统计", m_stuGetColor = BIZ_General.Color(resultTypeAll), m_nFontSize = 30 }); ;

                }
                pFTImageProduct.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = resultTypeAll.ToString(), m_stuGetColor = BIZ_General.Color(resultTypeAll), m_nFontSize = 30 }); ;
                PrimitiveTransitionEA(cameraEnum, pFTImageProduct.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HOperatorSet.CountSeconds(out pFTImageProduct.Image_DataProcessTime);

                HOperatorSet.CountSeconds(out pFTImageProduct.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTImageProduct);
                displayWindowClass.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);



            });
        }


        //统计数据保存
        private void StasticShowAndSave()
        {
            ExecuteMethod(() =>
            {
                //保存统计数据
                List<string[]> dataList = new List<string[]>();
                dataList.Add(BIZ_Variable.DgvColumnheaders.ToArray());
                List<string> strings = new List<string>();
                for (int i = 0; i < BIZ_Variable.DgvRowheaders.Count; i++)
                {
                    strings?.Clear();
                    strings.Add(BIZ_Variable.DgvRowheaders[i]);
                    for (int j = 0; j < pFTCleanCraft.SliceCount; j++)
                    {
                        switch (BIZ_Variable.DgvRowheaders[i])
                        {
                            case "产量":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData.SumupCount.ToString());
                                break;
                            case "良率":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData.Yield.ToString() + "%");
                                break;
                            case "OK数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData.OKCount.ToString());
                                break;
                            case "NG数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount.ToString());
                                break;
                            case "DB数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount.ToString());
                                break;
                        }
                    }
                    dataList.Add(strings.ToArray());
                }
                dataList.Add(new string[] { "———-------" });

                //缺项细分统计

                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    if (i == 0)
                    {
                        List<string> defectNames = new List<string> { "条号" }
                     .Concat(BIZ_Variable.StatisticalData_EASlices[i].defectItemInfos.Select(item => item.Description))
                     .ToList();
                        dataList.Add(defectNames.ToArray());
                    }


                    dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlices[i].defectItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());
                }

                dataList.Add(new string[] { "———-------" });

                //尺寸细分统计
                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    List<string> SizeNames = new List<string> { "条号" }
                  .Concat(BIZ_Variable.StatisticalData_EASlices[i].sizeItemInfos.Select(item => item.Description))
                     .ToList();
                    dataList.Add(SizeNames.ToArray());

                    dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlices[i].sizeItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());

                }

                //listview显示
                UIService.Instance.AddAction(() =>
                {
                    form.listNgInfo.Items.Clear();
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        NListView2.Add(form.listNgInfo, dataList[i], 300, ListViewShowType.Clear, false);

                    }
                });


                //保存到csv
                FileOperationCSV.WriteData_CSV2(BIZ_Variable.strProductDataPath, dataList, "");

            });
        }



        public void InitChart()
        {

            ExecuteMethod(() =>
            {
                ChartConfigPage?.Clear();
                form.tabChartMain.Controls?.Clear();

                int rowCount = 4;
                int colCount = 2;
                //初始化配置图表控件
                foreach (var item in pFTCleanCraft.ListEASizes)
                {
                    ChartConfigPage[item.Index] = new ChartUIParam(item.Description, new ScottPlot.FormsPlot());
                }

                if (pFTCleanCraft.ListEASizes.Count > 0)
                {
                    int chartCount = pFTCleanCraft.ListEASizes.Count;



                    int pageCount = (chartCount + 7) / 8;
                    //tabcontrol先清空
                    form.tabChartMain.TabPages.Clear();

                    for (int i = 0; i < pageCount; i++)
                    {

                        //再添加页
                        form.tabChartMain.TabPages.Add(new System.Windows.Forms.TabPage());
                        form.tabChartMain.TabPages[i].Text = $"第{i + 1}页";
                        form.tabChartMain.TabPages[i].BackColor = Color.White;
                        form.tabChartMain.TabPages[i].ForeColor = Color.Black;
                        form.tabChartMain.TabPages[i].Padding = new Padding(10);
                        form.tabChartMain.TabPages[i].AutoScroll = true;
                        form.tabChartMain.TabPages[i].AutoScrollMargin = new Size(10, 10);
                        form.tabChartMain.TabPages[i].AutoScrollMinSize = new Size(100, 100);
                        //form.tabChartMain.TabPages[i].UseWaitCursor = true;
                        form.tabChartMain.TabPages[i].Cursor = Cursors.Hand;
                        form.tabChartMain.TabPages[i].Dock = DockStyle.Fill;
                        //添加tableLayoutPanel,初始化控件rowCount，colCount
                        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                        tableLayoutPanel.RowCount = rowCount;
                        tableLayoutPanel.ColumnCount = colCount;
                        tableLayoutPanel.Dock = DockStyle.Fill;
                        tableLayoutPanel.AutoScroll = true;
                        //设置行样式为均分
                        tableLayoutPanel.RowStyles.Clear();
                        for (int j = 0; j < rowCount; j++)
                        {
                            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
                        }

                        //设置列样式为均分
                        tableLayoutPanel.ColumnStyles.Clear();
                        for (int j = 0; j < colCount; j++)
                        {
                            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));
                        }

                        //添加图表控件到 TableLayoutPanel
                        for (int j = 0; j < 8; j++)
                        {
                            int col = j % colCount;
                            int row = j / colCount;
                            if (i * 8 + j < chartCount)
                            {
                                ChartConfigPage[pFTCleanCraft.ListEASizes[i * 8 + j].Index].plotView.Dock = DockStyle.Fill;
                                ParamsChart paramsChart = new ParamsChart();
                                paramsChart.f_UpLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Up;
                                paramsChart.f_DownLimit = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue + pFTCleanCraft.ListEASizes[i * 8 + j].StD_Down;
                                paramsChart.f_StandarValue = pFTCleanCraft.ListEASizes[i * 8 + j].StDValue;
                                paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                                ChartConfigPage[pFTCleanCraft.ListEASizes[i * 8 + j].Index].InitCurve(paramsChart);
                                tableLayoutPanel.Controls.Add(ChartConfigPage[pFTCleanCraft.ListEASizes[i * 8 + j].Index].plotView, col, row);
                            }
                        }

                        //添加到tabcontrol
                        form.tabChartMain.TabPages[i].Controls.Add(tableLayoutPanel);



                    }


                }




            });



        }


        public void InitGrooveChart(bool flag = false)
        {
            ExecuteMethod(() =>
            {

                //初始化切片图表控件
                var GrooveSize = Enum.GetNames(typeof(MeasureSlotEnum));
                ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<ParamsChart>>>>> _data3DCacheGroup = new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<ParamsChart>>>>>();

                if (ChartGroovePage.Count > 0 && flag)
                {
                    for (int i = 0; i < GrooveSize.Length; i++)
                    {
                        _data3DCacheGroup[i] = ChartGroovePage[i]._data3DCache;
                    }
                }
                ChartGroovePage?.Clear();

                for (int i = 0; i < GrooveSize.Length; i++)
                {


                    ChartGroovePage[i] = new ChartUIParam(GrooveSize[i], new ScottPlot.FormsPlot());

                }

                form.tab_GrooveChart.Controls?.Clear();
                //默认2*3

                int rowCount = 3;
                int colCount = 2;


                form.tab_GrooveChart.RowCount = rowCount;
                form.tab_GrooveChart.ColumnCount = colCount;
                form.tab_GrooveChart.Dock = DockStyle.Fill;
                //设置行样式为均分
                form.tab_GrooveChart.RowStyles.Clear();
                for (int j = 0; j < rowCount; j++)
                {
                    form.tab_GrooveChart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
                }

                //设置列样式为均分
                form.tab_GrooveChart.ColumnStyles.Clear();
                for (int j = 0; j < colCount; j++)
                {
                    form.tab_GrooveChart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));
                }
                for (int i = 0; i < GrooveSize.Length; i++)
                {
                    int sliceIndex = form.GetCob_SliceIndex();


                    if (sliceIndex == -1)
                    {
                        MessageNotify.AddErrorLog("InitGrooveChart()_切片索引未选择");
                        return;
                    }
                    int grooveIndex = form.GetCob_GrooveIndex();
                    if (grooveIndex == -1)
                    {
                        MessageNotify.AddErrorLog("InitGrooveChart()_切片类型未选择");
                        return;
                    }

                    var slot = pFTCleanCraft.ListSlotInfos?
                       .FirstOrDefault(s => s.SliceID == sliceIndex + 1 && s.ShaveID == grooveIndex + 1);
                    if (slot == null)
                    {
                        MessageNotify.AddErrorLog("InitGrooveChart()_切片信息为空");
                        return;
                    }
                    ChartGroovePage[i].plotView.Dock = DockStyle.Fill;
                    ParamsChart paramsChart = new ParamsChart();
                    paramsChart.f_UpLimit = pFTCleanCraft.ListSlotSizeLimits[i].UpLimitValue + slot.AllValues[i];
                    paramsChart.f_DownLimit = pFTCleanCraft.ListSlotSizeLimits[i].lowLimitValue + slot.AllValues[i];
                    paramsChart.f_StandarValue = slot.AllValues[i];
                    paramsChart.points.Add(new PointClass() { CurName = "测量值", Point = new Point(0, 0), Color = Color.Blue });
                    ChartGroovePage[i].InitCurve(paramsChart);
                    ChartGroovePage[i].Init3DCache(pFTCleanCraft.ListSliceSet?.Select(d => d.ShaveNum).ToArray(), GrooveSize.Length);

                    if (ChartGroovePage.Count > 0 && flag)
                    {
                        if (_data3DCacheGroup?.Count > 0)
                        {
                            ChartGroovePage[i]._data3DCache = _data3DCacheGroup[i];
                        }

                    }

                    form.tab_GrooveChart.Controls.Add(ChartGroovePage[i].plotView, i % colCount, i / colCount);
                }
            });
        }
        /// <summary>
        /// 槽位尺寸判断
        /// </summary>
        /// <param name="cleanCraft"></param>
        /// <param name="slotMatrixA"></param>
        /// <param name="slotMatrixB"></param>
        public void GrooveDataJudge(PFTCleanCraft cleanCraft,
             List<List<SlotProduct>> slotMatrixA,
             List<List<SlotProduct>> slotMatrixB)
        {
            ExecuteMethod(() =>
            {
                // 2. 一次性拼合：0..splitIndex 取 A，其余取 B
                var allSlots = GetAllSlots(cleanCraft, slotMatrixA, slotMatrixB);
                for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
                {
                    for (int shaveId = 1; shaveId <= cleanCraft.ListSliceSet[sliceId].ShaveNum; shaveId++)
                    {
                        var cur = allSlots.Find(s => s.slotInfo.ShaveID == shaveId &&
                                                     s.slotInfo.SliceID == sliceId);
                        if (cur == null)
                        {
                            MessageNotify.AddErrorLog("GrooveDataJudge()_切片信息为空");
                            continue;
                        }
                        var strs = Enum.GetNames(typeof(MeasureSlotEnum));
                        for (int i = 0; i < strs.Length; i++)
                        {
                            SizeItemInfo sizeItemInfoCur = CreateSizeItemInfo(cleanCraft.ListSlotSizeLimits[i], cur.SlotSizes[i]);
                            SizeInfo sizeInfoCur = cur.SlotSizes[i];
                            sizeInfoCur.DataResult = Math.Round(sizeInfoCur.DataResult * cur.slotInfo.RealFixRito[i] + cur.slotInfo.RealFixOffset[i], 2);
                            if (sizeItemInfoCur.Enable)
                            {
                                GetSizeJudgeInfo(sizeItemInfoCur, ref sizeInfoCur);

                            }
                            else
                            {
                                (sizeInfoCur.resultType, sizeInfoCur.IsNG) = (ResultType.OK, false);
                            }

                            cur.SlotSizes[i] = sizeInfoCur;

                        }
                    }
                }
            });
        }


        /// <summary>
        /// 计算 TD 与 MD 方向槽位间距并写回 SlotProduct。
        /// </summary>
        /// <param name="cleanCraft">工艺参数对象。</param>
        /// <param name="slotMatrixA">第一组槽位矩阵。</param>
        /// <param name="slotMatrixB">第二组槽位矩阵。</param>
        /// <param name="pixelSizeX">X 方向像素物理尺寸（mm/px）。</param>
        /// <param name="pixelSizeY">Y 方向像素物理尺寸（mm/px）。</param>
        /// <param name="isSuccess">输出：计算是否完全成功。</param>
        public void MeasureDataDistance(
            PFTCleanCraft cleanCraft,
            List<List<SlotProduct>> slotMatrixA,
            List<List<SlotProduct>> slotMatrixB,
            double pixelSizeX,
            double pixelSizeY,
            out bool isSuccess)
        {
            isSuccess = ExecuteMethod(() =>
            {
                /* ====== 1. 参数合法性校验 ====== */
                if (cleanCraft?.ListSliceSet?[0] == null)
                    return false;

                /* ====== 2. 扁平化所有槽位 ====== */
                // 1. 找到分界点（最后一个 Cam1 的索引）
                int splitIndex = cleanCraft.ListSliceSet
                                           .Select((info, idx) => new { info.BelongCam, idx })
                                           .LastOrDefault(x => x.BelongCam == CameraEnum.Cam1.ToString())
                                           ?.idx ?? -1;

                // 2. 一次性拼合：0..splitIndex 取 A，其余取 B
                var allSlots = GetAllSlots(cleanCraft, slotMatrixA, slotMatrixB);
                if (!cleanCraft.ListSlotSizeLimits[(int)MeasureSlotEnum.MD间距].Enable && !cleanCraft.ListSlotSizeLimits[(int)MeasureSlotEnum.TD间距].Enable)
                {
                    return true;
                }
                /* ====== 3. TD 间距计算 ====== */

                for (int shaveId = 0; shaveId < cleanCraft.ListSliceSet[0].ShaveNum; shaveId++)
                {
                    //条从0开始
                    for (int sliceId = 0; sliceId < cleanCraft.SliceCount - 1; sliceId++)
                    {
                        var cur = allSlots.Find(s => s.slotInfo.ShaveID == shaveId + 1 &&
                                                      s.slotInfo.SliceID == sliceId);
                        var next = allSlots.Find(s => s.slotInfo.ShaveID == shaveId + 1 &&
                                                      s.slotInfo.SliceID == sliceId + 1);
                        if (cur == null || next == null)
                        {
                            MessageNotify.AddErrorLog("TD slot data missing.");
                            return false;
                        }

                        double tdWidth = Math.Abs(
                            (next.Slotlines[(int)DirectionEnum.右边].ColBegin +
                             next.Slotlines[(int)DirectionEnum.右边].ColEnd -
                             cur.Slotlines[(int)DirectionEnum.左边].ColBegin -
                             cur.Slotlines[(int)DirectionEnum.左边].ColEnd) * 0.5 * pixelSizeX);

                        if (cur.slotInfo.BelongCam != next.slotInfo.BelongCam)
                        {
                            double exOffset = cleanCraft.EXCam1Col - cleanCraft.EXCam2Col - cleanCraft.Cam1BaseCol;
                            tdWidth = Math.Abs(
                            ((next.Slotlines[(int)DirectionEnum.右边].ColBegin +
                             next.Slotlines[(int)DirectionEnum.右边].ColEnd -
                             cur.Slotlines[(int)DirectionEnum.左边].ColBegin -
                             cur.Slotlines[(int)DirectionEnum.左边].ColEnd) * 0.5 + exOffset) * pixelSizeX);

                        }

                        cur.SlotSizes[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;
                        next.SlotSizes[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;

                    }
                }

                /* ====== 4. MD 间距计算 ====== */
                for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
                {
                    for (int shaveId = 1; shaveId <= cleanCraft.ListSliceSet[0].ShaveNum - 1; shaveId++)
                    {
                        var cur = allSlots.Find(s => s.slotInfo.ShaveID == shaveId &&
                                                      s.slotInfo.SliceID == sliceId);
                        var next = allSlots.Find(s => s.slotInfo.ShaveID == shaveId + 1 &&
                                                      s.slotInfo.SliceID == sliceId);
                        if (cur == null || next == null)
                        {
                            MessageNotify.AddErrorLog("MD slot data missing.");
                            return false;
                        }

                        double mdWidth = Math.Abs(
                            (next.Slotlines[(int)DirectionEnum.上边].RowBegin +
                             next.Slotlines[(int)DirectionEnum.上边].RowEnd -
                             cur.Slotlines[(int)DirectionEnum.上边].RowBegin -
                             cur.Slotlines[(int)DirectionEnum.上边].RowEnd) * 0.5 * pixelSizeY);

                        cur.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult = mdWidth;
                        next.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult = mdWidth;

                    }
                }

                return true; // 全部成功
            });
        }



        /// <summary>
        /// 根据配置尺寸信息，在线阵图像中测量实际物理尺寸。
        /// </summary>
        /// <param name="cleanCraft">设备整体配置对象，包含槽位、切片、相机等全局信息。</param>
        /// <param name="slotMatrixA">Cam1 对应的槽位矩阵（行=切片，列=Shave）。</param>
        /// <param name="slotMatrixB">Cam2 对应的槽位矩阵。</param>
        /// <param name="sizeInfosA">Cam1 需要计算的尺寸信息列表。</param>
        /// <param name="sizeInfosB">Cam2 需要计算的尺寸信息列表。</param>
        /// <param name="lineInfosA">Cam1 已提取的线集合。</param>
        /// <param name="lineInfosB">Cam2 已提取的线集合。</param>
        /// <param name="pixelSizeX">X 方向像素尺寸（mm/pixel）。</param>
        /// <param name="pixelSizeY">Y 方向像素尺寸（mm/pixel）。</param>
        /// <param name="isSuccess">返回测量是否全部成功；失败时可通过 <see cref="MessageNotify"/> 查看详细错误。</param>

        public void MeasureConfigSize(
            PFTCleanCraft cleanCraft,
            List<List<SlotProduct>> slotMatrixA,
            List<List<SlotProduct>> slotMatrixB,
            List<SizeInfo> sizeInfosA,
            List<SizeInfo> sizeInfosB,
            List<LineInfo> lineInfosA,
            List<LineInfo> lineInfosB,
            double pixelSizeX,
            double pixelSizeY,
            out bool isSuccess)
        {
            try
            {
                /* ---------- 参数校验 ---------- */
                if (cleanCraft?.ListSliceSet?[0] == null)
                    MessageNotify.AddErrorLog("CleanCraft 参数不完整");

                /* ---------- 扁平化所有槽位 ---------- */
                var allSlots = GetAllSlots(cleanCraft, slotMatrixA, slotMatrixB);

                /* ---------- 配置尺寸计算 ---------- */
                var sizeMatrix = new[] { sizeInfosA, sizeInfosB };
                var lineMatrix = new[] { lineInfosA, lineInfosB };

                for (var cam = CameraEnum.Cam1; cam <= CameraEnum.Cam2; cam++)
                {
                    var listIdx = (int)cam;
                    foreach (var sizeInfo in sizeMatrix[listIdx])
                    {
                        var cfg = cleanCraft.ListEASizes
                                            .Find(x => x.Name == sizeInfo.Name && x.camera == cam);
                        if (cfg == null)
                            MessageNotify.AddErrorLog($"未找到配置尺寸：{sizeInfo.Name},需要检查工艺设置，当前尺寸名称为：{sizeInfo.Description}");

                        var line1 = GetLineInfo(cfg.Line1, cfg.ShaveIndex1, cfg.camera1, allSlots, lineMatrix);
                        var line2 = GetLineInfo(cfg.Line2, cfg.ShaveIndex2, cfg.camera2, allSlots, lineMatrix);

                        if (line1 == null || line2 == null)
                        {
                            MessageNotify.AddErrorLog($"未找到配置线：{cfg.Line1}、{cfg.Line2}，需要检查工艺设置，当前尺寸名称为：{sizeInfo.Description}");
                            goto Step;
                        }

                        switch (cfg.EditorType)
                        {
                            case nameof(EditorTypeEnum.线距离X):
                                var offset = cfg.camera1 != cfg.camera2
                                           ? cleanCraft.EXCam1Col - cleanCraft.EXCam2Col - cleanCraft.Cam1BaseCol
                                           : 0.0;

                                sizeInfo.DataResult = Math.Abs(
                                    ((line2.ColBegin + line2.ColEnd - line1.ColBegin - line1.ColEnd) * 0.5 + offset) * pixelSizeX);
                                sizeInfo.DataResult = Math.Round(sizeInfo.DataResult * cfg.StD_RealFixRito + cfg.StD_RealFixOffset, 2);
                                break;


                            case nameof(EditorTypeEnum.线距离Y):
                                sizeInfo.DataResult = Math.Abs(
                                    ((line2.RowBegin + line2.RowEnd - line1.RowBegin - line1.RowEnd) * 0.5) * pixelSizeY);
                                sizeInfo.DataResult = Math.Round(sizeInfo.DataResult * cfg.StD_RealFixRito + cfg.StD_RealFixOffset, 2);
                                break;

                        }

                    Step:
                        //获取sizeInfo在 sizeMatrix[listIdx]中的索引
                        int sizeInfoIndex = sizeMatrix[listIdx].FindIndex(x => x.Index == sizeInfo.Index);
                        //获取sizeInfo在 allSlots中的索引
                        SizeInfo sizeTemp = sizeMatrix[listIdx][sizeInfoIndex];
                        GetSizeJudgeInfo(cfg, ref sizeTemp);

                    }
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.Message);
                isSuccess = false;
            }

            /* ---------- 本地方法 ---------- */



        }


        /// <summary>
        /// 依据 SlotSizeLimit 与 SizeInfo 创建 SizeItemInfo 实例
        /// </summary>
        /// <param name="limit">槽位尺寸限制配置</param>
        /// <param name="size">当前尺寸结果</param>
        /// <returns>已初始化的 SizeItemInfo</returns>
        private SizeItemInfo CreateSizeItemInfo(SlotSizeLimit limit, SizeInfo size)
        {


            return new SizeItemInfo
            {
                // 基础开关
                Enable = limit.Enable,
                DB = limit.Enable,

                // 标准值与尺寸公差
                StDValue = size.StandardValue,
                StD_Up = size.upperLimitValue - size.StandardValue,
                StD_Down = size.lowerLimitValue - size.StandardValue,

                // 报警配置
                BJ = limit.IsBJ,
                CountNGBJ = limit.NGEACount,
                StD_BJ_Up = size.BJInfo.UpTolerance,
                StD_BJ_Down = size.BJInfo.DownTolerance
            };
        }



        List<SlotProduct> GetAllSlots(
        PFTCleanCraft cleanCraft,
        List<List<SlotProduct>> slotMatrixA,
        List<List<SlotProduct>> slotMatrixB)
        {
            var lastCam1Index = cleanCraft.ListSliceSet
                                          .Select((info, idx) => (info.BelongCam, idx))
                                          .LastOrDefault(x => x.BelongCam == CameraEnum.Cam1.ToString())
                                          .idx;

            return cleanCraft.ListSliceSet
                             .Select((_, idx) => idx <= lastCam1Index ? slotMatrixA[idx] : slotMatrixB[idx])
                             .SelectMany(slots => slots)
                             .ToList();
        }


        LineInfo GetLineInfo(
            string lineConfig,
            int shaveIndex,
            string cameraConfig,
            List<SlotProduct> allSlots,
            List<LineInfo>[] lineMatrix) // 注意：List<FrameCore.LineInfo>[]
        {
            if (string.IsNullOrWhiteSpace(lineConfig))
                return null;

            if (!Enum.TryParse(cameraConfig, true, out CameraEnum camera))
                return null;

            // 处理“第N条打孔区方向”
            const string pattern = @"^第(\d+)条打孔区(左边|右边|上边|下边)$";
            var match = Regex.Match(lineConfig, pattern);
            if (match.Success &&
                int.TryParse(match.Groups[1].Value, out var slotIndex) &&
                Enum.TryParse<DirectionEnum>(match.Groups[2].Value, out var direction))
            {
                var sliceId = slotIndex - 1;
                var shaveId = shaveIndex + 1;

                var slot = allSlots.Find(s => s.slotInfo.ShaveID == shaveId && s.slotInfo.SliceID == sliceId);
                return slot?.lineInfos[(int)direction];
            }

            // 普通线名
            var lines = camera == CameraEnum.Cam1 ? lineMatrix[0] : lineMatrix[1];
            return lines.FirstOrDefault(l => l.StrLineName == lineConfig);
        }





        /// <summary>
        /// 将尺寸报警累加至统计对象的 CountNGBJ。
        /// </summary>
        /// <param name="sizeInfo">检测结果。</param>
        /// <param name="sizeItemInfo">统计对象。</param>
        public static void AccumulateAlarmCount(SizeInfo sizeInfo, SizeItemInfo sizeItemInfo)
        {
            if (sizeInfo?.DBInfo?.IsAlarm == true) sizeItemInfo.CountNGDB++;
            if (sizeInfo?.BJInfo?.IsAlarm == true) sizeItemInfo.CountNGBJ++;
            if (sizeInfo?.TJInfo?.IsAlarm == true) sizeItemInfo.CountNGTJ++;

        }

        public void ProcessImageDefects(List<NgTypeInfo> imageDefects, CameraEnum cameraEnum, bool flag = false, int nSliceIndex = 0)
        {
            // 创建一个列表用于收集需要释放的对象
            List<NgTypeInfo> disposables = new List<NgTypeInfo>();

            DisplayWindowClass displayWindowClass;
            if (cameraEnum == CameraEnum.Cam1)
            {
                displayWindowClass = DisplayWindowClass1;
            }
            else
            {
                displayWindowClass = DisplayWindowClass4;
            }

            foreach (NgTypeInfo item in imageDefects)
            {
                // 查找对应的缺陷信息
                DefectItemInfo defectInfo = pFTCleanCraft.ListDefectInfos
                    .FirstOrDefault(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);

                if (defectInfo == null)
                {
                    // 记录未找到对应缺陷信息的错误
                    MessageNotify.AddErrorLog(
                        $"从缺项工艺ListDefectInfos中未找到对应的缺项信息{item.m_strNgTypeName}");
                    item.resultType = ResultType.NG;
                    if (!flag)
                    {
                        BIZ_General.NGImageQueueTransition_Slice(
                        cameraEnum,
                        new List<NgTypeInfo> { item },
                        item.m_nImgIndex,
                        true,
                        ref displayWindowClass.operation_Image.imageInfosShow, false, nSliceIndex);
                    }

                    continue;
                }
                else
                {
                    if (defectInfo.Enable)
                    {
                        item.resultType = ResultType.Warn;

                        // 检查缺陷面积是否超过阈值
                        if (defectInfo.EnableArea)
                        {
                            if (item.m_fNgTypArea > defectInfo.Area)
                            {
                                if (defectInfo.DB)
                                {
                                    item.bIsDB = true;
                                    item.resultType = ResultType.NG;
                                }

                                // 将缺陷信息添加到图像队列（启用报警）
                            }
                        }


                        if (defectInfo.HeightLogic)
                        {
                            if (item.m_fNgTypHeight > defectInfo.Height && item.resultType != ResultType.NG)
                            {
                                if (defectInfo.DB)
                                {
                                    item.bIsDB = true;
                                    item.resultType = ResultType.NG;
                                }



                            }
                        }


                        if (defectInfo.WidthLogic)
                        {
                            if (item.m_fNgTypWidth > defectInfo.Width && item.resultType != ResultType.NG)
                            {
                                if (defectInfo.DB)
                                {
                                    item.bIsDB = true;
                                    item.resultType = ResultType.NG;
                                }



                            }
                        }

                        // 如果结果类型为警告，将缺陷信息添加到图像队列（不启用报警）

                        if (!flag)
                        {
                            BIZ_General.NGImageQueueTransition_Slice(
                           cameraEnum,
                           new List<NgTypeInfo> { item },
                           item.m_nImgIndex,
                           item.resultType == ResultType.NG,
                           ref displayWindowClass.operation_Image.imageInfosShow, false, nSliceIndex);
                        }



                    }
                    else
                    {
                        // 如果缺陷未启用，将对象添加到 disposables 列表中
                        disposables.Add(item);

                    }
                }
            }

            // 在循环外部统一释放资源
            foreach (NgTypeInfo disposableItem in disposables)
            {
                disposableItem?.Dispose();
                MessageNotify.AddDebugLog($"未启用的缺项项目，释放缺项对象{disposableItem.m_strNgTypeName}");
            }

            // 从原始列表中移除已处理的项（可选）
            imageDefects.RemoveAll(item => disposables.Contains(item));
        }

        public void ProductDataDefectStastic(List<NgTypeInfo> deftInfos, List<DefectItemInfo> SatisticalDeftInfos)
        {
            foreach (var item in SatisticalDeftInfos)
            {
                var deftIndex = deftInfos.FindIndex(x => x.m_strNgTypeName.ToString() == item.Name && x.resultType == ResultType.NG);
                if (!(deftIndex == -1))
                {
                    item.CountNGDB++;
                }
            }
        }
        public void TJBJDeftInfo_WriteState(CameraEnum cameraEnum, List<NgTypeInfo> deftInfos, List<DefectItemInfo> TJBJDeftInfos, List<DefectItemInfo> CraftDeftInfos, int nsliceIndex)
        {
            foreach (var item in deftInfos)
            {
                var deftIndex = TJBJDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString());
                var deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString());
                if (deftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"第{nsliceIndex + 1}条缺项_BIZ_Variable.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (deftCraftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"第{nsliceIndex + 1}条缺项_pFTCleanCraft.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGBJ++;
                }
                if (TJBJDeftInfos[deftIndex].CountNGBJ > CraftDeftInfos[deftIndex].CountNGBJ && CraftDeftInfos[deftIndex].BJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + item.m_strNgTypeName.ToString());
                    TJBJDeftInfos[deftIndex].CountNGBJ = 0;
                }



                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGTJ++;
                }

                if (TJBJDeftInfos[deftIndex].CountNGTJ > CraftDeftInfos[deftIndex].CountNGTJ && CraftDeftInfos[deftIndex].TJ)
                {
                    TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + item.m_strNgTypeName.ToString());
                    TJBJDeftInfos[deftIndex].CountNGTJ = 0;
                }



            }


        }
        public void TJBJDeftInfo_WriteState(CameraEnum cameraEnum, List<NgTypeInfo> deftInfos, List<DefectItemInfo> TJBJDeftInfos, List<DefectItemInfo> CraftDeftInfos, int nsliceIndex, ResultType resultType = ResultType.NG)
        {
            foreach (var item in deftInfos)
            {
                var deftIndex = TJBJDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);
                var deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);
                if (deftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"第{nsliceIndex + 1}条缺项_BIZ_Variable.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (deftCraftIndex == -1)
                {
                    MessageNotify.AddErrorLog($"第{nsliceIndex + 1}条缺项_pFTCleanCraft.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                    continue;
                }

                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGDB++;
                }


                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGBJ++;
                }




                if (item.resultType == ResultType.NG)
                {
                    TJBJDeftInfos[deftIndex].CountNGTJ++;
                }



                for (int i = 0; i < TJBJDeftInfos.Count; i++)
                {
                    deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == cameraEnum);
                    if (deftCraftIndex == -1)
                    {
                        MessageNotify.AddErrorLog($"第{nsliceIndex + 1}条缺项_BIZ_Variable.ListDefectInfos_结果{item.m_strNgTypeName.ToString()}未找到对应的缺项信息");
                        continue;
                    }
                    if (TJBJDeftInfos[i].CountNGBJ > CraftDeftInfos[i].CountNGBJ && CraftDeftInfos[deftCraftIndex].BJ)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + item.m_strNgTypeName.ToString() + $"_当前个数：{TJBJDeftInfos[i].CountNGBJ}> 设定个数{CraftDeftInfos[deftCraftIndex].CountNGBJ}");

                    }
                    if (TJBJDeftInfos[i].CountNGTJ > CraftDeftInfos[deftCraftIndex].CountNGTJ && CraftDeftInfos[deftCraftIndex].TJ)
                    {
                        TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + item.m_strNgTypeName.ToString() + $"_当前个数：{TJBJDeftInfos[i].CountNGTJ}> 设定个数{CraftDeftInfos[deftCraftIndex].CountNGTJ}");
                    }
                    if (TJBJDeftInfos[i].CountNGDB > CraftDeftInfos[deftCraftIndex].CountNGDB && CraftDeftInfos[deftCraftIndex].DB)
                    {
                        resultType = ResultType.NG;
                        //记录
                        MessageNotify.AddDebugLog($"当前缺陷项目_{TJBJDeftInfos[i].Description}达到DB设定个数条件,当前个数：{TJBJDeftInfos[i].CountNGDB}>设定个数{CraftDeftInfos[deftCraftIndex].CountNGDB}");
                    }


                }

            }


        }



        public void TJBJSize_WriteState(CameraEnum cameraEnum, SizeInfo sizeInfo, SizeItemInfo TJBJSizeInfos, SizeItemInfo CraftSizeInfo, int nsliceIndex)
        {
            //if (sizeInfo.BJInfo.IsAlarm)
            //{
            //    TJBJSizeInfos.CountNGBJ++;
            //}

            if (TJBJSizeInfos.CountNGBJ > CraftSizeInfo.CountNGBJ && CraftSizeInfo.BJ)
            {
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description, describe = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description });
                //TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description);
                TJBJSizeInfos.CountNGBJ = 0;
            }



            //if (sizeInfo.TJInfo.IsAlarm)
            //{
            //    TJBJSizeInfos.CountNGTJ++;
            //}

            if (TJBJSizeInfos.CountNGTJ > CraftSizeInfo.CountNGTJ && CraftSizeInfo.TJ)
            {
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description, describe = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description });
                // TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description);
                TJBJSizeInfos.CountNGTJ = 0;
            }

        }


        /// <summary>
        /// 切换更新曲线缓存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CobSelectChange(object sender, EventArgs e)
        {
            ExecuteMethod(() =>
            {
                int sliceIndex = form.GetCob_SliceIndex();
                if (sliceIndex < 0)
                {
                    return;
                }
                int grooveIndex = form.GetCob_GrooveIndex();
                if (grooveIndex < 0)
                {
                    return;
                }

                var grooveSizeEnum = Enum.GetNames(typeof(MeasureSlotEnum));
                InitGrooveChart(true);

                UIService.Instance.AddAction(() =>
                {
                    for (int i = 0; i < grooveSizeEnum.Length; i++)
                    {

                        ChartGroovePage[i].UpData3DCache(sliceIndex, grooveIndex, i);

                    }
                });

            });

        }


        /// <summary>
        /// 尺寸NG判断
        /// </summary>
        /// <param name="sizeItemInfo"></param>
        /// <param name="sizeInfo"></param>
        public void GetSizeJudgeInfo(SizeItemInfo sizeItemInfo, ref SizeInfo sizeInfo)
        {
            // 初始化结果类型和报警状态
            sizeInfo.IsNG = true;
            sizeInfo.resultType = ResultType.Warn;
            sizeInfo.Index = sizeItemInfo.Index;
            // 检查是否在标准范围内
            bool isInStandardRange = sizeInfo.DataResult.Between(sizeItemInfo.StDValue + sizeItemInfo.StD_Down, sizeItemInfo.StDValue + sizeItemInfo.StD_Up);

            if (isInStandardRange)
            {
                sizeInfo.resultType = ResultType.OK;
                sizeInfo.IsNG = false;
            }
            else
            {
                // 设置报警信息
                if (sizeItemInfo.DB)
                {
                    SetAlarmInfo(sizeItemInfo, sizeInfo);
                }
            }

            // 检查停机条件并设置相关信息
            if (sizeItemInfo.TJ)
            {
                CheckTJCondition(sizeItemInfo, sizeInfo);
            }

            // 检查报警条件并设置相关信息
            if (sizeItemInfo.BJ)
            {
                CheckBJCondition(sizeItemInfo, sizeInfo);
            }
        }


        /// <summary>
        /// 将原始图像转换为显示图像
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="result"></param>
        /// <param name="frame"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <param name="operation_ImageInfo"></param>
        private void PrimitiveTransition(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                string imageSavePath = in_imageSaveParamer.ImageSave_Path_Primitive;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Primitive;
                ImageDeepthSaveFormat imageDeepthSaveFormat = (ImageDeepthSaveFormat)in_imageSaveParamer.SaveImageFormat_Deepth;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Primitive;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool IsCompression_Deepth = in_imageSaveParamer.IsCompression_Deepth;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_Screenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_Screenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_Screenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_Screenshot;
                string imageSaveName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString();
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                IsCompression_Deepth, HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, imageDeepthSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }


        /// <summary>
        /// 将原始图像转换为显示图像
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="showHalconWindow"></param>
        /// <param name="result"></param>
        /// <param name="frame"></param>
        /// <param name="PackgeNum"></param>
        /// <param name="EAIndex"></param>
        /// <param name="operation_ImageInfo"></param>
        private void PrimitiveTransitionEA(CameraEnum cameraEnum, ShowHalconWindow showHalconWindow, out Operation_ImageInfo operation_ImageInfo)
        {
            operation_ImageInfo = null;
            try
            {
                ImageSaveParamer in_imageSaveParamer = Project.Instance.cameraManager[cameraEnum].m_ImageSaveParamer;
                string imageSavePath = in_imageSaveParamer.ImageSave_Path_ProdImg;
                ImageSaveFormat saveImageFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdImg;
                ImageSaveSelect imageSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdImg;
                bool isShowImage = in_imageSaveParamer.IsShowImage_Primitive;
                bool HighDefinition_Screenshot = in_imageSaveParamer.HighDefinition_ProdScreenshot;
                ImageSaveSelect screenshotSaveSelect = (ImageSaveSelect)in_imageSaveParamer.SaveImageSelect_ProdScreenshot;
                string screenshotSavePath = in_imageSaveParamer.ImageSave_Path_ProdScreenshot;
                ImageSaveFormat screenshotSaveFormat = (ImageSaveFormat)in_imageSaveParamer.SaveImageFormat_ProdScreenshot;
                string imageSaveName = showHalconWindow.m_nImageFrameID.ToString().PadLeft(5, '0') + "_" + (showHalconWindow.m_nProductID).ToString();
                operation_ImageInfo = BIZ_General.ImageDataTypeTransition(showHalconWindow, Project.Instance.paramPathSet.ProductMojuanPath, imageSavePath, imageSaveName, saveImageFormat, imageSaveSelect, isShowImage,
                   HighDefinition_Screenshot, screenshotSaveSelect, Project.Instance.paramPathSet.ProductMojuanPath, screenshotSavePath, imageSaveName, screenshotSaveFormat, cameraEnum.ToString());
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }



        private void SetAlarmInfo(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            sizeInfo.DBInfo.IsAlarm = true;
            sizeInfo.DBInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_Up;
            sizeInfo.DBInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_Down;
            if (!sizeInfo.DataResult.Between(sizeInfo.DBInfo.DownTolerance, sizeInfo.DBInfo.UpTolerance))
            {
                sizeInfo.resultType = ResultType.NG;
            }
        }



        private void CheckBJCondition(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            bool isInBJRange = sizeInfo.DataResult >= (sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Down) &&
                               sizeInfo.DataResult <= (sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Up);

            if (!isInBJRange)
            {
                sizeInfo.BJInfo.IsAlarm = true;
                sizeInfo.BJInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Up;
                sizeInfo.BJInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_BJ_Down;
            }
        }



        private void CheckTJCondition(SizeItemInfo sizeItemInfo, SizeInfo sizeInfo)
        {
            bool isInTJRange = sizeInfo.DataResult >= (sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Down) &&
                               sizeInfo.DataResult <= (sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Up);

            if (!isInTJRange)
            {
                sizeInfo.TJInfo.IsAlarm = true;
                sizeInfo.TJInfo.UpTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Up;
                sizeInfo.TJInfo.DownTolerance = sizeItemInfo.StDValue + sizeItemInfo.StD_TJ_Down;
            }
        }


        /// <summary>
        /// 统计初始化
        /// </summary>
        /// <param name="statisticalData"></param>
        /// <param name="pFTCleanCraft"></param>
        /// <returns></returns>
        public List<StatisticalData_Slice> InitializeStatisticalData(
         List<StatisticalData_Slice> statisticalData,
             PFTCleanCraft pFTCleanCraft)
        {
            // 清空并重新初始化统计列表
            statisticalData?.Clear();
            statisticalData = new List<StatisticalData_Slice>();

            // 获取测量槽位的枚举名称（优化性能）
            var slotNames = Enum.GetNames(typeof(MeasureSlotEnum));

            for (int sliceIndex = 0; sliceIndex < pFTCleanCraft.SliceCount; sliceIndex++)
            {
                StatisticalData_Slice slice = new StatisticalData_Slice();
                for (int GroveIndex = 0; GroveIndex < pFTCleanCraft.ListSliceSet?.Select(d => d.ShaveNum).ToArray()[sliceIndex]; GroveIndex++)
                {
                    // 1. 添加预定义的测量槽位尺寸
                    for (int slotIndex = 0; slotIndex < slotNames.Length; slotIndex++)
                    {
                        slice.sizeItemInfos.Add(new SizeItemInfo
                        {
                            Name = slotNames[slotIndex],
                            Description = $"第{sliceIndex + 1}条第{GroveIndex + 1}个槽位{slotNames[slotIndex]}",
                            CountNGBJ = 0,
                            CountNGDB = 0,
                            CountNGTJ = 0
                        }); ;
                    }

                }



                // 2. 添加启用的EA尺寸项
                foreach (var eaSize in pFTCleanCraft.ListEASizes)
                {
                    if (eaSize.ArrayBelongSlice[sliceIndex] == 1 && eaSize.Enable)
                    {
                        var clonedSize = eaSize.DeepClone();
                        clonedSize.CountNGBJ = 0;
                        clonedSize.CountNGDB = 0;
                        clonedSize.CountNGTJ = 0;
                        slice.sizeItemInfos.Add(clonedSize);
                    }
                }

                // 3. 添加启用的缺陷项（修正了原始代码中的索引错误）
                for (int defectIndex = 0; defectIndex < pFTCleanCraft.ListDefectInfos.Count / 2; defectIndex++)
                {
                    if (pFTCleanCraft.ListDefectInfos[defectIndex].Enable)
                    {
                        var clonedDefect = pFTCleanCraft.ListDefectInfos[defectIndex].DeepClone();
                        clonedDefect.CountNGBJ = 0;
                        clonedDefect.CountNGDB = 0;
                        clonedDefect.CountNGTJ = 0;
                        slice.defectItemInfos.Add(clonedDefect);
                    }
                }
                slice.sizeTJDBBJItemInfos = slice.sizeItemInfos.DeepClone();
                slice.defectTJDBBJItemInfos = slice.defectItemInfos.DeepClone();
                statisticalData.Add(slice);
            }

            return statisticalData;
        }

        /// <summary>
        /// 统计更新
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="newData"></param>
        /// <param name="pFTCleanCraft"></param>
        /// <returns></returns>
        public List<StatisticalData_Slice> UpdateStatisticalData(
        List<StatisticalData_Slice> currentData, List<StatisticalData_Slice> newData,
        PFTCleanCraft pFTCleanCraft)
        {

            // 如果没有现有数据，直接返回新结构
            if (currentData == null || currentData.Count == 0)
                return newData;

            // 遍历所有切片
            for (int sliceIndex = 0; sliceIndex < newData.Count; sliceIndex++)
            {
                var newSlice = newData[sliceIndex];
                newData[sliceIndex].statisticalData = currentData[sliceIndex].statisticalData;

                // 如果当前数据有对应切片，则合并数据
                if (sliceIndex < currentData.Count)
                {
                    var currentSlice = currentData[sliceIndex];

                    // 合并尺寸项数据
                    MergeSizeItems(newSlice.sizeItemInfos, currentSlice.sizeItemInfos);

                    // 合并缺陷项数据
                    MergeDefectItems(newSlice.defectItemInfos, currentSlice.defectItemInfos);
                }
            }

            return newData;
        }

        private void MergeSizeItems(
          List<SizeItemInfo> newItems,
         List<SizeItemInfo> currentItems)
        {
            // 创建查找字典（按名称和描述）
            var currentDict = currentItems
                .GroupBy(x => (x.Name, x.Description))
                .ToDictionary(g => g.Key, g => g.First());

            // 更新新列表中的计数
            foreach (var newItem in newItems)
            {
                var key = (newItem.Name, newItem.Description);
                if (currentDict.TryGetValue(key, out var currentItem))
                {
                    // 保留现有计数
                    newItem.CountNGBJ = currentItem.CountNGBJ;
                    newItem.CountNGDB = currentItem.CountNGDB;
                    newItem.CountNGTJ = currentItem.CountNGTJ;
                }
                // 否则保持初始化的0计数
            }

            //// 添加当前有但新列表没有的项（按需保留）
            //foreach (var currentItem in currentItems)
            //{
            //    var key = (currentItem.Name, currentItem.Description);
            //    if (!newItems.Any(x =>
            //        x.Name == currentItem.Name &&
            //        x.Description == currentItem.Description))
            //    {
            //        // 添加缺失项（保留计数）
            //        newItems.Add(currentItem.DeepClone());
            //    }
            //}
        }

        private void MergeDefectItems(
            List<DefectItemInfo> newItems,
            List<DefectItemInfo> currentItems)
        {
            // 创建查找字典（按名称）
            var currentDict = currentItems
                .GroupBy(x => x.Name)
                .ToDictionary(g => g.Key, g => g.First());

            // 更新新列表中的计数
            foreach (var newItem in newItems)
            {
                if (currentDict.TryGetValue(newItem.Name, out var currentItem))
                {
                    // 保留现有计数
                    newItem.CountNGBJ = currentItem.CountNGBJ;
                    newItem.CountNGDB = currentItem.CountNGDB;
                    newItem.CountNGTJ = currentItem.CountNGTJ;
                }
                // 否则保持初始化的0计数
            }

            //// 添加当前有但新列表没有的项（按需保留）
            //foreach (var currentItem in currentItems)
            //{
            //    if (!newItems.Any(x => x.Name == currentItem.Name))
            //    {
            //        // 添加缺失项（保留计数）
            //        newItems.Add(currentItem.DeepClone());
            //    }
            //}
        }

        #endregion

        #region 方法模板
        /// <summary>
        /// 方法调用模板
        /// </summary>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        public void ExecuteMethod(Action action, [CallerMemberName] string methodName = "")
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
            try
            {
                // 执行传入的委托
                action();
            }
            catch (Exception ee)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ee.ToString()}";
                // 输出带调用堆栈的详细错误信息
                MessageNotify.AddErrorLog(fullErrorMessage);
            }
        }


        // 1. 无参返回 bool 的委托，用于统一异常包装
        public delegate bool TryWorkDelegate();

        /// <summary>
        /// 统一执行带返回值的业务逻辑，并捕获异常记录日志。
        /// </summary>
        /// <param name="work">无参且返回 bool 的业务委托。</param>
        /// <param name="methodName">调用方的方法名（自动填充）。</param>
        /// <returns>业务执行是否成功；发生异常时返回 false。</returns>
        public bool ExecuteMethod(TryWorkDelegate work,
                                  [CallerMemberName] string methodName = "")
        {
            var className = GetType().FullName ?? "UnknownClass";
            try
            {
                return work();          // 真正执行业务逻辑
            }
            catch (Exception ex)
            {
                var msg = $"[{className}.{methodName}] 发生异常：{ex}";
                MessageNotify.AddErrorLog(msg);
                return false;           // 发生异常视为失败
            }
        }
        #endregion
    }
}
