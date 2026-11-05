using Extensions;
using Frame_Core;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class Form_ControlCard : Form
    {
        BIZ_ControlCard bIZ_ControlCard = new BIZ_ControlCard();
      
        public Form_ControlCard(BIZ_ControlCard _bIZ_ControlCard)
        {
            InitializeComponent();
            bIZ_ControlCard = _bIZ_ControlCard;
        }

        public void LoadParam()
        {
            try
            {
                ControlTool.SetParamShow(cbx_AxisIndex1, ref bIZ_ControlCard.param_ControlCard.axis_Param.AxisIndex);
                ControlTool.SetParamShow(nud_GearRatioMolecule1, ref bIZ_ControlCard.param_ControlCard.axis_Param.GearRatioMolecule);
                ControlTool.SetParamShow(nud_GearRatioDenominator1, ref bIZ_ControlCard.param_ControlCard.axis_Param.GearRatioDenominator);
                ControlTool.SetParamShow(nud_AxisVelocity1, ref bIZ_ControlCard.param_ControlCard.axis_Param.Velocity);
                ControlTool.SetParamShow(nud_AxisAcceleration1, ref bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration);
                ControlTool.SetParamShow(nud_SmoothTime1, ref bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime);
                ControlTool.SetParamShow(nud_WorkPosition1, ref bIZ_ControlCard.param_ControlCard.axis_Param.WorkPosition1);
                ControlTool.SetParamShow(nud_MovementDistance, ref bIZ_ControlCard.param_ControlCard.axis_Param.MovementDistance);
                ControlTool.SetParamShow(cbx_duanM_IsAutoFocusing, ref bIZ_ControlCard.param_ControlCard.duanM_IsAutoFocusing);
                ControlTool.SetParamShow(cbx_duanM_Sign, ref bIZ_ControlCard.param_ControlCard.duanM_Sign);
                ControlTool.SetParamShow(nud_DuanMWorkTolerances, ref bIZ_ControlCard.param_ControlCard.duanM_WorkTolerances);
                ControlTool.SetParamShow(nud_daM_ReferenceCoordinate, ref bIZ_ControlCard.param_ControlCard.daM_ReferenceCoordinate);
                ControlTool.SetParamShow(nud_daM_SamplingQuantity, ref bIZ_ControlCard.param_ControlCard.daM_SamplingQuantity);
                ControlTool.SetParamShow(nud_WorkPositionDuanM1, ref bIZ_ControlCard.param_ControlCard.duanM_WorkPosition);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }


        public void SaveParam()
        {
            try
            {
                ControlTool.GetParamSave(cbx_AxisIndex1, ref bIZ_ControlCard.param_ControlCard.axis_Param.AxisIndex);
                ControlTool.GetParamSave(nud_GearRatioMolecule1, ref bIZ_ControlCard.param_ControlCard.axis_Param.GearRatioMolecule);
                ControlTool.GetParamSave(nud_GearRatioDenominator1, ref bIZ_ControlCard.param_ControlCard.axis_Param.GearRatioDenominator);
                ControlTool.GetParamSave(nud_AxisVelocity1, ref bIZ_ControlCard.param_ControlCard.axis_Param.Velocity);
                ControlTool.GetParamSave(nud_AxisAcceleration1, ref bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration);
                ControlTool.GetParamSave(nud_SmoothTime1, ref bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime);
                ControlTool.GetParamSave(nud_WorkPosition1, ref bIZ_ControlCard.param_ControlCard.axis_Param.WorkPosition1);
                ControlTool.GetParamSave(nud_MovementDistance, ref bIZ_ControlCard.param_ControlCard.axis_Param.MovementDistance);
                ControlTool.GetParamSave(cbx_duanM_IsAutoFocusing, ref bIZ_ControlCard.param_ControlCard.duanM_IsAutoFocusing);
                ControlTool.GetParamSave(cbx_duanM_Sign, ref bIZ_ControlCard.param_ControlCard.duanM_Sign);
                ControlTool.GetParamSave(nud_DuanMWorkTolerances, ref bIZ_ControlCard.param_ControlCard.duanM_WorkTolerances);
                ControlTool.GetParamSave(nud_daM_ReferenceCoordinate, ref bIZ_ControlCard.param_ControlCard.daM_ReferenceCoordinate);
                ControlTool.GetParamSave(nud_daM_SamplingQuantity, ref bIZ_ControlCard.param_ControlCard.daM_SamplingQuantity);
                ControlTool.GetParamSave(nud_WorkPositionDuanM1, ref bIZ_ControlCard.param_ControlCard.duanM_WorkPosition);

                bIZ_ControlCard.param_ControlCard.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
            }
        }

        private void Form_ControlCard_Load(object sender, EventArgs e)
        {
            LoadParam();         
        }

        private void Form_ControlCard_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btn_AxisStop1_Click(object sender, EventArgs e)
        {
            bIZ_ControlCard.Gen.MoveStop(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID);
        }

        private void btn_GoToHome1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                bIZ_ControlCard.Gen.GoToOOrigin(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity,
                    bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, bIZ_ControlCard.GetGearRatio()); 
            });           
        }

        private void btn_GoToWorkPosition1_Click(object sender, EventArgs e)
        {
            int workPosition = Convert.ToInt32(bIZ_ControlCard.param_ControlCard.axis_Param.WorkPosition1);
            bIZ_ControlCard.Gen.MoveStart(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, workPosition, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity, bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, true);
        }

        private void btn_NegativeMovement_Click(object sender, EventArgs e)
        {
            int workPosition = bIZ_ControlCard.MMToPulse(bIZ_ControlCard.param_ControlCard.axis_Param.MovementDistance);
            bIZ_ControlCard.Gen.MoveStart(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, -workPosition, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity, bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, false);
        }

        private void btn_PositiveMovement_Click(object sender, EventArgs e)
        {
            int workPosition = bIZ_ControlCard.MMToPulse(bIZ_ControlCard.param_ControlCard.axis_Param.MovementDistance);
            bIZ_ControlCard.Gen.MoveStart(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, workPosition, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity, bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, false);
        }

        private void btn_PositionZero1_Click(object sender, EventArgs e)
        {
            bIZ_ControlCard.Gen.ZeroPos(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID);
        }

        private void btn_ClearAlamr1_Click(object sender, EventArgs e)
        {
            bIZ_ControlCard.Gen.ClrSts(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID);
        }

        private void btn_AllGoToHome_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                bIZ_ControlCard.Gen.GoToOOrigin(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity,
                    bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, bIZ_ControlCard.GetGearRatio());
            });
        }

        private void btn_AllGoToWorkPosition_Click(object sender, EventArgs e)
        {
            int workPosition = Convert.ToInt32(bIZ_ControlCard.param_ControlCard.duanM_WorkPosition);
            bIZ_ControlCard.Gen.MoveStart(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, workPosition, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity, bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, true);
        }

        private void btn_GoToWorkPosition_DuanM1_Click(object sender, EventArgs e)
        {
            int workPosition = Convert.ToInt32(bIZ_ControlCard.param_ControlCard.duanM_WorkPosition);
            bIZ_ControlCard.Gen.MoveStart(bIZ_ControlCard.param_ControlCard.axis_Param.AxisID, workPosition, bIZ_ControlCard.param_ControlCard.axis_Param.Velocity, bIZ_ControlCard.param_ControlCard.axis_Param.Acceleration, (short)bIZ_ControlCard.param_ControlCard.axis_Param.SmoothTime, true);
        }

        private void btn_SaveParam_Click(object sender, EventArgs e)
        {
            SaveParam();
        }      

        private void UpdateState(PictureBox pictureBox, bool Value)
        {
            if (Value)  
            {
                pictureBox.BackColor = Color.Red; 
                pictureBox.ForeColor = Color.Black;
            }
            else
            {
                pictureBox.BackColor = Color.LightGray;
                pictureBox.ForeColor = Color.Black;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateState(pic_NegativeLimit1, bIZ_ControlCard.Gen.axis_State.NegativeLimit);
                UpdateState(pic_PositiveLimit1, bIZ_ControlCard.Gen.axis_State.PositiveLimit);
                UpdateState(pic_Home1, bIZ_ControlCard.Gen.axis_State.Home);
                UpdateState(pic_Alarm1, bIZ_ControlCard.Gen.axis_State.Alarm);
                UpdateState(pic_Error1, bIZ_ControlCard.Gen.axis_State.Abnormal);

                nud_AxisCurrentPosition1.Value = (decimal)bIZ_ControlCard.Gen.axis_State.CurrentPosition_Pulse;
                nud_AxisTargetPosition1.Value = (decimal)bIZ_ControlCard.Gen.axis_State.AxisTargetPosition;

                tbx_StepLength1.Text = ((double)bIZ_ControlCard.param_ControlCard.axis_Param.MovementDistance / bIZ_ControlCard.GetGearRatio()).ToString();
            }
            catch (Exception)
            {
               // Log4netRecord.ErrorLog.Error(ex);
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
