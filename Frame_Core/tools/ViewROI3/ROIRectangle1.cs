using System;
using HalconDotNet;


namespace ViewROI3
{
	/// <summary>
	/// This class demonstrates one of the possible implementations for a 
	/// (simple) rectangularly shaped ROI. ROIRectangle1 inherits 
	/// from the base class ROI and implements (besides other auxiliary
	/// methods) all virtual methods defined in ROI.cs.
	/// Since a simple rectangle is defined by two data points, by the upper 
	/// left corner and the lower right corner, we use four values (row1/col1) 
	/// and (row2/col2) as class members to hold these positions at 
	/// any time of the program. The four corners of the rectangle can be taken
	/// as handles, which the user can use to manipulate the size of the ROI. 
	/// Furthermore, we define a midpoint as an additional handle, with which
	/// the user can grab and drag the ROI. Therefore, we declare NumHandles
	/// to be 5 and set the activeHandle to be 0, which will be the upper left
	/// corner of our ROI.
	/// </summary>
	public class ROIRectangle1 : ROI
	{

		//private double row1, col1;   // upper left
		//private double row2, col2;   // lower right 
		//private double midR, midC;   // midpoint 
  //      private ROIType rOIType = ROIType.¾ØÐÎ1;
        /// <summary>Constructor</summary>
        public ROIRectangle1()
		{

			NumHandles = 5; // 4 corner points + midpoint
			activeHandleIdx = 4;
		}

        /// <summary>Constructor</summary>
        public ROIRectangle1(ROIParams roiParams)
        {

            NumHandles = 5; // 4 corner points + midpoint
            activeHandleIdx = 4;
            this.roiParams = roiParams;
            MeasureEnable = roiParams.MeasureEnable;
      
            this.roiParams.rOItype =ROIType.¾ØÐÎ1;
        }

        /// <summary>Creates a new ROI instance at the mouse position</summary>
        /// <param name="midX">
        /// x (=column) coordinate for interactive ROI
        /// </param>
        /// <param name="midY">
        /// y (=row) coordinate for interactive ROI
        /// </param>
        public override void createROI(double midX, double midY)
		{
            this.roiParams.midR = midY;
            this.roiParams.midC = midX;

            this.roiParams.row1 = this.roiParams.midR - 50;
            this.roiParams.col1 = this.roiParams.midC - 50;
            this.roiParams.row2 = this.roiParams.midR + 50;
            this.roiParams.col2 = this.roiParams.midC + 50;
		}

		/// <summary>Paints the ROI into the supplied window</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window,string index)
		{
			window.DispRectangle1(this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2);

			window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
			window.DispRectangle2(this.roiParams.row1, this.roiParams.col2, 0, 5, 5);
			window.DispRectangle2(this.roiParams.row2, this.roiParams.col2, 0, 5, 5);
			window.DispRectangle2(this.roiParams.row2, this.roiParams.col1, 0, 5, 5);
			window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
            window.SetTposition((int)roiParams.midR, (int)roiParams.midC);
            window.WriteString(index.ToString());
            if (!index.Contains("°´×ó¼ü"))
            {
                ROIIndex = Convert.ToInt32(index);
                roiParams.roiIndex = ROIIndex;
            }
        }

		/// <summary> 
		/// Returns the distance of the ROI handle being
		/// closest to the image point(x,y)
		/// </summary>
		/// <param name="x">x (=column) coordinate</param>
		/// <param name="y">y (=row) coordinate</param>
		/// <returns> 
		/// Distance of the closest ROI handle.
		/// </returns>
		public override double distToClosestHandle(double x, double y)
		{

			double max = 10000;
			double [] val = new double[NumHandles];

            this.roiParams.midR = ((this.roiParams.row2 - this.roiParams.row1) / 2) + this.roiParams.row1;
            this.roiParams.midC = ((this.roiParams.col2 - this.roiParams.col1) / 2) + this.roiParams.col1;

			val[0] = HMisc.DistancePp(y, x, this.roiParams.row1, this.roiParams.col1); // upper left 
			val[1] = HMisc.DistancePp(y, x, this.roiParams.row1, this.roiParams.col2); // upper right 
			val[2] = HMisc.DistancePp(y, x, this.roiParams.row2, this.roiParams.col2); // lower right 
			val[3] = HMisc.DistancePp(y, x, this.roiParams.row2, this.roiParams.col1); // lower left 
			val[4] = HMisc.DistancePp(y, x, this.roiParams.midR, this.roiParams.midC); // midpoint 

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
		/// <param name="window">HALCON window</param>
		public override void displayActive(HalconDotNet.HWindow window)
		{
			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(this.roiParams.row1, this.roiParams.col1, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(this.roiParams.row1, this.roiParams.col2, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(this.roiParams.row2, this.roiParams.col2, 0, 5, 5);
					break;
				case 3:
					window.DispRectangle2(this.roiParams.row2, this.roiParams.col1, 0, 5, 5);
					break;
				case 4:
					window.DispRectangle2(this.roiParams.midR, this.roiParams.midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>Gets the HALCON region described by the ROI</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRectangle1(this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2);
			return region;
		}

		/// <summary>
		/// Gets the model information described by 
		/// the interactive ROI
		/// </summary> 
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { this.roiParams.row1, this.roiParams.col1, this.roiParams.row2, this.roiParams.col2 });
		}


		/// <summary> 
		/// Recalculates the shape of the ROI instance. Translation is 
		/// performed at the active handle of the ROI object 
		/// for the image coordinate (x,y)
		/// </summary>
		/// <param name="newX">x mouse coordinate</param>
		/// <param name="newY">y mouse coordinate</param>
		public override void moveByHandle(double newX, double newY)
		{
			double len1, len2;
			double tmp;

			switch (activeHandleIdx)
			{
				case 0: // upper left 
                    this.roiParams.row1 = newY;
                    this.roiParams.col1 = newX;
					break;
				case 1: // upper right 
                    this.roiParams.row1 = newY;
                    this.roiParams.col2 = newX;
					break;
				case 2: // lower right 
                    this.roiParams.row2 = newY;
                    this.roiParams.col2 = newX;
					break;
				case 3: // lower left
                    this.roiParams.row2 = newY;
                    this.roiParams.col1 = newX;
					break;
				case 4: // midpoint 
					len1 = ((this.roiParams.row2 - this.roiParams.row1) / 2);
					len2 = ((this.roiParams.col2 - this.roiParams.col1) / 2);

                    this.roiParams.row1 = newY - len1;
                    this.roiParams.row2 = newY + len1;

                    this.roiParams.col1 = newX - len2;
                    this.roiParams.col2 = newX + len2;

					break;
			}

			if (this.roiParams.row2 <= this.roiParams.row1)
			{
				tmp = this.roiParams.row1;
                this.roiParams.row1 = this.roiParams.row2;
                this.roiParams.row2 = tmp;
			}

			if (this.roiParams.col2 <= this.roiParams.col1)
			{
				tmp = this.roiParams.col1;
                this.roiParams.col1 = this.roiParams.col2;
                this.roiParams.col2 = tmp;
			}

            this.roiParams.midR = ((this.roiParams.row2 - this.roiParams.row1) / 2) + this.roiParams.row1;
            this.roiParams.midC = ((this.roiParams.col2 - this.roiParams.col1) / 2) + this.roiParams.col1;

		}//end of method
	}//end of class
}//end of namespace
