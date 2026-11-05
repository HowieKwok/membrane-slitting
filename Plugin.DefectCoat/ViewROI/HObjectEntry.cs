using System;
using HalconDotNet;
using System.Collections;



namespace ViewROI
{

    /// <summary>
    /// HALCON 对象和显示上下文的绑定项。
    /// HWndCtrl 将图像、Region、XLD 等对象放入显示栈时，
    /// 用该类同时保存对象本身和绘制它需要的颜色、线宽、填充方式等参数。
    /// </summary>
	public class HObjectEntry
	{
		/// <summary>该 HObject 对应的图形上下文，例如颜色、线宽、绘制模式等。</summary>
		public Hashtable	gContext;

		/// <summary>需要显示的 HALCON 对象，可以是图像、Region 或 XLD。</summary>
		public HObject		HObj;



		/// <summary>创建一个 HALCON 显示对象绑定项。</summary>
		/// <param name="obj">
		/// HALCON object that is linked to the graphical context gc. 
		/// </param>
		/// <param name="gc"> 
		/// Hashlist of graphical states that are applied before the object
		/// is displayed. 
		/// </param>
		public HObjectEntry(HObject obj, Hashtable gc)
		{
			gContext = gc;
			HObj = obj;
		}

		/// <summary>清空图形上下文并释放当前 HALCON 对象。</summary>
		public void clear()
		{
			gContext.Clear();
			HObj.Dispose();
		}

	}//end of class
}//end of namespace
