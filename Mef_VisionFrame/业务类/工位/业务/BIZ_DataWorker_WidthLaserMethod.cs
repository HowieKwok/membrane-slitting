using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

using NPOI.SS.Formula.Functions;

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
                                              PFTCleanEAProductInfo pFTCleanEAProductInfo,
                                            int CamID = 6)
        {

            HOperatorSet.CountSeconds(out HTuple StartSendDB);
            var zones = ((SlaveMachineParam)Project.Instance.slaveMachineManager.tcpClientParam)?.Zones;




            // 1. 默认全部 0000
            var dbTypes = Enumerable.Range(0, 12)
                                    .Select(i => (i, "0000"))
                                    .ToArray();


            int lastCam1Index = BIZ_Variable.nSpliteIndex;

            // 2. 标记 NG 通道
            for (int i = 0; i < pFTCleanEAProductInfo.ListEASlicePD.Count; i++)
            {
                var channels = zones[i].Channels;
                if (channels == null) continue;

                bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= lastCam1Index
                                                                : i > lastCam1Index;

                if (needShow)
                {
                    foreach (var ch in channels)
                    {
                        int index = ch.Id - 1;
                        if (index >= 0 && index < dbTypes.Length &&
                         pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceAllResult == ResultType.NG)
                        {
                            dbTypes[index] = (index, (ch.Id).ToString().PadLeft(4, '0'));
                        }
                    }
                }

            }


            if (!Project.Instance.slaveMachineManager.initState || keyA <= 0)
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
                    MessageNotify.AddErrorLog($"与主机软件未连接（{Project.Instance.slaveMachineManager.initState}）,暂不发送打标，当前EA为{keyA}&&{string.Join(",", dbTypes.Select(t => t.Item2))}");
                }

                return;
            }



            // 3. 异步发送
            bIZ_ThreadManage.limitThreadPool.RunAction(() =>
            {
                Project.Instance.slaveMachineManager.SendDBInfo(FrameID, dbTypes, CamID, out string msg);

            });

            HOperatorSet.CountSeconds(out HTuple EndSendDB);
            EndSendDB = (EndSendDB - StartSendDB) * 1000;
            MessageNotify.AddDebugLog($"第{keyA}EA发送DB信息处理时长::{EndSendDB.D}");

        }


        /// <summary>
        /// 数据导出功能
        /// </summary>
        public void DataOutput()
        {
            ExecuteMethod(() =>
            {
                var items = new List<(int, int, int)> { };

                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlices.Count; i++)
                {
                    var data = (i + 1, (int)BIZ_Variable.StatisticalData_EASlices[i].statisticalData.OKCount, (int)BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount);
                    items.Add(data);
                }
                byte[] pdf = PdfReportHelper.BuildPdf(
                DateTime.Now,
                "Default",
                Project.Instance.systemInfo.currentMojuanhao,
                pFTCleanCraft.SliceCount,
                (int)BIZ_Variable.StatisticalData_EASlices[0].statisticalData.SumupCount,
                Project.Instance.userManage.userData.CurrentUser.ToString(),
                Project.Instance.userManage.userData.CurrentUserID.ToString(),
                items);

                System.IO.File.WriteAllBytes(Project.Instance.paramPathSet.ProductMojuanPath + "生产实绩报表.pdf", pdf);

                MessageNotify.AddDebugLog("生产实绩报表.pdf--导出完成");

            });
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
            if (productInfoA.ListEASlicePD.Count() != pFTCleanCraft.SliceCount ||
               productInfoB.ListEASlicePD.Count() != pFTCleanCraft.SliceCount)
            {
                // 报警：切片数量不一致
                MessageNotify.AddErrorLog("ProcessingData_FrameCam2()_切片数量不一致");
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "切片数量不一致", describe = "切片数量不一致" });
            }
            else
            {
                #region 计算EA结果


                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                    if (needShow)
                    {
                        productInfoA.ListEASlicePD[i].resultSliceConfigSizeNg = ResultType.NG;
                        productInfoA.ListEASlicePD[i].resultSliceDefectNg = ResultType.NG;
                        productInfoA.ListEASlicePD[i].resultSliceGrooveSizeNg = ResultType.NG;
                        productInfoA.ListEASlicePD[i].resultSliceAllResult = ResultType.NG;

                        productInfoB.ListEASlicePD[i].resultSliceConfigSizeNg = ResultType.NG;
                        productInfoB.ListEASlicePD[i].resultSliceDefectNg = ResultType.NG;
                        productInfoB.ListEASlicePD[i].resultSliceGrooveSizeNg = ResultType.NG;
                        productInfoB.ListEASlicePD[i].resultSliceAllResult = ResultType.NG;
                    }
                }




                // 计算槽位TD&MD尺寸
                MeasureDataDistance(pFTCleanCraft, productInfoA.ListEASlicePD, productInfoB.ListEASlicePD, Calipix_x, Calipix_y, out bool bIsSuccess);


                //槽位尺寸判断
                GrooveSliceDataJudge(pFTCleanCraft, productInfoA.ListEASlicePD, CameraEnum.Cam1);
                GrooveSliceDataJudge(pFTCleanCraft, productInfoB.ListEASlicePD, CameraEnum.Cam2);

                //计算配置尺寸项目
                MeasureNewConfigSize(pFTCleanCraft, productInfoA, productInfoB);


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
                List<SlotProduct> slotProducts = GetAllSlots(pFTCleanCraft, productInfoA.ListEASlicePD, productInfoB.ListEASlicePD);
                List<List<NgTypeInfo>> ListEADefects = new List<List<NgTypeInfo>>();
                //获取分界值
                // 1) 分界索引
                int lastCam1Index = pFTCleanCraft.ListSliceSet
                                        .Select((info, idx) => (info.IsLeftSide, idx))
                                        .LastOrDefault(x => x.IsLeftSide)
                                        .idx;

                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {


                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= lastCam1Index
                                                                : i > lastCam1Index;


                    if (!needShow)
                    {
                        continue;
                    }

                    #region 缺陷方面判断

                    HOperatorSet.CountSeconds(out HTuple startDeftTime);

                    EADetectJudgeDeal(BIZ_Variable.StatisticalData_EASlicesA[i], productInfoA.ListEASlicePD[i], CameraEnum.Cam1, i);
                    EADetectJudgeDeal(BIZ_Variable.StatisticalData_EASlicesB[i], productInfoB.ListEASlicePD[i], CameraEnum.Cam2, i);

                    #endregion

                    #region 尺寸方面判断
                    EASizeJudgeDeal(pFTCleanCraft, BIZ_Variable.StatisticalData_EASlicesA[i], productInfoA, CameraEnum.Cam1, i);
                    EASizeJudgeDeal(pFTCleanCraft, BIZ_Variable.StatisticalData_EASlicesB[i], productInfoB, CameraEnum.Cam2, i);

                    #endregion

                    productInfoA.ListEASlicePD[i].resultSliceAllResult = new ResultType[] { productInfoA.ListEASlicePD[i].resultSliceToltalResult, productInfoB.ListEASlicePD[i].resultSliceToltalResult }.Max();
                    productInfoB.ListEASlicePD[i].resultSliceAllResult = new ResultType[] { productInfoA.ListEASlicePD[i].resultSliceToltalResult, productInfoB.ListEASlicePD[i].resultSliceToltalResult }.Max();

                    if (keyA > 0)
                    {
                        if (productInfoA.ListEASlicePD[i].resultSliceAllResult == ResultType.NG)
                        {
                            BIZ_Variable.StatisticalData_EASlices[i].statisticalData.NGCount++;

                        }

                        StatisticalData statisticalData = BIZ_Variable.StatisticalData_EASlices[i].statisticalData;
                        BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);


                        if (productInfoA.ListEASlicePD[i].resultSliceToltalResult == ResultType.NG)
                        {
                            BIZ_Variable.StatisticalData_EASlicesA[i].statisticalData.NGCount++;
                        }

                        statisticalData = BIZ_Variable.StatisticalData_EASlicesA[i].statisticalData;
                        BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);

                        if (productInfoB.ListEASlicePD[i].resultSliceToltalResult == ResultType.NG)
                        {
                            BIZ_Variable.StatisticalData_EASlicesB[i].statisticalData.NGCount++;
                        }
                        statisticalData = BIZ_Variable.StatisticalData_EASlicesB[i].statisticalData;
                        BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);

                    }


                }


                var zones = ((SlaveMachineParam)Project.Instance.slaveMachineManager.tcpClientParam)?.Zones;


                if (zones == null || zones?.Count != 12)
                {
                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = "打标机未正确设置条数", describe = "打标机未正确设置条数" });

                }
                else
                {
                    DBChannel = Enumerable.Range(0, pFTCleanCraft.SliceCount)
                     .Select(i =>
                         zones != null && i < zones.Count && zones[i].Channels != null && zones[i].Channels.Count > 0
                             ? string.Join(",", zones[i].Channels.Select(c => c.Id.ToString().PadLeft(4, '0')))
                             : "NULL")
                     .ToList();

                    OutDBChannel = DBChannel;
                }

                productInfoA.EAFinfished = true;
                productInfoB.EAFinfished = true;


                #endregion
            }
        }


        private void UIAndFileDataSave(int keyA, List<string> DBChannel, PFTCleanEAProductInfo productInfoA, PFTCleanEAProductInfo productInfoB)
        {
            HOperatorSet.CountSeconds(out HTuple StartUIAndFileDataSave);
            //配置尺寸数据保存及显示
            List<RowData> rowConfigDatas = new List<RowData>();
            List<RowData> rowDeftDatas = new List<RowData>();
            List<RowData> rowAllDatas = new List<RowData>();


            List<PFTCleanEAProductInfo> productInfos = new List<PFTCleanEAProductInfo>() { productInfoA, productInfoB };

            for (int i = 0; i < productInfos.Count; i++)
            {
                rowConfigDatas?.Clear();
                rowConfigDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowConfigDatas.Add(new RowData() { dataStr = $"{i}", resultType = ResultType.OK });
                rowConfigDatas.Add(new RowData() { dataStr = $"{productInfoA.EAID}", resultType = ResultType.OK });

                foreach (var item in productInfos[i].ListEASizes)
                {
                    SizeItemInfo sizeItemInfo = pFTCleanCraft.ListEASizes.Find(x => x.Index == item.Index);
                    if (sizeItemInfo == null)
                    {
                        MessageNotify.AddErrorLog($"配置尺寸数据保存异常，找不到对应配置尺寸数据:{item.Name}");
                        continue;
                    }
                    rowConfigDatas.Add(new RowData() { dataStr = item.DataResult.ToString(), resultType = item.resultType });
                    string str = string.Join("_", sizeItemInfo.ArrayBelongSlice
                                  .Select((value, index) => new { value, index })
                                  .Where(x => x.value > 0)
                                 .Select(x => x.index)
                                 .ToArray());
                    rowConfigDatas.Add(new RowData() { dataStr = item.resultType.ToString(), resultType = item.resultType });
                    rowConfigDatas.Add(new RowData() { dataStr = str, resultType = item.resultType });
                    ChartDataShow(ChartConfigPage[item.Index], item.DataResult);

                }
                if (productInfos[i].ListEASizes.Count > 0)
                {
                    NDataPanelViewModel.Add(form.DpModelSizeConfig, BIZ_Variable.strEASizeConfigTitle.ToArray(), rowConfigDatas.ToArray());
                    FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizeConfigPath, BIZ_Variable.strEASizeConfigTitle.ToArray(), rowConfigDatas.ToArray(), "");

                }

            }


            //槽位尺寸数据保存及显示


            List<RowData> rowSingle = new List<RowData>();

            List<RowData> rowAllGroove = new List<RowData>();


            for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
            {
                bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                for (int n = 0; n < productInfos.Count; n++)
                {

                    rowAllGroove?.Clear();
                    rowAllGroove = new List<RowData>();
                    rowAllGroove.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                    rowAllGroove.Add(new RowData() { dataStr = $"{n}", resultType = productInfos[n].ListEASlicePD[i].resultSliceSizeNg });
                    rowAllGroove.Add(new RowData() { dataStr = $"{productInfos[n].EAID}", resultType = productInfos[n].ListEASlicePD[i].resultSliceSizeNg });
                    rowAllGroove.Add(new RowData() { dataStr = $"{i + 1}", resultType = productInfos[n].ListEASlicePD[i].resultSliceSizeNg });

                    for (int j = 1; j <= pFTCleanCraft.PerSlotCount; j++)
                    {
                        //槽位尺寸单独显示表
                        var strSlotName = Enum.GetNames(typeof(MeasureSlotEnum));
                        rowSingle.Clear();
                        rowSingle.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                        rowSingle.Add(new RowData() { dataStr = $"{n}", resultType = ResultType.OK });
                        rowSingle.Add(new RowData() { dataStr = $"{productInfos[n].EAID}", resultType = ResultType.OK });
                        rowSingle.Add(new RowData() { dataStr = $"{i + 1}", resultType = ResultType.OK });
                        rowSingle.Add(new RowData() { dataStr = j.ToString(), resultType = ResultType.OK });
                        for (int k = 0; k < pFTCleanCraft.ListSlotSizeLimits.Count; k++)
                        {
                            if (needShow)
                            {
                                if (pFTCleanCraft.ListSlotSizeLimits[k].Enable)
                                {
                                    SlotProduct slotProduct = productInfos[n].ListEASlicePD[i].listShave.Find(s => s.slotInfo.SliceID == i && s.slotInfo.ShaveID == j);

                                    if (slotProduct == null)
                                    {
                                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{i + 1}片第{j}个槽位尺寸数据缺失", describe = $"第{i + 1}片第{j + 1}个槽位尺寸数据缺失" });
                                        continue;
                                    }
                                    SizeInfo sizeInfo = slotProduct.SlotSizes[k];
                                    rowAllGroove.Add(new RowData() { dataStr = sizeInfo.DataResult.ToString(), resultType = sizeInfo.resultType });
                                    rowAllGroove.Add(new RowData() { dataStr = sizeInfo.resultType.ToString(), resultType = sizeInfo.resultType });
                                    rowSingle.Add(new RowData() { dataStr = sizeInfo.DataResult.ToString(), resultType = sizeInfo.resultType });
                                    rowSingle.Add(new RowData() { dataStr = sizeInfo.resultType.ToString(), resultType = sizeInfo.resultType });
                                    if (n == 0)
                                    {
                                        // ChartDataGrooveShow(i, j - 1, k, sizeInfo.DataResult);
                                        SlotProduct slotProduct0 = productInfos[0].ListEASlicePD[i].listShave.Find(s => s.slotInfo.SliceID == i && s.slotInfo.ShaveID == j);
                                        SlotProduct slotProduct1 = productInfos[1].ListEASlicePD[i].listShave.Find(s => s.slotInfo.SliceID == i && s.slotInfo.ShaveID == j);
                                        SizeInfo sizeInfo0 = slotProduct.SlotSizes[k];
                                        SizeInfo sizeInfo1 = slotProduct.SlotSizes[k];
                                        List<double> doubles = new List<double>();
                                        doubles.Add(sizeInfo0.DataResult);
                                        doubles.Add(sizeInfo1.DataResult);
                                        ChartDataGrooveShow(i, j - 1, k, doubles.ToArray());

                                    }

                                }

                            }

                        }
                        if (needShow)
                        {
                            int index = i;

                            if (pFTCleanCraft.CurCamIsLeft)
                            {
                                index = i;
                                if (n == 1)
                                {
                                    index = (BIZ_Variable.nSpliteIndex + 1) + i;
                                }
                            }
                            else
                            {
                                index = i - (BIZ_Variable.nSpliteIndex + 1);
                                if (n == 1)
                                {
                                    index = i;
                                }
                            }


                            NDataPanelViewModel.Add(form.DpModelEASizeDataPanels[index], BIZ_Variable.strEASizeSliceTitle.ToArray(), rowSingle.ToArray());

                        }

                    }
                    rowAllGroove.Add(new RowData() { dataStr = productInfos[n].ListEASlicePD[i].resultSliceGrooveSizeNg.ToString(), resultType = productInfos[n].ListEASlicePD[i].resultSliceGrooveSizeNg });
                    rowAllGroove.Add(new RowData() { dataStr = productInfos[n].ListEASlicePD[i].resultSliceConfigSizeNg.ToString(), resultType = productInfos[n].ListEASlicePD[i].resultSliceConfigSizeNg });
                    rowAllGroove.Add(new RowData() { dataStr = productInfos[n].ListEASlicePD[i].resultSliceToltalResult.ToString(), resultType = productInfos[n].ListEASlicePD[i].resultSliceToltalResult });



                    // GenerateSlotSizeTable(productInfoA, productInfoB);
                    if (needShow)
                    {
                        FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizePath, BIZ_Variable.strEASizeTitle.ToArray(), rowAllGroove.ToArray(), "");

                    }

                }



            }


            for (int n = 0; n < productInfos.Count; n++)
            {
                rowDeftDatas?.Clear();
                rowDeftDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                rowDeftDatas.Add(new RowData() { dataStr = $"{n}", resultType = ResultType.OK });
                rowDeftDatas.Add(new RowData() { dataStr = $"{productInfos[n].EAID}", resultType = ResultType.OK });



                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    rowDeftDatas.Add(new RowData() { dataStr = $"{productInfos[n].ListEASlicePD[i].resultSliceDefectNg}", resultType = productInfos[n].ListEASlicePD[i].resultSliceDefectNg });

                }
                NDataPanelViewModel.Add(form.DpModelEADetect, BIZ_Variable.strEADeftTitle.ToArray(), rowDeftDatas.ToArray());
                FileOperationCSV.WriteData_CSV(BIZ_Variable.strEADeftPath, BIZ_Variable.strEADeftTitle.ToArray(), rowDeftDatas.ToArray(), "");

            }

            for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
            {

                ResultType resultSliceDefectNg = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceDefectNg, productInfos[1].ListEASlicePD[i].resultSliceDefectNg }.Max();

                ResultType resultSliceSizeNg = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceSizeNg, productInfos[1].ListEASlicePD[i].resultSliceSizeNg }.Max();

                ResultType resultSliceConfigSizeNg = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceConfigSizeNg, productInfos[1].ListEASlicePD[i].resultSliceConfigSizeNg }.Max();

                ResultType resultSliceGrooveSizeNg = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceGrooveSizeNg, productInfos[1].ListEASlicePD[i].resultSliceGrooveSizeNg }.Max();

                /// <summary>
                /// 这里这是用于AB合并的结果
                /// </summary>
                ResultType resultSliceAllResult = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceAllResult, productInfos[1].ListEASlicePD[i].resultSliceAllResult }.Max();


                ResultType resultSliceToltalResult = new ResultType[] { productInfos[0].ListEASlicePD[i].resultSliceToltalResult, productInfos[1].ListEASlicePD[i].resultSliceToltalResult }.Max();


                rowAllDatas?.Clear();
                rowAllDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });

                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[0].EAID}", resultType = ResultType.OK });
                rowAllDatas.Add(new RowData() { dataStr = $"{i + 1}", resultType = resultSliceAllResult });


                //
                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[0].ListEASlicePD[i].resultSliceGrooveSizeNg}", resultType = productInfos[0].ListEASlicePD[i].resultSliceGrooveSizeNg });

                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[1].ListEASlicePD[i].resultSliceGrooveSizeNg}", resultType = productInfos[1].ListEASlicePD[i].resultSliceGrooveSizeNg });

                rowAllDatas.Add(new RowData() { dataStr = $"{resultSliceGrooveSizeNg}", resultType = resultSliceGrooveSizeNg });
                //

                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[0].ListEASlicePD[i].resultSliceConfigSizeNg}", resultType = productInfos[0].ListEASlicePD[i].resultSliceConfigSizeNg });
                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[1].ListEASlicePD[i].resultSliceConfigSizeNg}", resultType = productInfos[1].ListEASlicePD[i].resultSliceConfigSizeNg });
                rowAllDatas.Add(new RowData() { dataStr = $"{resultSliceConfigSizeNg}", resultType = resultSliceConfigSizeNg });


                rowAllDatas.Add(new RowData() { dataStr = $"{resultSliceSizeNg}", resultType = resultSliceSizeNg });

                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[0].ListEASlicePD[i].resultSliceDefectNg}", resultType = productInfos[0].ListEASlicePD[i].resultSliceDefectNg });
                rowAllDatas.Add(new RowData() { dataStr = $"{productInfos[1].ListEASlicePD[i].resultSliceDefectNg}", resultType = productInfos[1].ListEASlicePD[i].resultSliceDefectNg });
                rowAllDatas.Add(new RowData() { dataStr = $"{resultSliceDefectNg}", resultType = resultSliceDefectNg });

                rowAllDatas.Add(new RowData() { dataStr = $"{resultSliceAllResult}", resultType = resultSliceAllResult });
                rowAllDatas.Add(new RowData() { dataStr = DBChannel[i], resultType = resultSliceAllResult });
                bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                if (needShow)
                {
                    NDataPanelViewModel.Add(form.DpModelEADetectSize, BIZ_Variable.strEASizeDeftTitle.ToArray(), rowAllDatas.ToArray());
                    FileOperationCSV.WriteData_CSV(BIZ_Variable.strEASizeDeftPath, BIZ_Variable.strEASizeDeftTitle.ToArray(), rowAllDatas.ToArray(), "");

                }

            }




            //开始统计数据
            UIService.Instance.AddAction(() =>
             {
                 for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                 {
                     StatisticalData statisticalData = BIZ_Variable.StatisticalData_EASlices[i].statisticalData;
                     BIZ_General.ProdStatisticsCalculate(statisticalData, keyA, statisticalData.NGCount);
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[0], statisticalData.SumupCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[1], $"{statisticalData.Yield.ToString("0.00")}%");
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[2], statisticalData.OKCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[3], statisticalData.NGCount.ToString());
                     NDataGridView.SetCellValue(form.Dgv_StatsEA, BIZ_Variable.DgvColumnheaders[i + 1], BIZ_Variable.DgvRowheaders[4], statisticalData.NGCount.ToString());

                 }
             });

            StasticShowAndSave();
            GenerateSlotSizeTable(productInfoA, productInfoB);
            HOperatorSet.CountSeconds(out HTuple EndUIAndFileDataSave);
            EndUIAndFileDataSave = (EndUIAndFileDataSave - StartUIAndFileDataSave) * 1000;
            MessageNotify.AddDebugLog($"UI和文件数据保存时长::{EndUIAndFileDataSave.D}");


        }




        void EADetectJudgeDeal(StatisticalData_Slice statisticalData_Slice, PFTSliceProduct pFTSliceProduct, CameraEnum cameraEnum, int sliceIndex)
        {
            for (int j = 0; j < statisticalData_Slice.defectTJDBBJItemInfos.Count; j++)
            {
                statisticalData_Slice.defectTJDBBJItemInfos[j].CountNGBJ = 0;
                statisticalData_Slice.defectTJDBBJItemInfos[j].CountNGDB = 0;
                statisticalData_Slice.defectTJDBBJItemInfos[j].CountNGTJ = 0;

            }
            ProcessImageDefects(pFTSliceProduct.ListImageDefects, cameraEnum, true, sliceIndex + 1);
            pFTSliceProduct.resultSliceDefectNg = pFTSliceProduct.ListImageDefects.Count == 0 ? ResultType.OK
                                              : pFTSliceProduct.ListImageDefects.Max(d => d.resultType);

            if (pFTSliceProduct.resultSliceDefectNg == ResultType.NG)
            {
                pFTSliceProduct.resultSliceDefectNg = ResultType.Warn;
            }

            ProductDataDefectStastic(pFTSliceProduct.ListImageDefects, statisticalData_Slice.defectItemInfos);
            List<ResultType> resultTypes = new List<ResultType>();
            resultTypes.Add(ResultType.Warn);


            TJBJDeftInfo_WriteState(cameraEnum, pFTSliceProduct.ListImageDefects, statisticalData_Slice.defectTJDBBJItemInfos, pFTCleanCraft.ListDefectInfos, sliceIndex, ref pFTSliceProduct.resultSliceDefectNg);


        }

        void EASizeJudgeDeal(PFTCleanCraft pFTCleanCraft, StatisticalData_Slice statisticalData_Slice, PFTCleanEAProductInfo pFTCleanEAProductInfo, CameraEnum cameraEnum, int sliceIndex)
        {
            ResultType result = ResultType.OK;
            result = pFTCleanCraft.ListEASizes
                           .Where(c => c.ArrayBelongSlice[sliceIndex] == 1)                                    // 满足切片
                           .Join(pFTCleanEAProductInfo.ListEASizes,
                                 c => c.Index, s => s.Index, (_, s) => s.resultType) // 同 Index
                           .DefaultIfEmpty(result)                                                    // 万一没有匹配
                           .Max();


            pFTCleanEAProductInfo.ListEASlicePD[sliceIndex].resultSliceConfigSizeNg = result;

            // 遍历所有尺寸，过滤后一次性处理
            pFTCleanEAProductInfo.ListEASizes
               .Where(s => statisticalData_Slice.sizeItemInfos.Exists(info => info.Name == s.Name && info.Index == s.Index))
               .ForEach(s =>
               {
                   var craftItem = pFTCleanCraft.ListEASizes.Find(c => c.Name == s.Name);
                   var Sizeinfo = statisticalData_Slice.sizeItemInfos.Find(info => info.Name == s.Name);

                   TJBJSize_WriteState(craftItem.camera, s, Sizeinfo, craftItem, sliceIndex);
               });

            ResultType isDB = ResultType.OK;
            bool bIsBJ = false;
            bool bIsTJ = false;
            var strSlotName = Enum.GetNames(typeof(MeasureSlotEnum));
            List<SizeItemInfo> sizeItemBJTJDBInfos = new List<SizeItemInfo>();
            foreach (var item in pFTCleanCraft.ListSlotSizeLimits)
            {
                SizeItemInfo sizeItemInfo = new SizeItemInfo();
                sizeItemInfo.Name = item.Name;
                sizeItemInfo.Description = item.Description;
                sizeItemInfo.CountNGBJ = 0;
                sizeItemInfo.CountNGDB = 0;
                sizeItemInfo.CountNGTJ = 0;
                sizeItemBJTJDBInfos.Add(sizeItemInfo);

            }
            for (int j = 1; j <= pFTCleanCraft.PerSlotCount; j++)
            {

                for (int k = 0; k < strSlotName.Length; k++)
                {
                    SlotProduct slotProduct = pFTCleanEAProductInfo.ListEASlicePD[sliceIndex].listShave.Find(s => s.slotInfo.SliceID == sliceIndex && s.slotInfo.ShaveID == j);

                    if (slotProduct == null)
                    {
                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{sliceIndex + 1}片第{j}个槽位尺寸数据缺失", describe = $"第{sliceIndex + 1}条第{j + 1}个槽位尺寸数据缺失" });
                        continue;
                    }
                    SizeInfo sizeInfo = slotProduct.SlotSizes[k];
                    SizeItemInfo sizeItemInfo = statisticalData_Slice.sizeItemInfos.Find(s => s.Description == $"第{sliceIndex + 1}条第{j}个槽位{strSlotName[k]}");
                    SizeItemInfo CraftSizeItem = CreateSizeItemInfo(pFTCleanCraft.ListSlotSizeLimits[k], sizeInfo);

                    if (pFTCleanEAProductInfo.EAID > 0)
                    {

                        AccumulateAlarmCount(sizeInfo, sizeItemInfo);
                        sizeItemInfo.CountNGBJ = 0;
                        sizeItemInfo.CountNGTJ = 0;
                        AccumulateAlarmCount(sizeInfo, sizeItemBJTJDBInfos[k]);
                        TJBJSize_WriteState(cameraEnum, sizeInfo, sizeItemBJTJDBInfos[k], CraftSizeItem, sliceIndex, ref isDB, ref bIsBJ, ref bIsTJ, j);
                        result = isDB == ResultType.NG ? ResultType.NG
                          : sizeInfo.resultType == ResultType.OK ? ResultType.OK
                          : ResultType.Warn;
                    }
                    else
                    {
                        sizeInfo.resultType = ResultType.Warn;
                    }


                    pFTCleanEAProductInfo.ListEASlicePD[sliceIndex].resultSliceGrooveSizeNg = result;

                }

            }


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

                //获取分界值
                // 1) 分界索引
                int lastCam1Index = pFTCleanCraft.ListSliceSet
                                        .Select((info, idx) => (info.IsLeftSide, idx))
                                        .LastOrDefault(x => x.IsLeftSide)
                                        .idx;

                for (int i = 0; i < pFTCleanCraft.SliceCount; i++)
                {
                    resultSliceDefectNg.Add(ResultType.NG);          // 默认 NG


                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= lastCam1Index
                                                                : i > lastCam1Index;

                    List<NgTypeInfo> defects = new List<NgTypeInfo>(); ;
                    if (needShow)
                    {
                        defects = pFTImageProduct.ListImageSlicePD[i].ListImageDefects;
                        ListImageDefects.Add(defects.Count > 0 ? defects : new List<NgTypeInfo>());
                        pFTImageProduct.showHalconWindow.m_messageShowInfos.AddRange(pFTImageProduct.ListImageSlicePD[i].showHalconWindow.m_messageShowInfos);
                        pFTImageProduct.showHalconWindow.m_regionShowInfos.AddRange(pFTImageProduct.ListImageSlicePD[i].showHalconWindow.m_regionShowInfos);
                        pFTImageProduct.showHalconWindow.m_lineShowInfos.AddRange(pFTImageProduct.ListImageSlicePD[i].ListImagelines);
                    }
                    else
                    {
                        ListImageDefects.Add(defects.Count > 0 ? defects : new List<NgTypeInfo>());
                        resultSliceDefectNg[i] = ResultType.OK;
                    }



                }

                //缺陷判断
                for (int i = 0; i < ListImageDefects.Count; i++)
                {
                    string strDeftInfo = "无";
                    ProcessImageDefects(ListImageDefects[i], cameraEnum, false, i + 1);
                    var defects = ListImageDefects[i];
                    ;
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
                    foreach (var item in defects)
                    {
                        List<RowData> strDeftData = new List<RowData>
                        {
                            new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK },
                            new RowData() { dataStr = $"{item.m_nImgIndex}", resultType =resultSliceDefectNg[i] },
                            new RowData() { dataStr = $"{item.m_iProductIndex}", resultType = ResultType.OK },
                            new RowData() { dataStr = $"{item.m_iSliceIndex+1}", resultType =resultSliceDefectNg[i]  },
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

        private void EAFrameMethord(CameraEnum cameraEnum, PFTCleanEAProductInfo pFTCleanEAProductInfo, DisplayWindowClass displayWindowClass, string[] strDeftInitCamTitle = null, string strDeftInitCamPath = null)
        {
            ExecuteMethod(() =>
            {
                if (!pFTCleanEAProductInfo.EAFinfished)
                {
                    ProcessExecutionResult result = IntervalExecution.DelayExecute(DateTime.Now, 200, pFTCleanEAProductInfo, MyDelayProcess);
                    //报警：未完成切片
                    if (result == ProcessExecutionResult.Overtime)
                    {
                        MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{pFTCleanEAProductInfo.EAID}个产品未完成切片", describe = $"第{pFTCleanEAProductInfo.EAID}个产品未完成切片" });
                    }

                }
                else
                {


                }
                //pFTCleanEAProductInfo?.Dispose();
                //return;

                //【1】工艺处理             
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_StartDataProcessTime);


                List<ResultType> resultSliceDefectNg = new List<ResultType>();

                List<List<NgTypeInfo>> ListImageDefects = new List<List<NgTypeInfo>>();

                //获取分界值
                // 1) 分界索引
                int lastCam1Index = BIZ_Variable.nSpliteIndex;

                // 2) 一次性完成缺陷收集、结果计算及显示
                ResultType resultTypeSize = ResultType.OK;
                ResultType resultTypeDefect = ResultType.OK;
                ResultType resultTypeAll = ResultType.OK;
                resultSliceDefectNg = Enumerable.Range(0, pFTCleanCraft.SliceCount)
                    .Select(i =>
                    {
                        try
                        {
                            // 根据相机及分界索引决定是否显示
                            bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= lastCam1Index
                                                                          : i > lastCam1Index;
                            // 缺陷判断 
                            List<NgTypeInfo> defects = pFTCleanEAProductInfo.ListEASlicePD[i].ListImageDefects;


                            ListImageDefects.Add(defects);                          // 缓存缺陷列表

                            // 缺陷结果：无缺陷->OK，有缺陷->最大级别
                            var result = defects.Count == 0 ? ResultType.OK
                                                            : defects.Max(d => d.resultType);

                            //// 缺陷判断 
                            //List<NgTypeInfo> defects = pFTCleanEAProductInfo.ListEADefects
                            //                             .Where(d => d.m_iSliceIndex == i)
                            //                             .ToList();                 // 当前切片缺陷

                            //ListImageDefects.Add(defects);                          // 缓存缺陷列表

                            //// 缺陷结果：无缺陷->OK，有缺陷->最大级别
                            //var result = defects.Count == 0 ? ResultType.OK
                            //                                : defects.Max(d => d.resultType);




                            if (needShow)
                            {
                                List<LineInfo> lineInfos = GetLineInfo(pFTCleanCraft, pFTCleanEAProductInfo, cameraEnum)?.DeepClone();
                                foreach (var item in lineInfos)
                                {
                                    item.ColBegin *= 0.5;
                                    item.ColEnd *= 0.5;
                                    item.RowBegin *= 0.5;
                                    item.RowEnd *= 0.5;
                                }
                                pFTCleanEAProductInfo.showHalconWindow.m_lineShowInfos.AddRange(lineInfos);

                                BIZ_General.AddMessageShowWindow(defects, pFTCleanEAProductInfo.showHalconWindow, i + 1);
                                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"S{i + 1}尺寸结果：{pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceSizeNg.ToString()}", m_stuGetColor = BIZ_General.Color(pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceSizeNg) });
                                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"S{i + 1}缺陷结果：{pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceDefectNg.ToString()}", m_stuGetColor = BIZ_General.Color(pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceDefectNg) });
                                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"S{i + 1}总结果：{pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceAllResult.ToString()}", m_stuGetColor = BIZ_General.Color(pFTCleanEAProductInfo.ListEASlicePD[i].resultSliceAllResult) });

                            }

                            return result;
                        }
                        catch (Exception ee)
                        {

                            MessageNotify.AddErrorLog($"相机{cameraEnum}第{pFTCleanEAProductInfo.EAID}个产品第{i + 1}切片缺陷判断异常：{ee.Message}");
                            return ResultType.NG;
                        }



                    })
                    .ToList();

                //配置尺寸显示
                foreach (var size in pFTCleanEAProductInfo.ListEASizes)
                {
                    pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{size.Description}:{size.DataResult}_({size.StandardValue - size.DBInfo.DownTolerance},{size.StandardValue + size.DBInfo.UpTolerance})", m_stuGetColor = BIZ_General.Color(size.resultType) });
                }
                resultTypeAll = pFTCleanEAProductInfo.ListEASlicePD.Max(s => s.resultSliceAllResult);
                pFTCleanEAProductInfo.showHalconWindow.m_nMode = 1;
                pFTCleanEAProductInfo.showHalconWindow.m_strCamName = Project.Instance.cameraManager[cameraEnum].m_CamParam.CameraName;
                pFTCleanEAProductInfo.showHalconWindow.m_bIsNG = resultTypeAll == (ResultType.NG);
                if (pFTCleanEAProductInfo.EAID == 0)
                {
                    pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = "第0个产品", m_stuGetColor = BIZ_General.Color(resultTypeAll) }); ;

                }
                pFTCleanEAProductInfo.showHalconWindow.m_messageShowInfos.Insert(0, new MessageShowInfo() { m_strMessage = resultTypeAll.ToString(), m_stuGetColor = BIZ_General.Color(resultTypeAll), m_nFontSize = 30 }); ;
                PrimitiveTransitionEA(cameraEnum, pFTCleanEAProductInfo.showHalconWindow, out Operation_ImageInfo operation_ImageInfo);
                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_DataProcessTime);

                HOperatorSet.CountSeconds(out pFTCleanEAProductInfo.Image_EndTime);
                //计算时间
                BIZ_General.CalculateProcessingTime(pFTCleanEAProductInfo);
                displayWindowClass.operation_Image.imageInfosShow.Enqueue(operation_ImageInfo);



            });
        }


        public ProcessExecutionResult MyDelayProcess(ProductBase productBase)
        {
            PFTCleanEAProductInfo pFTCleanEAProductInfo = productBase as PFTCleanEAProductInfo;
            // 你的逻辑
            if (pFTCleanEAProductInfo.EAFinfished)
            {
                return ProcessExecutionResult.Success;
            }
            else
            {
                return ProcessExecutionResult.Overtime;
            }

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
                        bool needShow = pFTCleanCraft.CurCamIsLeft ? j <= BIZ_Variable.nSpliteIndex : j > BIZ_Variable.nSpliteIndex;
                        switch (BIZ_Variable.DgvRowheaders[i])
                        {
                            case "产量":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[j].statisticalData.SumupCount.ToString());
                                break;
                            case "良率":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[j].statisticalData.Yield.ToString("0.00") + "%");
                                break;
                            case "OK数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[j].statisticalData.OKCount.ToString());
                                break;
                            case "NG数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[j].statisticalData.NGCount.ToString());
                                break;
                            case "DB数":
                                strings.Add(BIZ_Variable.StatisticalData_EASlices[j].statisticalData.NGCount.ToString());
                                break;
                        }
                    }
                    dataList.Add(strings.ToArray());
                }
                dataList.Add(new string[] { "相机A缺项细分统计" });

                //缺项细分统计

                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlicesA.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                    if (i == 0)
                    {
                        List<string> defectNames = new List<string> { "条号" }
                     .Concat(BIZ_Variable.StatisticalData_EASlicesA[i].defectItemInfos.Select(item => item.Description))
                     .ToList();
                        dataList.Add(defectNames.ToArray());
                    }

                    if (needShow)
                    {
                        dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlicesA[i].defectItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());
                    }

                }

                dataList.Add(new string[] { "相机A尺寸细分统计" });

                //尺寸细分统计
                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlicesA.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;


                    if (needShow)
                    {
                        List<string> SizeNames = new List<string> { "条号" }
                      .Concat(BIZ_Variable.StatisticalData_EASlicesA[i].sizeItemInfos.Select(item => item.Description))
                         .ToList();
                        dataList.Add(SizeNames.ToArray());
                        dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlicesA[i].sizeItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());

                    }

                }


                dataList.Add(new string[] { "相机B缺项细分统计" });

                //缺项细分统计

                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlicesB.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                    if (i == 0)
                    {
                        List<string> defectNames = new List<string> { "条号" }
                     .Concat(BIZ_Variable.StatisticalData_EASlicesB[i].defectItemInfos.Select(item => item.Description))
                     .ToList();
                        dataList.Add(defectNames.ToArray());
                    }

                    if (needShow)
                    {
                        dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlicesB[i].defectItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());
                    }

                }

                dataList.Add(new string[] { "相机B尺寸细分统计" });

                //尺寸细分统计
                for (int i = 0; i < BIZ_Variable.StatisticalData_EASlicesB.Count; i++)
                {
                    //找到statisticalData_Slices[i]中defectItemInfos的Description的集合
                    bool needShow = pFTCleanCraft.CurCamIsLeft ? i <= BIZ_Variable.nSpliteIndex : i > BIZ_Variable.nSpliteIndex;
                 

                    if (needShow)
                    {
                        List<string> SizeNames = new List<string> { "条号" }
                       .Concat(BIZ_Variable.StatisticalData_EASlicesB[i].sizeItemInfos.Select(item => item.Description))
                          .ToList();
                        dataList.Add(SizeNames.ToArray());

                        dataList.Add(new List<string>
                            {
                                 (i + 1).ToString()
                            }.Concat(BIZ_Variable.StatisticalData_EASlicesB[i].sizeItemInfos.Select(item => item.CountNGDB.ToString())).ToArray());

                    }

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
               
                // 1. 初始化切片图表控件
                var GrooveSize = Enum.GetNames(typeof(MeasureSlotEnum));
                var _data3DCacheGroup =
                    new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, Queue<ParamsChart>>>>>();
                int sliceIndex = form.GetCob_SliceIndex();
                bool needShow = pFTCleanCraft.CurCamIsLeft ? sliceIndex <= (BIZ_Variable.nSpliteIndex) : sliceIndex > BIZ_Variable.nSpliteIndex;

                // 2. 备份旧缓存（flag=true 时）
                if (ChartGroovePage.Count > 0 && flag)
                {
                    for (int i = 0; i < GrooveSize.Length; i++)
                        _data3DCacheGroup[i] = ChartGroovePage[i]._data3DCache;
                }

                ChartGroovePage?.Clear();
             
                // 3. 预创建所有 ChartUIParam（保持原索引）
                for (int i = 0; i < pFTCleanCraft.ListSlotSizeLimits.Count; i++)
                    ChartGroovePage[i] = new ChartUIParam(pFTCleanCraft.ListSlotSizeLimits[i].Description, new ScottPlot.FormsPlot());

                // 4. 面板清空
                form.tab_GrooveChart.Controls?.Clear();

                int colCount = 2;
                int rowCount = (GrooveSize.Length + colCount - 1) / colCount;   // 全部槽位参与布局

                form.tab_GrooveChart.RowCount = rowCount;
                form.tab_GrooveChart.ColumnCount = colCount;
                form.tab_GrooveChart.Dock = DockStyle.Fill;

                // 5. 均分行样式
                form.tab_GrooveChart.RowStyles.Clear();
                for (int j = 0; j < rowCount; j++)
                    form.tab_GrooveChart.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));

                // 6. 均分列样式
                form.tab_GrooveChart.ColumnStyles.Clear();
                for (int j = 0; j < colCount; j++)
                    form.tab_GrooveChart.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / colCount));

                // 7. 按原始顺序创建 & 放置 & 显隐
                for (int i = 0; i < GrooveSize.Length; i++)
                {
                  
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

                  
                    if (!needShow)
                    {
                        MessageNotify.AddDebugLog("InitGrooveChart()_切片不在显示范围");
                        return;
                    }

                    var slot = pFTCleanCraft.ListSlotInfos?
                               .FirstOrDefault(s => s.SliceID == sliceIndex && s.ShaveID == grooveIndex + 1);
                    if (slot == null)
                    {
                        MessageNotify.AddErrorLog("InitGrooveChart()_切片信息为空");
                        return;
                    }

                    ChartGroovePage[i].plotView.Dock = DockStyle.Fill;

                    var paramsChart = new ParamsChart
                    {
                        f_UpLimit = pFTCleanCraft.ListSlotSizeLimits[i].UpLimitValue + slot.AllValues[i],
                        f_DownLimit = pFTCleanCraft.ListSlotSizeLimits[i].lowLimitValue + slot.AllValues[i],
                        f_StandarValue = slot.AllValues[i]
                    };
                    paramsChart.points.Add(new PointClass
                    {
                        CurName = "测量值0",
                        Point = new Point(0, 0),
                        Color = Color.Blue
                    });

                    paramsChart.points.Add(new PointClass
                    {
                        CurName = "测量值1",
                        Point = new Point(0, 0),
                        Color = Color.Red
                    });


                    ChartGroovePage[i].InitCurve(paramsChart);
                    ChartGroovePage[i].Init3DCache(pFTCleanCraft.ListSliceSet?.Select(d => d.ShaveNum).ToArray(),
                                                 GrooveSize.Length);

                    // 恢复缓存
                    if (flag && _data3DCacheGroup?.Count > 0)
                        ChartGroovePage[i]._data3DCache = _data3DCacheGroup[i];

                    // 8. 核心：先按原位置放进面板，再决定显隐
                    form.tab_GrooveChart.Controls.Add(ChartGroovePage[i].plotView,
                                                    i % colCount,
                                                    i / colCount);
                    ChartGroovePage[i].plotView.Visible = pFTCleanCraft.ListSlotSizeLimits[i].Enable;
                }
            });
        }




        public void GrooveSliceDataJudge(PFTCleanCraft cleanCraft,
            List<PFTSliceProduct> slotMatrix, CameraEnum cameraEnum)
        {
            ExecuteMethod(() =>
            {
                int splitIndex = BIZ_Variable.nSpliteIndex;
                for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
                {
                    bool needshow = (cleanCraft.CurCamIsLeft ? sliceId <= (splitIndex) : sliceId > splitIndex);
                    if (needshow)
                    {
                        for (int shaveId = 0; shaveId < cleanCraft.PerSlotCount; shaveId++)
                        {
                            var cur = slotMatrix[sliceId].listShave[shaveId];
                            var strs = Enum.GetNames(typeof(MeasureSlotEnum));
                            for (int i = 0; i < strs.Length; i++)
                            {
                                SizeItemInfo sizeItemInfoCur = CreateSizeItemInfo(cleanCraft.ListSlotSizeLimits[i], cur.SlotSizes[i], sliceId);
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
                }
            });
        }

        List<LineInfo> GetLineInfo(PFTCleanCraft cleanCraft, PFTCleanEAProductInfo pFTCleanEAProductInfo, CameraEnum cameraEnum)
        {
            List<LineInfo> lineInfos = new List<LineInfo>();
            lineInfos.AddRange(pFTCleanEAProductInfo.ListEAlines);
            /* ====== 2. 扁平化所有槽位 ====== */
            // 1. 找到分界点（最后一个 Cam1 的索引）
            int lastCam1Index = cleanCraft.ListSliceSet
                                       .Select((info, idx) => new { info.IsLeftSide, idx })
                                       .LastOrDefault(x => x.IsLeftSide)
                                       ?.idx ?? -1;

            for (int i = 0; i < pFTCleanEAProductInfo.ListEASlicePD.Count; i++)
            {
                bool needShow = cleanCraft.CurCamIsLeft ? i <= lastCam1Index
                                                       : i > lastCam1Index;
                if (needShow)
                {
                    lineInfos.AddRange(pFTCleanEAProductInfo.ListEASlicePD[i].ListImagelines);
                }
            }

            return lineInfos;


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
            List<PFTSliceProduct> slotMatrixA,
            List<PFTSliceProduct> slotMatrixB,
            double pixelSizeX,
            double pixelSizeY,
            out bool isSuccess)
        {
            isSuccess = ExecuteMethod(() =>
            {
                /* ====== 1. 参数合法性校验 ====== */
                if (cleanCraft?.ListSliceSet?[0] == null)
                    return false;


                int splitIndex = cleanCraft.ListSliceSet
                                           .Select((info, idx) => new { info.IsLeftSide, idx })
                                           .LastOrDefault(x => x.IsLeftSide)
                                           ?.idx ?? -1;

                MeasureDataSignleDistance(cleanCraft, slotMatrixA, CameraEnum.Cam1);
                MeasureDataSignleDistance(cleanCraft, slotMatrixB, CameraEnum.Cam2);
                //算错位

                for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
                {
                    bool needshow = (cleanCraft.CurCamIsLeft ? sliceId <= (splitIndex) : sliceId > splitIndex);
                    if (needshow)
                    {
                        for (int shaveId = 0; shaveId < cleanCraft.PerSlotCount; shaveId++)
                        {
                            SlotProduct curA = slotMatrixA[sliceId].listShave[shaveId];
                            SlotProduct curB = slotMatrixB[sliceId].listShave[shaveId];
                            double offsetX = curA.SlotSizes[(int)MeasureSlotEnum.TD定位距].DataResult - curB.SlotSizes[(int)MeasureSlotEnum.TD定位距].DataResult;
                            curA.SlotSizes[(int)MeasureSlotEnum.TD错位].DataResult = offsetX;
                            curB.SlotSizes[(int)MeasureSlotEnum.TD错位].DataResult = -offsetX;
                            if (curA.Slotlines[(int)DirectionEnum.上边].IsLineSuccessfull && curB.Slotlines[(int)DirectionEnum.上边].IsLineSuccessfull)
                            {
                                double offsetY =
                                ((curA.Slotlines[(int)DirectionEnum.上边].RowBegin + curA.Slotlines[(int)DirectionEnum.上边].RowEnd) * 0.5
                                 - (curB.Slotlines[(int)DirectionEnum.上边].RowBegin + curB.Slotlines[(int)DirectionEnum.上边].RowEnd) * 0.5) * pixelSizeY;
                                curA.SlotSizes[(int)MeasureSlotEnum.MD错位].DataResult = offsetY;
                                curB.SlotSizes[(int)MeasureSlotEnum.MD错位].DataResult = -offsetY;
                            }

                        }
                    }

                }

                return true; // 全部成功
            });
        }



        public void MeasureDataSignleDistance(PFTCleanCraft cleanCraft,
            List<PFTSliceProduct> slotMatrixA,
             CameraEnum cameraEnum)
        {
            ExecuteMethod(() =>
            {

                double pixelSizeX = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_X;
                double pixelSizeY = Project.Instance.cameraManager[cameraEnum].m_CamParam.CalibrationValue_Y;
                /* ====== 2. 扁平化所有槽位 ====== */
                // 1. 找到分界点（最后一个 Cam1 的索引）
                int splitIndex = BIZ_Variable.nSpliteIndex;

                /* ====== 3. TD 间距计算 ====== */

                for (int shaveId = 0; shaveId < cleanCraft.PerSlotCount; shaveId++)
                {
                    for (int sliceId = 0; sliceId < cleanCraft.SliceCount - 1; sliceId++)
                    {
                        bool needshow = (cleanCraft.CurCamIsLeft ? sliceId <= (splitIndex - 1) : sliceId > splitIndex);

                        if (!needshow)
                        {
                            continue;
                        }


                        SlotProduct cur = slotMatrixA[sliceId].listShave[shaveId];
                        SlotProduct next = slotMatrixA[sliceId + 1].listShave[shaveId];



                        /* ******  公共计算  ****** */
                        double tdWidth = Math.Abs(
                        (cur.Slotlines[(int)DirectionEnum.左间隙边].ColBegin +
                         cur.Slotlines[(int)DirectionEnum.左间隙边].ColEnd -
                         cur.Slotlines[(int)DirectionEnum.右边].ColBegin -
                         cur.Slotlines[(int)DirectionEnum.右边].ColEnd) * 0.5 * pixelSizeX);

                        // TD 间距 + 对齐
                        cur.SlotSizes[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;
                        next.SlotSizes[(int)MeasureSlotEnum.TD间距].DataResult = tdWidth;
                        cur.SlotSizes[(int)MeasureSlotEnum.TD对齐].DataResult = cur.SlotSizes[(int)MeasureSlotEnum.TD定位距].DataResult - cur.SlotSizes[(int)MeasureSlotEnum.TD定位距].StandardValue;
                        next.SlotSizes[(int)MeasureSlotEnum.TD对齐].DataResult = next.SlotSizes[(int)MeasureSlotEnum.TD定位距].DataResult - next.SlotSizes[(int)MeasureSlotEnum.TD定位距].StandardValue;


                        // TD 左 / 右间隙（共用同一宽度）
                        tdWidth = Math.Abs(
                            (cur.Slotlines[(int)DirectionEnum.左边].ColBegin +
                             cur.Slotlines[(int)DirectionEnum.左边].ColEnd -
                             cur.Slotlines[(int)DirectionEnum.左间隙边].ColBegin -
                             cur.Slotlines[(int)DirectionEnum.左间隙边].ColEnd) * 0.5 * pixelSizeX);

                        cur.SlotSizes[(int)MeasureSlotEnum.TD左间隙].DataResult = tdWidth;
                        next.SlotSizes[(int)MeasureSlotEnum.TD左间隙].DataResult = tdWidth;

                        tdWidth = Math.Abs(
                         (cur.Slotlines[(int)DirectionEnum.右边].ColBegin +
                          cur.Slotlines[(int)DirectionEnum.右边].ColEnd -
                          cur.Slotlines[(int)DirectionEnum.右间隙边].ColBegin -
                          cur.Slotlines[(int)DirectionEnum.右间隙边].ColEnd) * 0.5 * pixelSizeX);

                        cur.SlotSizes[(int)MeasureSlotEnum.TD右间隙].DataResult = tdWidth;
                        next.SlotSizes[(int)MeasureSlotEnum.TD右间隙].DataResult = tdWidth;

                        /* ******  Cam2 区间额外处理最后一个 slice 右间隙  ****** */
                        if (!pFTCleanCraft.CurCamIsLeft && sliceId > splitIndex)   // 处于 Cam2 区间
                        {
                            if (sliceId == cleanCraft.SliceCount - 2)              // 最后一个 slice
                            {
                                tdWidth = Math.Abs(
                                    (next.Slotlines[(int)DirectionEnum.右边].ColBegin +
                                     next.Slotlines[(int)DirectionEnum.右边].ColEnd -
                                     next.Slotlines[(int)DirectionEnum.右间隙边].ColBegin -
                                     next.Slotlines[(int)DirectionEnum.右间隙边].ColEnd) * 0.5 * pixelSizeX);
                                next.SlotSizes[(int)MeasureSlotEnum.TD右间隙].DataResult = tdWidth;
                            }
                        }


                        cur.SlotSizes[(int)MeasureSlotEnum.MD尺寸].DataResult = Math.Abs(
                            (cur.Slotlines[(int)DirectionEnum.上边].RowBegin +
                             cur.Slotlines[(int)DirectionEnum.上边].RowEnd -
                             cur.Slotlines[(int)DirectionEnum.下边].RowBegin -
                             cur.Slotlines[(int)DirectionEnum.下边].RowEnd) * 0.5 * pixelSizeY);

                        next.SlotSizes[(int)MeasureSlotEnum.MD尺寸].DataResult = Math.Abs(
                            (next.Slotlines[(int)DirectionEnum.上边].RowBegin +
                             next.Slotlines[(int)DirectionEnum.上边].RowEnd -
                             next.Slotlines[(int)DirectionEnum.下边].RowBegin -
                             next.Slotlines[(int)DirectionEnum.下边].RowEnd) * 0.5 * pixelSizeY);

                        cur.SlotSizes[(int)MeasureSlotEnum.TD尺寸].DataResult = Math.Abs(
                           (cur.Slotlines[(int)DirectionEnum.左边].ColBegin +
                            cur.Slotlines[(int)DirectionEnum.左边].ColEnd -
                            cur.Slotlines[(int)DirectionEnum.右边].ColBegin -
                            cur.Slotlines[(int)DirectionEnum.右边].ColEnd) * 0.5 * pixelSizeX);

                        next.SlotSizes[(int)MeasureSlotEnum.TD尺寸].DataResult = Math.Abs(
                             (next.Slotlines[(int)DirectionEnum.左边].ColBegin +
                              next.Slotlines[(int)DirectionEnum.左边].ColEnd -
                              next.Slotlines[(int)DirectionEnum.右边].ColBegin -
                              next.Slotlines[(int)DirectionEnum.右边].ColEnd) * 0.5 * pixelSizeX);


                    }
                }


                /* ====== 4. MD 间距计算 ====== */
                for (int sliceId = 0; sliceId < cleanCraft.SliceCount; sliceId++)
                {
                    bool needshow = (pFTCleanCraft.CurCamIsLeft ? sliceId <= (splitIndex ) : sliceId > splitIndex);

                    if (!needshow)
                    {
                        continue;
                    }

                    for (int shaveId = 0; shaveId < cleanCraft.PerSlotCount - 1; shaveId++)
                    {
                        SlotProduct cur = slotMatrixA[sliceId].listShave[shaveId];
                        SlotProduct next = slotMatrixA[sliceId].listShave[shaveId + 1];

                        double mdHeihgt = Math.Abs(
                            (cur.Slotlines[(int)DirectionEnum.上边].RowBegin +
                             cur.Slotlines[(int)DirectionEnum.上边].RowEnd -
                             next.Slotlines[(int)DirectionEnum.上边].RowBegin -
                             next.Slotlines[(int)DirectionEnum.上边].RowEnd) * 0.5 * pixelSizeY);

                        if (!cur.Slotlines[(int)DirectionEnum.上边].IsLineSuccessfull && !next.Slotlines[(int)DirectionEnum.上边].IsLineSuccessfull)
                        {
                            mdHeihgt = -999;
                        }

                        /* ******  公共计算  ****** */
                        if (shaveId == 0)
                        {
                            cur.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult = cur.SlotSizes[(int)MeasureSlotEnum.TD定位距].DataResult;
                        }

                        next.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult = mdHeihgt;

                        cur.SlotSizes[(int)MeasureSlotEnum.MD对齐].DataResult = cur.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult - cur.SlotSizes[(int)MeasureSlotEnum.MD间距].StandardValue;
                        next.SlotSizes[(int)MeasureSlotEnum.MD对齐].DataResult = next.SlotSizes[(int)MeasureSlotEnum.MD间距].DataResult - next.SlotSizes[(int)MeasureSlotEnum.MD间距].StandardValue;

                    }
                }

            });
        }









        public void MeasureNewConfigSize(PFTCleanCraft cleanCraft,
        PFTCleanEAProductInfo pFTCleanEAProductInfoA, PFTCleanEAProductInfo pFTCleanEAProductInfoB)
        {
            ExecuteMethod(() =>
            {
                List<LineInfo> lineInfosA = GetLineInfo(cleanCraft, pFTCleanEAProductInfoA, CameraEnum.Cam1);

                List<LineInfo> lineInfosB = GetLineInfo(cleanCraft, pFTCleanEAProductInfoA, CameraEnum.Cam2);

                var lineMatrix = new[] { lineInfosA, lineInfosB };

                /* ---------- 扁平化所有槽位 ---------- */
                var allSlots = GetNewAllSlots(cleanCraft, pFTCleanEAProductInfoA.ListEASlicePD, pFTCleanEAProductInfoB.ListEASlicePD);

                var ProductInfos = new List<PFTCleanEAProductInfo>();
                ProductInfos.Add(pFTCleanEAProductInfoA);
                ProductInfos.Add(pFTCleanEAProductInfoB);


                for (var cam = CameraEnum.Cam1; cam <= CameraEnum.Cam2; cam++)
                {
                    var listIdx = (int)cam;
                    double pixelSizeX = Project.Instance.cameraManager[cam].m_CamParam.CalibrationValue_X;
                    double pixelSizeY = Project.Instance.cameraManager[cam].m_CamParam.CalibrationValue_Y;
                    ProductInfos[listIdx].ListEASizes?.Clear();
                    ProductInfos[listIdx].ListEASizes = new List<SizeInfo>();
                    for (int i = 0; i < cleanCraft.ListEASizes.Count; i++)
                    {
                        var cfg = cleanCraft.ListEASizes[i];
                        if (cfg.camera != cam)
                            continue;

                        var line1 = GetLineInfo(cfg.Line1, cfg.ShaveIndex1, cfg.camera1, allSlots[listIdx], lineMatrix);
                        var line2 = GetLineInfo(cfg.Line2, cfg.ShaveIndex2, cfg.camera2, allSlots[listIdx], lineMatrix);
                        SizeInfo sizeInfo = new SizeInfo();
                        sizeInfo.Name = cfg.Name;
                        sizeInfo.Description = cfg.Description;
                        sizeInfo.Index = cfg.Index;

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

                        GetSizeJudgeInfo(cfg, ref sizeInfo);
                        ProductInfos[listIdx].ListEASizes.Add(sizeInfo);

                    }
                }

            });
        }

        /// <summary>
        /// 生成槽位汇总尺寸表格
        /// </summary>
        public void GenerateSlotSizeTable(PFTCleanEAProductInfo pFTCleanEAProductInfoA,
          PFTCleanEAProductInfo pFTCleanEAProductInfoB)
        {
            //临时修整


            List<string> titles = new List<string>();
            titles.Add("时间");
            titles.Add("运行速度");
            titles.Add("产品型号");
            titles.Add("膜卷号");
            titles.Add("相机索引");
            titles.Add("EA号");
            titles.Add("条号");
            titles.Add("各分条判定结果");

            foreach (var slot in pFTCleanCraft.ListSlotSizeLimits)
            {
                if (slot.Enable)
                {
                    titles.Add(slot.Description);
                }
            }

            foreach (var slot in pFTCleanCraft.ListSlotSizeLimits)
            {
                if (slot.Enable)
                {
                    MeasureSlotEnum slotEnum = (MeasureSlotEnum)Enum.Parse(typeof(MeasureSlotEnum), slot.Name);
                    //看看slotEnum在titleDic中对应的多个key值
                    for (int i = 1; i <= pFTCleanCraft.PerSlotCount; i++)
                    {
                        titles.Add(slot.Description+i.ToString());
                        titles.Add(slot.Description + i.ToString()+"判定结果");

                    }

                }
            }


            List<PFTCleanEAProductInfo> pFTCleanEAProductInfos = new List<PFTCleanEAProductInfo>() { pFTCleanEAProductInfoA, pFTCleanEAProductInfoB };


            for (int i = 0; i < pFTCleanEAProductInfos.Count; i++)
            {
                for (int j = 0; j < pFTCleanCraft.SliceCount; j++)
                {
                   bool needshow = (pFTCleanCraft.CurCamIsLeft ? j <= (BIZ_Variable.nSpliteIndex) : j > BIZ_Variable.nSpliteIndex);
                    if(needshow)
                    {
                        List<RowData> rowDatas = new List<RowData>();
                        rowDatas.Add(new RowData() { dataStr = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = Project.Instance.slaveMachineManager.GetMacheineSpeed().ToString(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = Project.Instance.slaveMachineManager.GetProduceCode(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = Project.Instance.slaveMachineManager.GetRollCode(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = i.ToString(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = pFTCleanEAProductInfoA.EAID.ToString(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = (j + 1).ToString(), resultType = ResultType.OK });
                        rowDatas.Add(new RowData() { dataStr = pFTCleanEAProductInfos[i].ListEASlicePD[j].resultSliceAllResult.ToString(), resultType = ResultType.OK });
                        foreach (var slot in pFTCleanCraft.ListSlotSizeLimits)
                        {
                            if (slot.Enable)
                            {
                                MeasureSlotEnum slotEnum = (MeasureSlotEnum)Enum.Parse(typeof(MeasureSlotEnum), slot.Name);
                                var sizeInfos = pFTCleanEAProductInfos[i].ListEASlicePD[j].listShave.SelectMany(sp => sp.SlotSizes).Where(size => size.Name == slotEnum.ToString()).ToList();
                                var result = sizeInfos.Max(x => x.resultType);
                                rowDatas.Add(new RowData() { dataStr = result.ToString(), resultType = result });

                            }
                        }
                        foreach (var slot in pFTCleanCraft.ListSlotSizeLimits)
                        {
                            if (slot.Enable)
                            {
                                MeasureSlotEnum slotEnum = (MeasureSlotEnum)Enum.Parse(typeof(MeasureSlotEnum), slot.Name);
                                var sizeInfos = pFTCleanEAProductInfos[i].ListEASlicePD[j].listShave.SelectMany(sp => sp.SlotSizes).Where(size => size.Name == slotEnum.ToString()).ToList();
                                for (int n = 1; n <= pFTCleanCraft.PerSlotCount; n++)
                                {
                                    rowDatas.Add(new RowData() { dataStr = sizeInfos[n - 1].DataResult.ToString(), resultType = sizeInfos[n - 1].resultType });
                                    rowDatas.Add(new RowData() { dataStr = sizeInfos[n - 1].resultType.ToString(), resultType = sizeInfos[n - 1].resultType });
                                }
                            }
                        }

                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + "EA_新数据汇总表", titles.ToArray(), rowDatas.ToArray(), "");

                    }
                }
              
            }






          

        }


        List<List<SlotProduct>> GetNewAllSlots(
         PFTCleanCraft cleanCraft,
            List<PFTSliceProduct> slotMatrixA,
             List<PFTSliceProduct> slotMatrixB)
        {
            var lastCam1Index = cleanCraft.ListSliceSet
                                          .Select((info, idx) => (info.IsLeftSide, idx))
                                          .LastOrDefault(x => x.IsLeftSide)
                                          .idx;

            List<SlotProduct> allSlotsA = new List<SlotProduct>();
            List<SlotProduct> allSlotsB = new List<SlotProduct>();
            for (var i = 0; i < cleanCraft.SliceCount; i++)
            {
                bool needShow = (cleanCraft.CurCamIsLeft ? i <= (lastCam1Index) : i > lastCam1Index);
                if (needShow)
                {
                    allSlotsA.AddRange(slotMatrixA[i].listShave);
                    allSlotsB.AddRange(slotMatrixB[i].listShave);

                }
            }
            return new List<List<SlotProduct>> { allSlotsA, allSlotsB };
        }




        /// <summary>
        /// 依据 SlotSizeLimit 与 SizeInfo 创建 SizeItemInfo 实例
        /// </summary>
        /// <param name="limit">槽位尺寸限制配置</param>
        /// <param name="size">当前尺寸结果</param>
        /// <returns>已初始化的 SizeItemInfo</returns>
        private SizeItemInfo CreateSizeItemInfo(SlotSizeLimit limit, SizeInfo size, int sliceIndex = 0)
        {


            return new SizeItemInfo
            {
                // 基础开关
                Enable = limit.Enable,
                DB = limit.ArrayDBSlice[sliceIndex] == 1,

                // 标准值与尺寸公差
                StDValue = size.StandardValue,
                StD_Up = size.upperLimitValue - size.StandardValue,
                StD_Down = size.lowerLimitValue - size.StandardValue,
                CountNGDB = (int)limit.ArrayDBSliceCount[sliceIndex],

                // 报警配置
                BJ = limit.ArrayBJSlice[sliceIndex] == 1,
                CountNGBJ = (int)limit.ArrayDBSliceCount[sliceIndex],
                StD_BJ_Up = size.lowerLimitValue - size.StandardValue,
                StD_BJ_Down = size.lowerLimitValue - size.StandardValue
            };
        }




        List<SlotProduct> GetAllSlots(
        PFTCleanCraft cleanCraft,
           List<PFTSliceProduct> slotMatrixA,
            List<PFTSliceProduct> slotMatrixB)
        {
            var lastCam1Index = cleanCraft.ListSliceSet
                                          .Select((info, idx) => (info.IsLeftSide, idx))
                                          .LastOrDefault(x => x.IsLeftSide)
                                          .idx;

            return cleanCraft.ListSliceSet
                             .Select((_, idx) => idx <= lastCam1Index ? slotMatrixA[idx] : slotMatrixB[idx])
                             .SelectMany(slots => slots.listShave)
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
            //1.先判断lines里面是否有lineConfig和IsLineSuccessfull==true的line
            //2.如果有，则返回lineConfig对应的line
            //3.如果没有，则返回lines有StrLineName == lineConfig，
            // 普通线名
            // 根据相机类型选择对应的线条列表
            var lines = camera == CameraEnum.Cam1 ? lineMatrix[0] : lineMatrix[1];

            // 如果线条列表为空，直接返回 null
            if (lines == null || lines.Count == 0)
                return null;

            // 优先查找成功的线条，且名称匹配
            var successfulLine = lines.FirstOrDefault(x => x.IsLineSuccessfull && x.StrLineName == lineConfig);

            // 如果找到成功的线条，返回它；否则返回第一个名称匹配的线条（无论是否成功）
            return successfulLine ?? lines.FirstOrDefault(l => l.StrLineName == lineConfig);

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
        public void TJBJDeftInfo_WriteState(CameraEnum cameraEnum, List<NgTypeInfo> deftInfos, List<DefectItemInfo> TJBJDeftInfos, List<DefectItemInfo> CraftDeftInfos, int nsliceIndex, ref ResultType resultType)
        {
            foreach (var item in deftInfos)
            {
                var deftIndex = TJBJDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == CameraEnum.Cam1);
                var deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == CameraEnum.Cam1);
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
                    deftCraftIndex = CraftDeftInfos.FindIndex(x => x.Name == item.m_strNgTypeName.ToString() && x.camera == CameraEnum.Cam1);
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


            if (TJBJSizeInfos.CountNGBJ > CraftSizeInfo.CountNGBJ && CraftSizeInfo.BJ)
            {
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description, describe = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description });
                //TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.报警, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description);
                TJBJSizeInfos.CountNGBJ = 0;
            }


            if (TJBJSizeInfos.CountNGTJ > CraftSizeInfo.CountNGTJ && CraftSizeInfo.TJ)
            {
                MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.停机, name = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description, describe = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description });
                // TJBJInfo.AlarmWriteState(AlarmName.检测项报警停机, StopAlarmStateEnum.停机, cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description);
                TJBJSizeInfos.CountNGTJ = 0;
            }

        }

        public void TJBJSize_WriteState(CameraEnum cameraEnum, SizeInfo sizeInfo, SizeItemInfo TJBJSizeInfos, SizeItemInfo CraftSizeInfo, int nsliceIndex, ref ResultType IsDB, ref bool BBJ, ref bool BTJ, int zoneIndex)
        {

            if (TJBJSizeInfos.CountNGDB >= CraftSizeInfo.CountNGDB && CraftSizeInfo.DB)
            {
                //MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description, describe = cameraEnum.ToString() + $"_第{nsliceIndex + 1}条_" + sizeInfo.Description });
                if (zoneIndex == pFTCleanCraft.PerSlotCount)
                {
                    MessageNotify.AddDebugLog($"第{nsliceIndex + 1}条：当前{TJBJSizeInfos.Description}>=设定值{CraftSizeInfo.CountNGDB}，触发DB：{TJBJSizeInfos.CountNGDB}");
                }

                IsDB = ResultType.NG;


            }

            if (TJBJSizeInfos.CountNGBJ > CraftSizeInfo.CountNGBJ && CraftSizeInfo.BJ)
            {
                if (!BBJ)
                {

                    BBJ = true;
                }
                if (zoneIndex == pFTCleanCraft.PerSlotCount)
                {
                    MessageNotify.AddStopAlarm(new StopAlarmMessage() { time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"), StopAlarmState = StopAlarmStateEnum.报警, name = $"第{nsliceIndex + 1}条：当前{TJBJSizeInfos.Description}超过设定值{CraftSizeInfo.CountNGBJ}，触发BJ：{TJBJSizeInfos.CountNGBJ}", describe = $"第{nsliceIndex + 1}条：当前{TJBJSizeInfos.Description}超过设定值{CraftSizeInfo.CountNGBJ}，触发BJ：{TJBJSizeInfos.CountNGBJ}" });
                }

            }


            if (TJBJSizeInfos.CountNGTJ > CraftSizeInfo.CountNGTJ && CraftSizeInfo.TJ)
            {
                if (zoneIndex == pFTCleanCraft.PerSlotCount)
                {
                    MessageNotify.AddStopAlarm(new StopAlarmMessage()
                    {
                        time = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss"),
                        StopAlarmState = StopAlarmStateEnum.停机,
                        name = $"第{nsliceIndex + 1}条：当前{TJBJSizeInfos.Description}超过设定值{CraftSizeInfo.CountNGTJ}，触发TJ：{TJBJSizeInfos.CountNGTJ}",
                        describe = $"第{nsliceIndex + 1}条：当前{TJBJSizeInfos.Description}超过设定值{CraftSizeInfo.CountNGTJ}，触发TJ" +
               $"：{TJBJSizeInfos.CountNGTJ}"
                    });
                }
                if (!BTJ)
                {
                    BTJ = true;
                }


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

                        bool needshow = pFTCleanCraft.CurCamIsLeft ? sliceIndex <= BIZ_Variable.nSpliteIndex : sliceIndex > BIZ_Variable.nSpliteIndex;

                        if (needshow)
                        {
                            ChartGroovePage[i].UpData3DCache(sliceIndex, grooveIndex, i);
                        }


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
                bool isShowImage = in_imageSaveParamer.IsShowImage_Product;
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
                    if (pFTCleanCraft.ListDefectInfos[defectIndex].Enable && pFTCleanCraft.ListDefectInfos[defectIndex].camera == CameraEnum.Cam1)
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
