using System;
using HalconDotNet;

namespace ViewROI
{
	/// <summary>
	/// This class demonstrates one of the possible implementations for a 
	/// circular ROI. ROICircle inherits from the base class ROI and 
	/// implements (besides other auxiliary methods) all virtual methods 
	/// defined in ROI.cs.
	/// </summary>
	/// <remarks>
	/// 圆形 ROI 是 ViewROI 库保留的通用形状。
	/// 当前涂布缺陷参数界面只开放 Rectangle1；后续若开放圆形 ROI，必须同步参数保存和检测层 CreateRegion。
	/// </remarks>
	public class ROICircle : ROI
	{

		/// <summary>圆半径，单位为像素。</summary>
		private double radius;
		/// <summary>圆边界控制点坐标，用于拖动改变半径。</summary>
		private double row1, col1;  // first handle
		/// <summary>圆心坐标，用于整体平移。</summary>
		private double midR, midC;  // second handle


		/// <summary>创建圆形 ROI，两个控制点分别为边界点和圆心。</summary>
		public ROICircle()
		{
			NumHandles = 2; // one at corner of circle + midpoint
			activeHandleIdx = 1;
		}



		/// <summary>在鼠标位置创建默认半径 100 像素的圆形 ROI。</summary>
		public override void createROI(double midX, double midY)
		{
			midR = midY;
			midC = midX;

			radius = 100;

			row1 = midR;
			col1 = midC + radius;
		}

		/// <summary>绘制圆形 ROI 和两个控制点。</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			window.DispCircle(midR, midC, radius);
			window.DispRectangle2(row1, col1, 0, 5, 5);
			window.DispRectangle2(midR, midC, 0, 5, 5);
		}

		/// <summary>计算鼠标点到圆心/边界控制点的最近距离，并激活最近控制点。</summary>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			double [] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, row1, col1); // border handle 
			val[1] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

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

		/// <summary>绘制当前激活控制点。</summary>
		public override void displayActive(HalconDotNet.HWindow window)
		{

			switch (activeHandleIdx)
			{
				case 0:
					window.DispRectangle2(row1, col1, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>把当前圆形 ROI 转换为 HALCON Circle Region。</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenCircle(midR, midC, radius);
			return region;
		}

		public override double getDistanceFromStartPoint(double row, double col)
		{
			double sRow = midR; // assumption: we have an angle starting at 0.0
			double sCol = midC + 1 * radius;

			double angle = HMisc.AngleLl(midR, midC, sRow, sCol, midR, midC, row, col);

			if (angle < 0)
				angle += 2 * Math.PI;

			return (radius * angle);
		}

		/// <summary>输出圆形 ROI 参数，顺序为中心 Row、中心 Col、半径。</summary>
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { midR, midC, radius });
		}

		/// <summary>根据激活控制点拖动圆形 ROI：边界点改变半径，圆心点整体平移。</summary>
		public override void moveByHandle(double newX, double newY)
		{
			HTuple distance;
			double shiftX,shiftY;

			switch (activeHandleIdx)
			{
				case 0: // handle at circle border

					row1 = newY;
					col1 = newX;
					HOperatorSet.DistancePp(new HTuple(row1), new HTuple(col1),
											new HTuple(midR), new HTuple(midC),
											out distance);

					radius = distance[0].D;
					break;
				case 1: // midpoint 

					shiftY = midR - newY;
					shiftX = midC - newX;

					midR = newY;
					midC = newX;

					row1 -= shiftY;
					col1 -= shiftX;
					break;
			}
		}
	}//end of class
}//end of namespace
