using System;
using HalconDotNet;

using NPOI.SS.Formula.Functions;

namespace ViewROI3
{
	/// <summary>
	/// This class demonstrates one of the possible implementations for a 
	/// circular ROI. ROICircle inherits from the base class ROI and 
	/// implements (besides other auxiliary methods) all virtual methods 
	/// defined in ROI.cs.
	/// </summary>
	public class ROICircle : ROI
	{

		//private double radius;
		//private double row1, col1;  // first handle
		//private double midR, midC;  // second handle
		//private ROIType rOIType = ROIType.圆;
		public int nIndexRoi = -1;


        public ROICircle()
		{
			NumHandles = 2; // one at corner of circle + midpoint
			activeHandleIdx = 1;
		}

		public ROICircle(ROIParams roiParams)
		{
            NumHandles = 2; // one at corner of circle + midpoint
            activeHandleIdx = 1;
            this.roiParams = roiParams;
			MeasureEnable = roiParams.MeasureEnable;
            this.roiParams.rOItype = ROIType.圆;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        public override void createROI(double midX, double midY)
		{
            this.roiParams.midR = midY;
            this.roiParams.midC = midX;

            this.roiParams.radius = 100;

            this.roiParams.row1 = this.roiParams.midR;
            this.roiParams.col1 = this.roiParams.midC + this.roiParams.radius;
		}

		/// <summary>Paints the ROI into the supplied window</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window, string index)
		{
			window.DispCircle(this.roiParams.midR, this.roiParams.midC, this.roiParams.radius);
			window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
			window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
            if (!index.Contains("按左键"))
            {
                ROIIndex = Convert.ToInt32(index);
				roiParams.roiIndex = ROIIndex;
            }
        }

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			double [] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, this.roiParams.row1, this.roiParams.col1); // border handle 
			val[1] = HMisc.DistancePp(y, x, this.roiParams.midR, this.roiParams.midC); // midpoint 

			for (int i=0; i < NumHandles; i++)
			{
				if (val[i] < max)
				{
					max = val[i];
					activeHandleIdx = i;
				}
			}// end of for 
			return val[activeHandleIdx];
		}

		/// <summary> 
		/// Paints the active handle of the ROI object into the supplied window 
		/// </summary>
		public override void displayActive(HalconDotNet.HWindow window)
		{

			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>Gets the HALCON region described by the ROI</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenCircle(this.roiParams.midR, this.roiParams.midC, this.roiParams.radius);
			return region;
		}

		public override double getDistanceFromStartPoint(double row, double col)
		{
			double sRow = this.roiParams.midR; // assumption: we have an angle starting at 0.0
			double sCol = this.roiParams.midC + 1 * this.roiParams.radius;

			double angle = HMisc.AngleLl(this.roiParams.midR, this.roiParams.midC, sRow, sCol, this.roiParams.midR, this.roiParams.midC, row, col);

			if (angle < 0)
				angle += 2 * Math.PI;

			return (this.roiParams.radius * angle);
		}

		/// <summary>
		/// Gets the model information described by 
		/// the  ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { this.roiParams.midR, this.roiParams.midC, this.roiParams.radius });
		}

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y)
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			HTuple distance;
			double shiftX,shiftY;

			switch (activeHandleIdx)
			{
				case 0: // handle at circle border

                    this.roiParams.row1 = newY;
                    this.roiParams.col1 = newX;
					HOperatorSet.DistancePp(new HTuple(this.roiParams.row1), new HTuple(this.roiParams.col1),
											new HTuple(this.roiParams.midR), new HTuple(this.roiParams.midC),
											out distance);

                    this.roiParams.radius = distance[0].D;
					break;
				case 1: // midpoint 

					shiftY = this.roiParams.midR - newY;
					shiftX = this.roiParams.midC - newX;

                    this.roiParams.midR = newY;
                    this.roiParams.midC = newX;

                    this.roiParams.row1 -= shiftY;
                    this.roiParams.col1 -= shiftX;
					break;
			}
		}
	}//end of class
}//end of namespace
