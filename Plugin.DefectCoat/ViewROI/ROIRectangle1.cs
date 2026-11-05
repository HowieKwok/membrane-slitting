using System;
using HalconDotNet;


namespace ViewROI
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
	/// <remarks>
	/// 本插件只把 Rectangle1 作为局部 ROI 和屏蔽 ROI 的界面编辑形状。
	/// 对外模型参数固定为 row1、col1、row2、col2 四个值，不输出中心点、宽高或角度。
	/// </remarks>
	public class ROIRectangle1 : ROI
	{

		/// <summary>矩形左上角坐标；row1 是上边界行坐标，col1 是左边界列坐标。</summary>
		private double row1, col1;   // upper left
		/// <summary>矩形右下角坐标；row2 是下边界行坐标，col2 是右边界列坐标，拖动后会保证 row2 >= row1、col2 >= col1。</summary>
		private double row2, col2;   // lower right 
		/// <summary>矩形中心点坐标，只用于中心手柄拖动，不作为本插件保存参数的主字段。</summary>
		private double midR, midC;   // midpoint 

        /// <summary>
        /// 创建 Rectangle1 ROI。
        /// 5 个控制点分别为四个角点和中心点，默认激活中心点。
        /// </summary>
        public ROIRectangle1()
		{

			NumHandles = 5; // 4 corner points + midpoint
			activeHandleIdx = 4;
		}

		/// <summary>在鼠标位置创建默认 100x100 像素的 Rectangle1。</summary>
		/// <param name="midX">
		/// x (=column) coordinate for interactive ROI
		/// </param>
		/// <param name="midY">
		/// y (=row) coordinate for interactive ROI
		/// </param>
		public override void createROI(double midX, double midY)
		{
			midR = midY;
			midC = midX;

			row1 = midR - 50;
			col1 = midC - 50;
			row2 = midR + 50;
			col2 = midC + 50;
		}

		/// <summary>在 HALCON 窗口中绘制矩形边框和五个拖拽控制点。</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			window.DispRectangle1(row1, col1, row2, col2);

			window.DispRectangle2(row1, col1, 0, 5, 5);
			window.DispRectangle2(row1, col2, 0, 5, 5);
			window.DispRectangle2(row2, col2, 0, 5, 5);
			window.DispRectangle2(row2, col1, 0, 5, 5);
			window.DispRectangle2(midR, midC, 0, 5, 5);
		}

		/// <summary>
		/// 计算鼠标点到五个控制点的最小距离，并记录最近控制点为 activeHandleIdx。
		/// x 是列坐标，y 是行坐标。
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

			midR = ((row2 - row1) / 2) + row1;
			midC = ((col2 - col1) / 2) + col1;

			val[0] = HMisc.DistancePp(y, x, row1, col1); // upper left 
			val[1] = HMisc.DistancePp(y, x, row1, col2); // upper right 
			val[2] = HMisc.DistancePp(y, x, row2, col2); // lower right 
			val[3] = HMisc.DistancePp(y, x, row2, col1); // lower left 
			val[4] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

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

		/// <summary>用激活手柄颜色绘制当前 activeHandleIdx 指向的控制点。</summary>
		/// <param name="window">HALCON window</param>
		public override void displayActive(HalconDotNet.HWindow window)
		{
			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(row1, col1, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(row1, col2, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(row2, col2, 0, 5, 5);
					break;
				case 3:
					window.DispRectangle2(row2, col1, 0, 5, 5);
					break;
				case 4:
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>把当前四坐标转换为 HALCON Rectangle1 Region。</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRectangle1(row1, col1, row2, col2);
			return region;
		}

		/// <summary>
		/// 输出 Rectangle1 的模型参数。
		/// 返回顺序固定为 row1、col1、row2、col2，HalconViewRoiAdapter 依赖该顺序回写界面参数。
		/// </summary>
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { row1, col1, row2, col2 });
		}


		/// <summary>
		/// 根据当前激活控制点拖动矩形。
		/// 四角手柄改变对应边界，中心手柄整体平移；最后会归一化上下左右边界。
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
					row1 = newY;
					col1 = newX;
					break;
				case 1: // upper right 
					row1 = newY;
					col2 = newX;
					break;
				case 2: // lower right 
					row2 = newY;
					col2 = newX;
					break;
				case 3: // lower left
					row2 = newY;
					col1 = newX;
					break;
				case 4: // midpoint 
					len1 = ((row2 - row1) / 2);
					len2 = ((col2 - col1) / 2);

					row1 = newY - len1;
					row2 = newY + len1;

					col1 = newX - len2;
					col2 = newX + len2;

					break;
			}

			if (row2 <= row1)
			{
				tmp = row1;
				row1 = row2;
				row2 = tmp;
			}

			if (col2 <= col1)
			{
				tmp = col1;
				col1 = col2;
				col2 = tmp;
			}

			midR = ((row2 - row1) / 2) + row1;
			midC = ((col2 - col1) / 2) + col1;

		}//end of method
	}//end of class
}//end of namespace
