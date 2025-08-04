using Frame_Core;
using LogTool;
using NPOI.SS.UserModel.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class ChartForm : Form
    {
        /// <summary>
        /// 显示点数量
        /// </summary>
        int showPointNum = 100;
  
        /// <summary>
        /// 曲线图表
        /// </summary>
        public Dictionary<SizeEnum, EHGraph> EHGraphs = new Dictionary<SizeEnum, EHGraph>();

        public ChartForm()
        {
            InitializeComponent();        
        }
        private void ChartForm_Load(object sender, EventArgs e)
        {
            InitChartForm();
        }

        private void ChartForm_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        /// <summary>
        /// 初始化曲线图界面
        /// </summary>
        private void InitChartForm()
        {
            try
            {

                //====================================
                //初始化界面 
                foreach (var item in Enum.GetNames(typeof(SizeEnum)))//初始化曲线图
                {
                    SizeEnum sizeEnum = (SizeEnum)Enum.Parse(typeof(SizeEnum), item);
                    EHGraphs.Add(sizeEnum, new EHGraph(item.ToString()));//chartBase
                    EHGraphs[sizeEnum].AddLine();
                    EHGraphs[sizeEnum].AdaptYAxes(40, 50);
                    EHGraphs[sizeEnum].ChangeChartName(item.ToString());

                }

                TableLayoutPanel TablePanel = tableLayoutPanel_Main;
                TablePanel.AutoScroll = true;
                TablePanel.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(TablePanel, true);

                TablePanel.Controls.Clear();
                TablePanel.ColumnCount = 1;
                TablePanel.RowCount = EHGraphs.Count;

                //tablePanel.ColumnStyles.Clear();
                //tablePanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

                TablePanel.RowStyles.Clear();
                for (int i = 0; i < TablePanel.RowCount; i++)
                {
                    TablePanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
                }

                foreach (var item in EHGraphs.Keys)
                {
                    TablePanel.Controls.Add(EHGraphs[item].groupBox);
                }

                SetAdaptYAxes();
            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }


        /// <summary>
        /// 刷新曲线图名称
        /// </summary>
        public void UpdateChartName()
        {

        }

  

        /// <summary>
        /// 刷新曲线图窗体
        /// </summary>
        public void UpdataChartForm(SizeEnum sizeEnum, double pointData)
        {
            try
            {
                int num = EHGraphs.Where(p => p.Key == sizeEnum).ToList().Count;
                if (num == 0) return;
                EHGraphs[sizeEnum].AddPoint(new List<double>() { pointData }, (uint)showPointNum);
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 清除曲线图窗体数据
        /// </summary>
        public void ClearChartForm()
        {
            try
            {
                foreach (var item in EHGraphs.Keys)
                {
                    EHGraphs[item].AddLine();
                }

            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        /// <summary>
        /// 设置曲线图上下限区域
        /// </summary>
        public void SetAdaptYAxes()
        {
            try
            {
                //double upper = 100;
                //double lower = -20;
                //upper = Project.Instance.craftInfo.disPositionInfoList[selectGroove + 0].ProcessUper;
                //lower = Project.Instance.craftInfo.disPositionInfoList[selectGroove + 0].ProcessLower;
                //EHGraphs[ChartName.A定位距Y].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disPositionInfoList[selectStrip + GrooveCount].ProcessUper;
                //lower = Project.Instance.craftInfo.disPositionInfoList[selectStrip + GrooveCount].ProcessLower;
                //EHGraphs[ChartName.A定位距X].AdaptYAxes(lower, upper);


                //upper = Project.Instance.craftInfo.disPositionInfoList[selectGroove + (SliceCount + GrooveCount)].ProcessUper;
                //lower = Project.Instance.craftInfo.disPositionInfoList[selectGroove + (SliceCount + GrooveCount)].ProcessLower;
                //EHGraphs[ChartName.B定位距Y].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disPositionInfoList[(selectStrip + GrooveCount) + (SliceCount + GrooveCount)].ProcessUper;
                //lower = Project.Instance.craftInfo.disPositionInfoList[(selectStrip + GrooveCount) + (SliceCount + GrooveCount)].ProcessLower;
                //EHGraphs[ChartName.B定位距X].AdaptYAxes(lower, upper);


                //upper = Project.Instance.craftInfo.disGrooveInfoList[1].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[1].ProcessLower;
                //EHGraphs[ChartName.A槽长].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disGrooveInfoList[0].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[0].ProcessLower;
                //EHGraphs[ChartName.A槽宽].AdaptYAxes(lower, upper);


                //upper = Project.Instance.craftInfo.disGrooveInfoList[1].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[1].ProcessLower;
                //EHGraphs[ChartName.B槽长].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disGrooveInfoList[0].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[0].ProcessLower;
                //EHGraphs[ChartName.B槽宽].AdaptYAxes(lower, upper);


                //upper = Project.Instance.craftInfo.disGrooveInfoList[2].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[2].ProcessLower;
                //EHGraphs[ChartName.错位左].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disGrooveInfoList[3].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[3].ProcessLower;
                //EHGraphs[ChartName.错位上].AdaptYAxes(lower, upper);


                //upper = Project.Instance.craftInfo.disGrooveInfoList[4].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[4].ProcessLower;
                //EHGraphs[ChartName.错位右].AdaptYAxes(lower, upper);

                //upper = Project.Instance.craftInfo.disGrooveInfoList[5].ProcessUper;
                //lower = Project.Instance.craftInfo.disGrooveInfoList[5].ProcessLower;
                //EHGraphs[ChartName.错位下].AdaptYAxes(lower, upper);

            }
            catch (Exception ex)
            {

                 Log4netRecord.ErrorLog.Debug(ex);
            }
        }

    

    }
}
