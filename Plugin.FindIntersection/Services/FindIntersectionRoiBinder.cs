using System;
using Frame_Core.pluginParams;
using HalconDotNet;
using Plugin.FindIntersection.ViewModels;
using ViewROI;

namespace Plugin.FindIntersection.Services
{
    public sealed class FindIntersectionRoiBinder
    {
        private readonly ROIController roiController;
        private readonly ROIRectangle2 line1Roi = new ROIRectangle2();
        private readonly ROIRectangle2 line2Roi = new ROIRectangle2();
        private bool line1Created;
        private bool line2Created;

        public FindIntersectionRoiBinder(ROIController roiController)
        {
            if (roiController == null) throw new ArgumentNullException(nameof(roiController));
            this.roiController = roiController;
        }

        public ROIRectangle2 Line1Roi
        {
            get { return line1Roi; }
        }

        public ROIRectangle2 Line2Roi
        {
            get { return line2Roi; }
        }

        public void ApplyParamsToViewRoi(ParamsFindIntersection p)
        {
            if (p == null) return;
            ApplyLine1(p);
            ApplyLine2(p);
            roiController.activeROIidx = -1;
        }

        public void SaveViewRoiToParams(ParamsFindIntersection p)
        {
            if (p == null) return;
            SaveLine1(p);
            SaveLine2(p);
        }

        public void SaveViewRoiToViewModel(ToolVm vm)
        {
            if (vm == null) return;

            vm.Params.UpdateFromRoi(1, line1Roi.midR, line1Roi.midC,
                line1Roi.phi * 180.0 / Math.PI, line1Roi.length1 * 2.0,
                line1Roi.length2 * 2.0, line1Roi.PointNum);

            vm.Params.UpdateFromRoi(2, line2Roi.midR, line2Roi.midC,
                line2Roi.phi * 180.0 / Math.PI, line2Roi.length1 * 2.0,
                line2Roi.length2 * 2.0, line2Roi.PointNum);
        }

        public double GetActiveAngleDegree()
        {
            ROIRectangle2 roi = roiController.activeROIidx == 1 ? line2Roi : line1Roi;
            return roi.phi * 180.0 / Math.PI;
        }

        private void ApplyLine1(ParamsFindIntersection p)
        {
            line1Roi.PointNum = p.hv_Line1_RoiPointsNum;
            line1Roi.ROIName = "Line1";
            line1Roi.midR = p.hv_Line1_RoiMidRow;
            line1Roi.midC = p.hv_Line1_RoiMidColumn;
            HOperatorSet.TupleRad(p.hv_Line1_RoiAngle, out HTuple phi);
            line1Roi.phi = phi.D;
            line1Roi.length2 = p.hv_Line1_RoiWidth / 2.0;
            line1Roi.length1 = p.hv_Line1_RoiHeight / 2.0;
            EnsureCreated(line1Roi, ref line1Created);
        }

        private void ApplyLine2(ParamsFindIntersection p)
        {
            line2Roi.PointNum = p.hv_Line2_RoiPointsNum;
            line2Roi.ROIName = "Line2";
            line2Roi.midR = p.hv_Line2_RoiMidRow;
            line2Roi.midC = p.hv_Line2_RoiMidColumn;
            HOperatorSet.TupleRad(p.hv_Line2_RoiAngle, out HTuple phi);
            line2Roi.phi = phi.D;
            line2Roi.length2 = p.hv_Line2_RoiWidth / 2.0;
            line2Roi.length1 = p.hv_Line2_RoiHeight / 2.0;
            EnsureCreated(line2Roi, ref line2Created);
        }

        private void EnsureCreated(ROIRectangle2 roi, ref bool created)
        {
            if (!created)
            {
                created = true;
                roiController.setROIShape(roi);
                roiController.roiMode.createROI(roi.midC, roi.midR);
                roiController.ROIList.Add(roiController.roiMode);
                roiController.resetROI();
            }

            roi.updateHandlePos();
        }

        private void SaveLine1(ParamsFindIntersection p)
        {
            p.hv_Line1_RoiPointsNum = line1Roi.PointNum;
            p.hv_Line1_RoiMidRow = line1Roi.midR;
            p.hv_Line1_RoiMidColumn = line1Roi.midC;
            HOperatorSet.TupleDeg(line1Roi.phi, out HTuple angle);
            p.hv_Line1_RoiAngle = angle;
            p.hv_Line1_RoiWidth = line1Roi.length2 * 2.0;
            p.hv_Line1_RoiHeight = line1Roi.length1 * 2.0;
        }

        private void SaveLine2(ParamsFindIntersection p)
        {
            p.hv_Line2_RoiPointsNum = line2Roi.PointNum;
            p.hv_Line2_RoiMidRow = line2Roi.midR;
            p.hv_Line2_RoiMidColumn = line2Roi.midC;
            HOperatorSet.TupleDeg(line2Roi.phi, out HTuple angle);
            p.hv_Line2_RoiAngle = angle;
            p.hv_Line2_RoiWidth = line2Roi.length2 * 2.0;
            p.hv_Line2_RoiHeight = line2Roi.length1 * 2.0;
        }
    }
}
