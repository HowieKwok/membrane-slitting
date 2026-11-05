using System;
using HalconDotNet;


namespace ViewROI
{

    /// <summary>
    /// ROI 抽象基类。
    /// 子类负责实现具体形状的创建、绘制、拖拽、Region 输出和模型参数输出；
    /// ROIController 只通过这些虚方法管理不同形状的 ROI。
    /// </summary>    
    public class ROI
	{

        /// <summary>ROI 控制点数量，由具体 ROI 子类在构造函数中设置。</summary>
        protected int   NumHandles;
        /// <summary>当前激活控制点序号，鼠标拖动时由子类 distToClosestHandle 更新。</summary>
		protected int	activeHandleIdx;

        /// <summary>
        /// ROI 正负标记。正 ROI 参与区域并集，负 ROI 参与区域扣除。
        /// </summary>
        protected int     OperatorFlag;

        /// <summary>当前 ROI 绘制线型；负 ROI 默认用虚线。</summary>
        public HTuple     flagLineStyle;

		/// <summary>正 ROI 标记，参与最终区域并集。</summary>
		public const int  POSITIVE_FLAG	= ROIController.MODE_ROI_POS;

		/// <summary>负 ROI 标记，参与最终区域扣除。</summary>
		public const int  NEGATIVE_FLAG	= ROIController.MODE_ROI_NEG;

		/// <summary>线段 ROI 类型编号。</summary>
		public const int  ROI_TYPE_LINE       = 10;
		/// <summary>圆形 ROI 类型编号。</summary>
		public const int  ROI_TYPE_CIRCLE     = 11;
		/// <summary>圆弧 ROI 类型编号。</summary>
		public const int  ROI_TYPE_CIRCLEARC  = 12;
		/// <summary>Rectangle1 ROI 类型编号。</summary>
		public const int  ROI_TYPE_RECTANCLE1 = 13;
		/// <summary>Rectangle2 ROI 类型编号。</summary>
		public const int  ROI_TYPE_RECTANGLE2 = 14;


		/// <summary>正 ROI 绘制线型，默认实线。</summary>
		protected HTuple  posOperation = new HTuple();
		/// <summary>负 ROI 绘制线型，默认虚线。</summary>
		protected HTuple  negOperation = new HTuple(new int[] { 2, 2 });

		/// <summary>构造 ROI 基类，具体几何参数由子类保存。</summary>
		public ROI() { }

        /// <summary>按鼠标点击位置创建 ROI 初始形状。</summary>
        /// <param name="midX">
        /// x (=column) coordinate for ROI
        /// </param>
        /// <param name="midY">
        /// y (=row) coordinate for ROI
        /// </param>
        public virtual void createROI(double midX, double midY) { }

        /// <summary>把 ROI 轮廓绘制到 HALCON 窗口。</summary>
        /// <param name="window">HALCON window</param>
        public virtual void draw(HalconDotNet.HWindow window) { }

        /// <summary> 
        /// 计算鼠标位置到最近控制点的距离，用于判断点击命中了哪个 ROI。
        /// </summary>
        /// <param name="x">x (=column) coordinate</param>
        /// <param name="y">y (=row) coordinate</param>
        /// <returns> 
        /// Distance of the closest ROI handle.
        /// </returns>
        public virtual double distToClosestHandle(double x, double y)
		{
			return 0.0;
		}

        /// <summary> 
        /// 绘制当前激活控制点，通常用红色手柄显示。
        /// </summary>
        /// <param name="window">HALCON window</param>
        public virtual void displayActive(HalconDotNet.HWindow window) { }

        /// <summary> 
        /// 拖动当前激活控制点，并根据鼠标图像坐标更新 ROI 几何参数。
        /// </summary>
        /// <param name="x">x (=column) coordinate</param>
        /// <param name="y">y (=row) coordinate</param>
        public virtual void moveByHandle(double x, double y) { }

        /// <summary>将当前 ROI 转换为 HALCON Region。</summary>
        public virtual HRegion getRegion()
		{
			return null;
		}

		/// <summary>计算指定点到 ROI 起始点的沿线距离，线段/圆弧类会重写该方法。</summary>
		public virtual double getDistanceFromStartPoint(double row, double col)
		{
			return 0.0;
		}
		/// <summary>
		/// 输出 ROI 模型参数，供外部保存或显示。
		/// </summary> 
		public virtual HTuple getModelData()
		{
			return null;
		}

		/// <summary>Number of handles defined for the ROI.</summary>
		/// <returns>ROI 控制点数量。</returns>
		public int getNumHandles()
		{
			return NumHandles;
		}

		/// <summary>获取当前激活控制点序号。</summary>
		/// <returns>Index of the active handle (from the handle list)</returns>
		public int getActHandleIdx()
		{
			return activeHandleIdx;
		}

        /// <summary>
        /// 获取 ROI 正负标记。
        /// </summary>
        public int getOperatorFlag()
		{
			return OperatorFlag;
		}

        /// <summary>
        /// 设置 ROI 正负标记，并同步绘制线型。
        /// The sign is used when creating a model region for matching
        /// applications by summing up all positive and negative ROI models
        /// created so far.
        /// </summary>
        /// <param name="flag">Sign of ROI object</param>
        public void setOperatorFlag(int flag)
		{
			OperatorFlag = flag;

			switch (OperatorFlag)
			{
				case ROI.POSITIVE_FLAG:
					flagLineStyle = posOperation;
					break;
				case ROI.NEGATIVE_FLAG:
					flagLineStyle = negOperation;
					break;
				default:
					flagLineStyle = posOperation;
					break;
			}
		}
	}//end of class
}//end of namespace
