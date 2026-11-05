using System;
using System.Collections;
using HalconDotNet;



namespace ViewROI
{
	/// <summary>图形上下文异常或状态通知委托。</summary>
	public delegate void GCDelegate(string val);

    /// <summary>
    /// HALCON 图形显示上下文。
    /// 用 Hashtable 保存颜色、线宽、绘制模式等 dev_set_* 风格的显示参数，
    /// HWndCtrl 绘制图像和 ROI 叠加对象前会应用这些参数。
	/// </summary>
	public class GraphicsContext
	{

		/// <summary>单色显示模式键，对应 HALCON set_color/dev_set_color。</summary>
		public const string GC_COLOR	 = "Color";

		/// <summary>多色显示模式键，对应 HALCON set_colored/dev_set_colored。</summary>
		public const string GC_COLORED	 = "Colored";

		/// <summary>线宽显示模式键，对应 HALCON set_line_width。</summary>
		public const string GC_LINEWIDTH = "LineWidth";

		/// <summary>绘制模式键，对应 HALCON set_draw，例如 margin/fill。</summary>
		public const string GC_DRAWMODE  = "DrawMode";

		/// <summary>形状显示模式键，对应 HALCON set_shape。</summary>
		public const string GC_SHAPE     = "Shape";

		/// <summary>LUT 查找表显示模式键，对应 HALCON set_lut。</summary>
		public const string GC_LUT       = "Lut";

		/// <summary>图像绘制模式键，对应 HALCON set_paint。</summary>
		public const string GC_PAINT     = "Paint";

		/// <summary>线型模式键，对应 HALCON set_line_style。</summary>
		public const string GC_LINESTYLE = "LineStyle";

		/// <summary>当前对象待应用的图形设置表，键为 GC_* 常量。</summary>
		private Hashtable		graphicalSettings;

		/// <summary>上一次已经应用到窗口的图形设置缓存，用于避免重复设置。</summary>
		public	Hashtable		stateOfSettings;

		/// <summary>遍历图形设置 Hashtable 时使用的迭代器。</summary>
		private IEnumerator iterator;

		/// <summary>图形上下文异常通知回调。</summary>
		public GCDelegate   gcNotification;


		/// <summary>创建空图形上下文。</summary>
		public GraphicsContext()
		{
			graphicalSettings = new Hashtable(10, 0.2f);
			gcNotification = new GCDelegate(dummy);
			stateOfSettings = new Hashtable(10, 0.2f);
		}


		/// <summary>使用已有设置表创建图形上下文。</summary>
		/// <param name="settings"> 
		/// List of modes, which describes the graphical context 
		/// </param>
		public GraphicsContext(Hashtable settings)
		{
			graphicalSettings = settings;
			gcNotification = new GCDelegate(dummy);
			stateOfSettings = new Hashtable(10, 0.2f);
		}

		/// <summary>
		/// 将图形上下文应用到 HALCON 窗口。
		/// 只在设置变化时调用对应 SetColor/SetDraw/SetLineWidth 等接口，减少重复设置。
		/// </summary>
		/// <param name="window">当前 HALCON 窗口。</param>
		/// <param name="cContext">需要应用到窗口的图形设置表。</param>
		public void applyContext(HWindow window, Hashtable cContext)
		{
			string key  = "";
			string valS = "";
			int    valI = -1;
			HTuple valH = null;

			iterator = cContext.Keys.GetEnumerator();

			try
			{
				while (iterator.MoveNext())
				{

					key = (string)iterator.Current;

					if (stateOfSettings.Contains(key) &&
						stateOfSettings[key] == cContext[key])
						continue;

					switch (key)
					{
						case GC_COLOR:
							valS = (string)cContext[key];
							window.SetColor(valS);
							if (stateOfSettings.Contains(GC_COLORED))
								stateOfSettings.Remove(GC_COLORED);

							break;
						case GC_COLORED:
							valI = (int)cContext[key];
							window.SetColored(valI);

							if (stateOfSettings.Contains(GC_COLOR))
								stateOfSettings.Remove(GC_COLOR);

							break;
						case GC_DRAWMODE:
							valS = (string)cContext[key];
							window.SetDraw(valS);
							break;
						case GC_LINEWIDTH:
							valI = (int)cContext[key];
							window.SetLineWidth(valI);
							break;
						case GC_LUT:
							valS = (string)cContext[key];
							window.SetLut(valS);
							break;
						case GC_PAINT:
							valS = (string)cContext[key];
							window.SetPaint(valS);
							break;
						case GC_SHAPE:
							valS = (string)cContext[key];
							window.SetShape(valS);
							break;
						case GC_LINESTYLE:
							valH = (HTuple)cContext[key];
							window.SetLineStyle(valH);
							break;
						default:
							break;
					}


					if (valI != -1)
					{
						if (stateOfSettings.Contains(key))
							stateOfSettings[key] = valI;
						else
							stateOfSettings.Add(key, valI);

						valI = -1;
					}
					else if (valS != "")
					{
						if (stateOfSettings.Contains(key))
							stateOfSettings[key] = valI;
						else
							stateOfSettings.Add(key, valI);

						valS = "";
					}
					else if (valH != null)
					{
						if (stateOfSettings.Contains(key))
							stateOfSettings[key] = valI;
						else
							stateOfSettings.Add(key, valI);

						valH = null;
					}
				}//while
			}
			catch (HOperatorException e)
			{
				gcNotification(e.Message);
				return;
			}
		}


		/// <summary>设置单色显示颜色，并移除多色模式。</summary>
		/// <param name="val"> 
		/// A single color, e.g. "blue", "green" ...etc. 
		/// </param>
		public void setColorAttribute(string val)
		{
			if (graphicalSettings.ContainsKey(GC_COLORED))
				graphicalSettings.Remove(GC_COLORED);

			addValue(GC_COLOR, val);
		}

		/// <summary>设置多色显示模式，并移除单色模式。</summary>
		/// <param name="val"> 
		/// The colored mode, which can be either "colored3" or "colored6"
		/// or "colored12" 
		/// </param>
		public void setColoredAttribute(int val)
		{
			if (graphicalSettings.ContainsKey(GC_COLOR))
				graphicalSettings.Remove(GC_COLOR);

			addValue(GC_COLORED, val);
		}

		/// <summary>设置绘制模式，例如 margin 或 fill。</summary>
		/// <param name="val"> 
		/// One of the possible draw modes: "margin" or "fill" 
		/// </param>
		public void setDrawModeAttribute(string val)
		{
			addValue(GC_DRAWMODE, val);
		}

		/// <summary>设置线宽。</summary>
		/// <param name="val"> 
		/// The line width, which can range from 1 to 50 
		/// </param>
		public void setLineWidthAttribute(int val)
		{
			addValue(GC_LINEWIDTH, val);
		}

		/// <summary>设置 LUT 查找表模式。</summary>
		/// <param name="val"> 
		/// One of the possible modes of look up tables. For 
		/// further information on particular setups, please refer to the
		/// Reference Manual entry of the operator set_lut.
		/// </param>
		public void setLutAttribute(string val)
		{
			addValue(GC_LUT, val);
		}


		/// <summary>设置图像 paint 显示模式。</summary>
		/// <param name="val"> 
		/// One of the possible paint modes. For further 
		/// information on particular setups, please refer refer to the
		/// Reference Manual entry of the operator set_paint.
		/// </param>
		public void setPaintAttribute(string val)
		{
			addValue(GC_PAINT, val);
		}


		/// <summary>设置形状显示模式。</summary>
		/// <param name="val">
		/// One of the possible shape modes. For further 
		/// information on particular setups, please refer refer to the
		/// Reference Manual entry of the operator set_shape.
		/// </param>
		public void setShapeAttribute(string val)
		{
			addValue(GC_SHAPE, val);
		}

		/// <summary>设置线型，例如实线或虚线。</summary>
		/// <param name="val"> 
		/// HALCON set_line_style 使用的线型参数。
		/// </param>
		public void setLineStyleAttribute(HTuple val)
		{
			addValue(GC_LINESTYLE, val);
		}

		/// <summary>写入 int 类型图形设置值。</summary>
		/// <param name="key"> 
		/// GC_* 常量定义的图形设置键。
		/// </param>
		/// <param name="val"> 
		/// int 类型图形设置值。
		/// </param>
		private void addValue(string key, int val)
		{
			if (graphicalSettings.ContainsKey(key))
				graphicalSettings[key] = val;
			else
				graphicalSettings.Add(key, val);
		}

		/// <summary>写入 string 类型图形设置值。</summary>
		/// <param name="key"> 
		/// GC_* 常量定义的图形设置键。
		/// </param>
		/// <param name="val"> 
		/// string 类型图形设置值。
		/// </param>
		private void addValue(string key, string val)
		{
			if (graphicalSettings.ContainsKey(key))
				graphicalSettings[key] = val;
			else
				graphicalSettings.Add(key, val);
		}


		/// <summary>写入 HTuple 类型图形设置值。</summary>
		/// <param name="key">
		/// GC_* 常量定义的图形设置键。
		/// </param>
		/// <param name="val"> 
		/// HTuple 类型图形设置值。
		/// </param>
		private void addValue(string key, HTuple val)
		{
			if (graphicalSettings.ContainsKey(key))
				graphicalSettings[key] = val;
			else
				graphicalSettings.Add(key, val);
		}

		/// <summary>清空当前图形设置表。</summary>
		public void clear()
		{
			graphicalSettings.Clear();
		}


		/// <summary>复制当前图形上下文。</summary>
		public GraphicsContext copy()
		{
			return new GraphicsContext((Hashtable)this.graphicalSettings.Clone());
		}


		/// <summary>按键读取某项图形设置；不存在时返回 null。</summary>
		/// <param name="key"> 
		/// GC_* 常量定义的图形设置键。
		/// </param>
		public object getGraphicsAttribute(string key)
		{
			if (graphicalSettings.ContainsKey(key))
				return graphicalSettings[key];

			return null;
		}

		/// <summary>复制当前图形设置表，供 HObjectEntry 保存显示上下文。</summary>
		/// <returns>当前图形上下文设置表的副本。</returns>
		public Hashtable copyContextList()
		{
			return (Hashtable)graphicalSettings.Clone();
		}


		/// <summary>默认空通知回调，避免外部没有订阅时出现空引用。</summary>
		public void dummy(string val) { }

	}//end of class
}//end of namespace
