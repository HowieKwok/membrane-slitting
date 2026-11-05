using System;
using HalconDotNet;

namespace ViewROI
{
	/// <summary>
	/// This class demonstrates one of the possible implementations for a 
	/// linear ROI. ROILine inherits from the base class ROI and 
	/// implements (besides other auxiliary methods) all virtual methods 
	/// defined in ROI.cs.
	/// </summary>
	/// <remarks>
	/// 线段 ROI 是 ViewROI 库保留的通用形状，当前涂布缺陷检测界面不作为检测 ROI 参数保存。
	/// 如后续启用，必须明确它和检测层 Region/测量逻辑的对应关系。
	/// </remarks>
	public class ROILine : ROI
	{

		/// <summary>线段起点坐标，row 为行坐标，col 为列坐标。</summary>
		private double row1, col1;   // first end point of line
		/// <summary>线段终点坐标，箭头手柄绘制在该端。</summary>
		private double row2, col2;   // second end point of line
		/// <summary>线段中点坐标，用于整体平移。</summary>
		private double midR, midC;   // midPoint of line

		/// <summary>终点箭头 XLD 控制点，用于显示线段方向。</summary>
		private HXLDCont arrowHandleXLD;

		/// <summary>创建线段 ROI，三个控制点分别为起点、终点和中点。</summary>
		public ROILine()
		{
			NumHandles = 3;        // two end points of line
			activeHandleIdx = 2;
			arrowHandleXLD = new HXLDCont();
			arrowHandleXLD.GenEmptyObj();
		}


		/// <summary>在鼠标位置创建默认水平线段 ROI。</summary>
		public override void createROI(double midX, double midY)
		{
			midR = midY;
			midC = midX;

			row1 = midR;
			col1 = midC - 50;
			row2 = midR;
			col2 = midC + 50;

			updateArrowHandle();
		}

		/// <summary>绘制线段、起点手柄、终点箭头手柄和中点手柄。</summary>
		public override void draw(HalconDotNet.HWindow window)
		{

			window.DispLine(row1, col1, row2, col2);

			window.DispRectangle2(row1, col1, 0, 5, 5);
			window.DispObj(arrowHandleXLD);  //window.DispRectangle2( row2, col2, 0, 5, 5);
			window.DispRectangle2(midR, midC, 0, 5, 5);
		}

		/// <summary>计算鼠标点到起点、终点和中点三个控制点的最近距离，并激活最近控制点。</summary>
		public override double distToClosestHandle(double x, double y)
		{

			double max = 10000;
			double [] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, row1, col1); // upper left 
			val[1] = HMisc.DistancePp(y, x, row2, col2); // upper right 
			val[2] = HMisc.DistancePp(y, x, midR, midC); // midpoint 

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
					window.DispObj(arrowHandleXLD); //window.DispRectangle2(row2, col2, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
			}
		}

		/// <summary>把当前线段 ROI 转换为 HALCON 线 Region。</summary>
		public override HRegion getRegion()
		{
			HRegion region = new HRegion();
			region.GenRegionLine(row1, col1, row2, col2);
			return region;
		}

		/// <summary>计算指定点到线段起点的直线距离。</summary>
		public override double getDistanceFromStartPoint(double row, double col)
		{
			double distance = HMisc.DistancePp(row, col, row1, col1);
			return distance;
		}
		/// <summary>输出线段 ROI 模型参数，顺序为起点 Row/Col、终点 Row/Col。</summary>
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { row1, col1, row2, col2 });
		}

		/// <summary>根据当前激活控制点拖动线段：端点改变线段方向和长度，中点整体平移。</summary>
		public override void moveByHandle(double newX, double newY)
		{
			double lenR, lenC;

			switch (activeHandleIdx)
			{
				case 0: // first end point
					row1 = newY;
					col1 = newX;

					midR = (row1 + row2) / 2;
					midC = (col1 + col2) / 2;
					break;
				case 1: // last end point
					row2 = newY;
					col2 = newX;

					midR = (row1 + row2) / 2;
					midC = (col1 + col2) / 2;
					break;
				case 2: // midpoint 
					lenR = row1 - midR;
					lenC = col1 - midC;

					midR = newY;
					midC = newX;

					row1 = midR + lenR;
					col1 = midC + lenC;
					row2 = midR - lenR;
					col2 = midC - lenC;
					break;
			}
			updateArrowHandle();
		}


		/// <summary>根据线段终点方向重建箭头 XLD 控制点。</summary>
		private void updateArrowHandle()
		{
			double length,dr,dc, halfHW;
			double rrow1, ccol1,rowP1, colP1, rowP2, colP2;

			double headLength = 15;
			double headWidth  = 15;


			arrowHandleXLD.Dispose();
			arrowHandleXLD.GenEmptyObj();

			rrow1 = row1 + (row2 - row1) * 0.8;
			ccol1 = col1 + (col2 - col1) * 0.8;

			length = HMisc.DistancePp(rrow1, ccol1, row2, col2);
			if (length == 0)
				length = -1;

			dr = (row2 - rrow1) / length;
			dc = (col2 - ccol1) / length;

			halfHW = headWidth / 2.0;
			rowP1 = rrow1 + (length - headLength) * dr + halfHW * dc;
			rowP2 = rrow1 + (length - headLength) * dr - halfHW * dc;
			colP1 = ccol1 + (length - headLength) * dc - halfHW * dr;
			colP2 = ccol1 + (length - headLength) * dc + halfHW * dr;

			if (length == -1)
				arrowHandleXLD.GenContourPolygonXld(rrow1, ccol1);
			else
				arrowHandleXLD.GenContourPolygonXld(new HTuple(new double[] { rrow1, row2, rowP1, row2, rowP2, row2 }),
													new HTuple(new double[] { ccol1, col2, colP1, col2, colP2, col2 }));
		}

	}//end of class
}//end of namespace
