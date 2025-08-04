using System;
using HalconDotNet;

using NPOI.SS.Formula.Functions;

namespace ViewROI3
{
	/// <summary>
	/// This class demonstrates one of the possible implementations for a 
	/// linear ROI. ROILine inherits from the base class ROI and 
	/// implements (besides other auxiliary methods) all virtual methods 
	/// defined in ROI.cs.
	/// </summary>
	public class ROILine : ROI
	{

		//private double row1, col1;   // first end point of line
		//private double row2, col2;   // second end point of line
		//private double midR, midC;   // midPoint of line
  //      private ROIType rOIType = ROIType.Ïß;
        private HXLDCont arrowHandleXLD;

		public ROILine(ROIParams roiParams)
		{
			NumHandles = 3;        // two end points of line
			activeHandleIdx = 2;
			arrowHandleXLD = new HXLDCont();
			arrowHandleXLD.GenEmptyObj();
            this.roiParams=roiParams;
            MeasureEnable = roiParams.MeasureEnable;
            this.roiParams.rOItype = ROIType.Ïß;

        }

        public ROILine()
		{
            NumHandles = 3;        // two end points of line
            activeHandleIdx = 2;
            arrowHandleXLD = new HXLDCont();
            arrowHandleXLD.GenEmptyObj();
        }


        /// <summary>Creates a new ROI instance at the mouse position.</summary>
        public override void createROI(double midX, double midY)
		{
            this.roiParams.midR = midY;
            this.roiParams.midC = midX;

            this.roiParams.row1 = this.roiParams.midR;
            this.roiParams.col1 = this.roiParams.midC - 50;
            this.roiParams.row2 = this.roiParams.midR;
            this.roiParams.col2 = this.roiParams.midC + 50;

			updateArrowHandle();
		}

		/// <summary>Paints the ROI into the supplied window.</summary>
		public override void draw(HalconDotNet.HWindow window,string index)
		{

			window.DispLine(this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2);
            window.SetTposition((int)roiParams.midR, (int)roiParams.midC);
            window.WriteString(index.ToString());
            window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
			window.DispObj(arrowHandleXLD);  //window.DispRectangle2( row2, col2, 0, 5, 5);
			window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
            if (!index.Contains("°´×ó¼ü"))
            {
                ROIIndex = Convert.ToInt32(index);
                roiParams.roiIndex = ROIIndex;
            }
        }

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y).
		/// </summary>
		public override double distToClosestHandle(double x, double y)
		{

			double max = 10000;
			double [] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, this.roiParams.row1, this.roiParams.col1); // upper left 
			val[1] = HMisc.DistancePp(y, x, this.roiParams.row2, this.roiParams.col2); // upper right 
			val[2] = HMisc.DistancePp(y, x, this.roiParams.midR, this.roiParams.midC); // midpoint 

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
		/// Paints the active handle of the ROI object into the supplied window. 
		/// </summary>
		public override void displayActive(HalconDotNet.HWindow window)
		{

			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
					break;
				case 1:
					window.DispObj(arrowHandleXLD); //window.DispRectangle2(row2, col2, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>Gets the HALCON region described by the ROI.</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRegionLine(this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2);
			return region;
		}

		public override double getDistanceFromStartPoint(double row, double col)
		{
			double distance = HMisc.DistancePp(row, col, this.roiParams.row1, this.roiParams.col1);
			return distance;
		}
		/// <summary>
		/// Gets the model information described by 
		/// the ROI.
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2 });
		}

		/// <summary> 
		/// Recalculates the shape of the ROI. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y).
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			double lenR, lenC;

			switch (activeHandleIdx)
			{
				case 0: // first end point
                    this.roiParams.row1 = newY;
                    this.roiParams.col1 = newX;

                    this.roiParams.midR = (this.roiParams.row1 + this.roiParams.row2) / 2;
                    this.roiParams.midC = (this.roiParams.col1 + this.roiParams.col2) / 2;
					break;
				case 1: // last end point
                    this.roiParams.row2 = newY;
                    this.roiParams.col2 = newX;

                    this.roiParams.midR = (this.roiParams.row1 + this.roiParams.row2) / 2;
                    this.roiParams.midC = (this.roiParams.col1 + this.roiParams.col2) / 2;
					break;
				case 2: // midpoint 
					lenR = this.roiParams.row1 - this.roiParams.midR;
					lenC = this.roiParams.col1 - this.roiParams.midC;

                    this.roiParams.midR = newY;
                    this.roiParams.midC = newX;

                    this.roiParams.row1 = this.roiParams.midR + lenR;
                    this.roiParams.col1 = this.roiParams.midC + lenC;
                    this.roiParams.row2 = this.roiParams.midR - lenR;
                    this.roiParams.col2 = this.roiParams.midC - lenC;
					break;
			}
			updateArrowHandle();
		}


        /// <summary> Auxiliary method </summary>
        public void updateArrowHandle()
		{
            double length, dr, dc, halfHW;
            double rrow1, ccol1, rowP1, colP1, rowP2, colP2;

            double headLength = 15;
            double headWidth = 15;


            arrowHandleXLD.Dispose();
            arrowHandleXLD.GenEmptyObj();

            rrow1 =roiParams.row1 + (roiParams.row2 - roiParams.row1) * 0.8;
            ccol1 = roiParams.col1 + (roiParams.col2 - roiParams.col1) * 0.8;

            length = HMisc.DistancePp(rrow1, ccol1, roiParams.row2, roiParams.col2);
            if (length == 0)
                length = -1;

            dr = (roiParams.row2 - rrow1) / length;
            dc = (roiParams.col2 - ccol1) / length;

            halfHW = headWidth / 2.0;
            rowP1 = rrow1 + (length - headLength) * dr + halfHW * dc;
            rowP2 = rrow1 + (length - headLength) * dr - halfHW * dc;
            colP1 = ccol1 + (length - headLength) * dc - halfHW * dr;
            colP2 = ccol1 + (length - headLength) * dc + halfHW * dr;

            if (length == -1)
                arrowHandleXLD.GenContourPolygonXld(rrow1, ccol1);
            else
                arrowHandleXLD.GenContourPolygonXld(new HTuple(new double[] { rrow1, roiParams.row2, rowP1, roiParams.row2, rowP2, roiParams.row2 }),
                                                    new HTuple(new double[] { ccol1, roiParams.col2, colP1, roiParams.col2, colP2, roiParams.col2 }));

        }

    }//end of class
}//end of namespace
