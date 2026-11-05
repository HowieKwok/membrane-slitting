using System;
using HalconDotNet;

namespace ViewROI
{

	/// <summary>
	/// This class implements an ROI shaped as a circular
	/// arc. ROICircularArc inherits from the base class ROI and 
	/// implements (besides other auxiliary methods) all virtual methods 
	/// defined in ROI.cs.
	/// </summary>
	/// <remarks>
	/// 圆弧 ROI 是 ViewROI 库的通用交互形状，当前涂布缺陷检测参数界面未开放。
	/// 如果后续启用，需要补充参数保存字段和检测层 Region 生成逻辑。
	/// </remarks>
	public class ROICircularArc : ROI
	{
		// handles：圆心、半径点、起始角点、终止角点。
		/// <summary>圆弧中心点坐标。</summary>
		private double midR, midC;       // 0. handle: midpoint
		/// <summary>半径控制点坐标，用于调整圆弧半径。</summary>
		private double sizeR, sizeC;     // 1. handle        
		/// <summary>起始角控制点坐标。</summary>
		private double startR, startC;   // 2. handle
		/// <summary>终止角控制点坐标。</summary>
		private double extentR, extentC; // 3. handle

		// model data to specify the arc
		/// <summary>圆弧半径，单位为像素。</summary>
		private double radius;
		/// <summary>圆弧起始角和角度范围，单位为弧度。</summary>
		private double startPhi, extentPhi; // -2*PI <= x <= 2*PI

		// display attributes
		/// <summary>圆弧 XLD 轮廓对象，用于显示。</summary>
		private HXLDCont  contour;
		/// <summary>终止方向箭头 XLD 对象，用于显示圆弧方向。</summary>
		private HXLDCont  arrowHandleXLD;
		/// <summary>HALCON 圆弧方向字符串，通常为 positive 或 negative。</summary>
		private string    circDir;
		/// <summary>2π 常量缓存。</summary>
		private double    TwoPI;
		/// <summary>π 常量缓存。</summary>
		private double    PI;


		/// <summary>创建圆弧 ROI，包含圆心、半径、起始角、终止角四个控制点。</summary>
		public ROICircularArc()
		{
			NumHandles = 4;         // midpoint handle + three handles on the arc
			activeHandleIdx = 0;
			contour = new HXLDCont();
			circDir = "";

			TwoPI = 2 * Math.PI;
			PI = Math.PI;

			arrowHandleXLD = new HXLDCont();
			arrowHandleXLD.GenEmptyObj();
		}

		/// <summary>在鼠标位置创建默认半径和默认角度范围的圆弧 ROI。</summary>
		public override void createROI(double midX, double midY)
		{
			midR = midY;
			midC = midX;

			radius = 100;

			sizeR = midR;
			sizeC = midC - radius;

			startPhi = PI * 0.25;
			extentPhi = PI * 1.5;
			circDir = "positive";

			determineArcHandles();
			updateArrowHandle();
		}

		/// <summary>绘制圆弧轮廓、圆心、半径、起始角和终止方向箭头控制点。</summary>
		/// <param name="window">HALCON window</param>
		public override void draw(HalconDotNet.HWindow window)
		{
			contour.Dispose();
			contour.GenCircleContourXld(midR, midC, radius, startPhi,
										(startPhi + extentPhi), circDir, 1.0);
			window.DispObj(contour);
			window.DispRectangle2(sizeR, sizeC, 0, 5, 5);
			window.DispRectangle2(midR, midC, 0, 5, 5);
			window.DispRectangle2(startR, startC, startPhi, 10, 2);
			window.DispObj(arrowHandleXLD);
		}

		/// <summary>计算鼠标点到四个圆弧控制点的最近距离，并激活最近控制点。</summary>
		public override double distToClosestHandle(double x, double y)
		{
			double max = 10000;
			double [] val = new double[NumHandles];

			val[0] = HMisc.DistancePp(y, x, midR, midC);       // midpoint 
			val[1] = HMisc.DistancePp(y, x, sizeR, sizeC);     // border handle 
			val[2] = HMisc.DistancePp(y, x, startR, startC);   // border handle 
			val[3] = HMisc.DistancePp(y, x, extentR, extentC); // border handle 

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
					window.DispRectangle2(midR, midC, 0, 5, 5);
					break;
				case 1:
					window.DispRectangle2(sizeR, sizeC, 0, 5, 5);
					break;
				case 2:
					window.DispRectangle2(startR, startC, startPhi, 10, 2);
					break;
				case 3:
					window.DispObj(arrowHandleXLD);
					break;
			}
		}

		/// <summary>
		/// 根据当前激活控制点拖动圆弧。
		/// 圆心点整体平移，半径点改变半径，起始/终止角点改变圆弧角度范围。
		/// </summary>
		public override void moveByHandle(double newX, double newY)
		{
			HTuple distance;
			double dirX, dirY, prior, next, valMax, valMin;

			switch (activeHandleIdx)
			{
				case 0: // midpoint 
					dirY = midR - newY;
					dirX = midC - newX;

					midR = newY;
					midC = newX;

					sizeR -= dirY;
					sizeC -= dirX;

					determineArcHandles();
					break;

				case 1: // handle at circle border                  
					sizeR = newY;
					sizeC = newX;

					HOperatorSet.DistancePp(new HTuple(sizeR), new HTuple(sizeC),
											new HTuple(midR), new HTuple(midC), out distance);
					radius = distance[0].D;
					determineArcHandles();
					break;

				case 2: // start handle for arc                
					dirY = newY - midR;
					dirX = newX - midC;

					startPhi = Math.Atan2(-dirY, dirX);

					if (startPhi < 0)
						startPhi = PI + (startPhi + PI);

					setStartHandle();
					prior = extentPhi;
					extentPhi = HMisc.AngleLl(midR, midC, startR, startC, midR, midC, extentR, extentC);

					if (extentPhi < 0 && prior > PI * 0.8)
						extentPhi = (PI + extentPhi) + PI;
					else if (extentPhi > 0 && prior < -PI * 0.7)
						extentPhi = -PI - (PI - extentPhi);

					break;

				case 3: // end handle for arc
					dirY = newY - midR;
					dirX = newX - midC;

					prior = extentPhi;
					next = Math.Atan2(-dirY, dirX);

					if (next < 0)
						next = PI + (next + PI);

					if (circDir == "positive" && startPhi >= next)
						extentPhi = (next + TwoPI) - startPhi;
					else if (circDir == "positive" && next > startPhi)
						extentPhi = next - startPhi;
					else if (circDir == "negative" && startPhi >= next)
						extentPhi = -1.0 * (startPhi - next);
					else if (circDir == "negative" && next > startPhi)
						extentPhi = -1.0 * (startPhi + TwoPI - next);

					valMax = Math.Max(Math.Abs(prior), Math.Abs(extentPhi));
					valMin = Math.Min(Math.Abs(prior), Math.Abs(extentPhi));

					if ((valMax - valMin) >= PI)
						extentPhi = (circDir == "positive") ? -1.0 * valMin : valMin;

					setExtentHandle();
					break;
			}

			circDir = (extentPhi < 0) ? "negative" : "positive";
			updateArrowHandle();
		}

		/// <summary>把圆弧 XLD 轮廓转换为 HALCON Region。</summary>
		public override HRegion getRegion()
		{
			HRegion region;
			contour.Dispose();
			contour.GenCircleContourXld(midR, midC, radius, startPhi, (startPhi + extentPhi), circDir, 1.0);
			region = new HRegion(contour);
			return region;
		}

		/// <summary>输出圆弧 ROI 模型参数，顺序为中心 Row/Col、半径、起始角、角度范围。</summary>
		public override HTuple getModelData()
		{
			return new HTuple(new double[] { midR, midC, radius, startPhi, extentPhi });
		}

		/// <summary>根据当前半径、起始角和范围角重算起始/终止控制点坐标。</summary>
		private void determineArcHandles()
		{
			setStartHandle();
			setExtentHandle();
		}

		/// <summary>重算圆弧起始角控制点坐标。</summary>
		private void setStartHandle()
		{
			startR = midR - radius * Math.Sin(startPhi);
			startC = midC + radius * Math.Cos(startPhi);
		}

		/// <summary>重算圆弧终止角控制点坐标。</summary>
		private void setExtentHandle()
		{
			extentR = midR - radius * Math.Sin(startPhi + extentPhi);
			extentC = midC + radius * Math.Cos(startPhi + extentPhi);
		}

		/// <summary>根据圆弧终止点和方向重建箭头 XLD 控制点。</summary>
		private void updateArrowHandle()
		{
			double row1, col1, row2, col2;
			double rowP1, colP1, rowP2, colP2;
			double length,dr,dc, halfHW, sign, angleRad;
			double headLength = 15;
			double headWidth  = 15;

			arrowHandleXLD.Dispose();
			arrowHandleXLD.GenEmptyObj();

			row2 = extentR;
			col2 = extentC;
			angleRad = (startPhi + extentPhi) + Math.PI * 0.5;

			sign = (circDir == "negative") ? -1.0 : 1.0;
			row1 = row2 + sign * Math.Sin(angleRad) * 20;
			col1 = col2 - sign * Math.Cos(angleRad) * 20;

			length = HMisc.DistancePp(row1, col1, row2, col2);
			if (length == 0)
				length = -1;

			dr = (row2 - row1) / length;
			dc = (col2 - col1) / length;

			halfHW = headWidth / 2.0;
			rowP1 = row1 + (length - headLength) * dr + halfHW * dc;
			rowP2 = row1 + (length - headLength) * dr - halfHW * dc;
			colP1 = col1 + (length - headLength) * dc - halfHW * dr;
			colP2 = col1 + (length - headLength) * dc + halfHW * dr;

			if (length == -1)
				arrowHandleXLD.GenContourPolygonXld(row1, col1);
			else
				arrowHandleXLD.GenContourPolygonXld(new HTuple(new double[] { row1, row2, rowP1, row2, rowP2, row2 }),
					new HTuple(new double[] { col1, col2, colP1, col2, colP2, col2 }));
		}

	}//end of class
}//end of namespace

