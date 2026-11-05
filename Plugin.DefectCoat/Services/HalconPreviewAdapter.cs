using HalconDotNet;
using System;
using ViewROI;

namespace Plugin.DefectCoat.Services
{
    /// <summary>
    /// HALCON 预览窗口适配器。
    /// 用于 NG Region、小图、预处理图等只读预览窗口；本类会 CopyObj 保存显示副本，
    /// 因此 Dispose/Clear 只释放自己复制出来的显示对象，不释放外部 Params 或检测结果持有的原始 HObject。
    /// </summary>
    public sealed class HalconPreviewAdapter : IDisposable
    {
        /// <summary>承载预览图的 WinForms HALCON 控件，由 WPF WindowsFormsHost 持有。</summary>
        private readonly HWindowControl window;

        /// <summary>ViewROI 提供的窗口控制器，这里只用图像缩放/平移能力，不启用 ROI 编辑。</summary>
        private readonly HWndCtrl viewCtrl;

        /// <summary>重绘回调委托，必须保存字段以便 Dispose 时准确退订。</summary>
        private readonly HWndCtrl.ReDrawDelegate redrawHandler;

        /// <summary>鼠标移动回调委托，必须保存字段以便 Dispose 时准确退订。</summary>
        private readonly HMouseEventHandler mouseMoveHandler;

        /// <summary>当前预览图像副本，由本适配器负责 Dispose。</summary>
        private HObject image;

        /// <summary>蓝色叠加区域副本，通常用于检测 ROI 或辅助区域显示。</summary>
        private HObject overlayBlue;

        /// <summary>黄色叠加区域副本，预留给调试区域显示。</summary>
        private HObject overlayYellow;

        /// <summary>红色叠加区域副本，通常用于已分类或主要 NG 区域显示。</summary>
        private HObject overlayRed;

        /// <summary>绿色叠加区域副本，通常用于未分类区域显示。</summary>
        private HObject overlayGreen;

        /// <summary>橙色叠加区域副本，通常用于命中类别但尺寸未达要求区域显示。</summary>
        private HObject overlayOrange;

        /// <summary>释放标记，防止重复退订事件或重复释放 HObject。</summary>
        private bool disposed;

        /// <summary>
        /// 创建预览窗口适配器。
        /// 该构造函数只绑定显示控件和事件，不加载图像，也不接管外部 HObject 生命周期。
        /// </summary>
        public HalconPreviewAdapter(HWindowControl windowControl)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            viewCtrl = new HWndCtrl(window);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            viewCtrl.setDispLevel(HWndCtrl.MODE_VIEW_NONE);
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            redrawHandler = OnRedraw;
            mouseMoveHandler = OnMouseMove;
            viewCtrl.ReDrawEvent += redrawHandler;
            window.HMouseMove += mouseMoveHandler;
        }

        /// <summary>鼠标移动事件，向 ViewModel 输出当前 Row/Col/Gray 文本。</summary>
        public event Action<HalconMouseInfoArgs> MouseMoved;

        /// <summary>
        /// 显示预览图像。
        /// 传入的 hoImage 会被 CopyObj 成本类私有副本，外部仍然负责自己的 HObject 生命周期。
        /// </summary>
        public void ShowImage(HObject hoImage, bool fit)
        {
            if (disposed) return;
            ReplaceObject(ref image, hoImage);
            if (!HasObject(image))
            {
                Clear();
                return;
            }

           
            viewCtrl.addIconicVar(image);
            viewCtrl.resetflag = fit;
            viewCtrl.resetAll();
            viewCtrl.repaint();
        }

        /// <summary>
        /// 设置预览叠加区域。
        /// 所有输入区域都会复制成显示副本，颜色由 OnRedraw 中固定绘制。
        /// </summary>
        public void ShowOverlay(HObject blue, HObject yellow, HObject red, HObject green, HObject orange)
        {
            ReplaceObject(ref overlayBlue, blue);
            ReplaceObject(ref overlayYellow, yellow);
            ReplaceObject(ref overlayRed, red);
            ReplaceObject(ref overlayGreen, green);
            ReplaceObject(ref overlayOrange, orange);
            Repaint();
        }

        /// <summary>清空预览图像和叠加区域，并释放本适配器持有的显示副本。</summary>
        public void Clear()
        {
            if (disposed) return;
            DisposeObject(ref image);
            DisposeObject(ref overlayBlue);
            DisposeObject(ref overlayYellow);
            DisposeObject(ref overlayRed);
            DisposeObject(ref overlayGreen);
            DisposeObject(ref overlayOrange);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            try
            {
                window.HalconWindow.ClearWindow();
            }
            catch
            {
            }
        }

        /// <summary>请求 ViewROI 控制器重绘当前图像和叠加区域。</summary>
        public void Repaint()
        {
            if (disposed) return;
            viewCtrl.repaint();
        }

        /// <summary>退订事件并释放所有显示副本；不释放外部传入的原始 HObject。</summary>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                viewCtrl.ReDrawEvent -= redrawHandler;
                window.HMouseMove -= mouseMoveHandler;
            }
            catch
            {
            }
            DisposeObject(ref image);
            DisposeObject(ref overlayBlue);
            DisposeObject(ref overlayYellow);
            DisposeObject(ref overlayRed);
            DisposeObject(ref overlayGreen);
            DisposeObject(ref overlayOrange);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
        }

        /// <summary>ViewROI 重绘图像后补画业务叠加区域。绘制顺序决定重叠时红色区域最后显示。</summary>
        private void OnRedraw()
        {
            try
            {
                DrawOverlay(overlayBlue, "blue", 2);
                DrawOverlay(overlayYellow, "yellow", 2);
                DrawOverlay(overlayGreen, "green", 3);
                DrawOverlay(overlayOrange, "orange", 3);
                DrawOverlay(overlayRed, "red", 3);
            }
            catch
            {
            }
        }

        /// <summary>按指定颜色和线宽绘制一个区域叠加；空对象直接跳过。</summary>
        private void DrawOverlay(HObject region, string color, int lineWidth)
        {
            if (!HasObject(region)) return;
            window.HalconWindow.SetDraw("margin");
            window.HalconWindow.SetLineWidth(lineWidth);
            window.HalconWindow.SetColor(color);
            window.HalconWindow.DispObj(region);
        }

        /// <summary>鼠标移动时计算图像坐标和灰度，封装成 HalconMouseInfoArgs 交给 ViewModel。</summary>
        private void OnMouseMove(object sender, HMouseEventArgs e)
        {
            MouseMoved?.Invoke(new HalconMouseInfoArgs
            {
                Row = e.Y,
                Column = e.X,
                Text = BuildMouseText(e.Y, e.X)
            });
        }

        /// <summary>
        /// 构造状态栏文本。
        /// 多通道图像只读取第 1 通道灰度；临时 AccessChannel 结果在 finally 中释放。
        /// </summary>
        private string BuildMouseText(double rowValue, double colValue)
        {
            if (!HasObject(image)) return "图像:-  行:--  列:--  灰度:-";
            HObject channel = null;
            try
            {
                int row = (int)Math.Round(rowValue);
                int col = (int)Math.Round(colValue);
                HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
                if (row < 0 || col < 0 || row >= height.I || col >= width.I)
                    return "图像:-  行:--  列:--  灰度:-";

                HObject read = image;
                HOperatorSet.CountChannels(image, out HTuple channels);
                if (channels.I > 1)
                {
                    HOperatorSet.AccessChannel(image, out channel, 1);
                    read = channel;
                }
                HOperatorSet.GetGrayval(read, row, col, out HTuple gray);
                return string.Format("图像:-  行:{0}  列:{1}  灰度:{2}", row, col, gray);
            }
            catch
            {
                return "图像:-  行:--  列:--  灰度:-";
            }
            finally
            {
                channel?.Dispose();
            }
        }

        /// <summary>判断 HALCON 对象是否已初始化且至少包含一个对象，防止空对象绘制导致异常。</summary>
        private static bool HasObject(HObject obj)
        {
            if (obj == null || !obj.IsInitialized()) return false;
            try
            {
                HOperatorSet.CountObj(obj, out HTuple count);
                return count.I > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>用 source 的 CopyObj 替换 target；target 旧副本会先释放。</summary>
        private static void ReplaceObject(ref HObject target, HObject source)
        {
            DisposeObject(ref target);
            if (HasObject(source))
                target = source.CopyObj(1, -1);
        }

        /// <summary>安全释放一个本类持有的 HObject，并把引用置空。</summary>
        private static void DisposeObject(ref HObject obj)
        {
            try
            {
                obj?.Dispose();
            }
            catch
            {
            }
            obj = null;
        }
    }
}
