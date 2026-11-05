using System;
using System.Globalization;
using System.Windows.Forms;
using HalconDotNet;
using ViewROI;

namespace Plugin.CropPart.Services
{
    public sealed class HalconImageMouseInfoEventArgs : EventArgs
    {
        public HalconImageMouseInfoEventArgs(string sizeText, string positionText)
        {
            SizeText = sizeText;
            PositionText = positionText;
        }

        public string SizeText { get; private set; }
        public string PositionText { get; private set; }
    }

    public sealed class CropPartHalconViewAdapter : IDisposable
    {
        private readonly HWindowControl window;
        private readonly HWndCtrl viewCtrl;
        private HImage displayImage;
        private bool disposed;
        private string sizeText = "Size  R:-   C:-";
        private string positionText = "R:-  C:-  Value:-";

        public event EventHandler<HalconImageMouseInfoEventArgs> MouseInfoChanged;

        public CropPartHalconViewAdapter(HWindowControl windowControl)
        {
            window = windowControl ?? throw new ArgumentNullException(nameof(windowControl));
            window.Dock = DockStyle.Fill;
            viewCtrl = new HWndCtrl(window);
            viewCtrl.ho_Image?.Dispose();
            viewCtrl.ho_Image_Changed?.Dispose();
            viewCtrl.ho_Image = null;
            viewCtrl.ho_Image_Changed = null;
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            viewCtrl.setViewState(HWndCtrl.MODE_VIEW_MOVE);
          
            window.HMouseMove += Window_HMouseMove;
        }

        public HWindowControl WindowControl
        {
            get { return window; }
        }

        public void ShowImage(HObject image, bool fit)
        {
            if (disposed) return;

            try
            {
                if (image == null || !image.IsInitialized())
                {
                    Clear();
                    return;
                }

                ReleaseDisplayImage();
                displayImage = new HImage();
                HobjectToHimage(image, ref displayImage);
                viewCtrl.addIconicVar(displayImage);
                RefreshSizeText();
                positionText = "R:-  C:-  Value:-";
                RaiseMouseInfoChanged();
                if (fit) viewCtrl.resetAll();
                viewCtrl.repaint();
            }
            catch
            {
                Clear();
            }
        }

        public void Fit()
        {
            if (disposed) return;
            try
            {
                viewCtrl.resetAll();
                viewCtrl.repaint();
            }
            catch
            {
            }
        }

        public void Clear()
        {
            if (disposed) return;
            try
            {
                HOperatorSet.ClearWindow(window.HalconWindow);
                ReleaseDisplayImage();
                sizeText = "Size  R:-   C:-";
                positionText = "R:-  C:-  Value:-";
                RaiseMouseInfoChanged();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
  
            try
            {
                viewCtrl.ho_Image = null;
                viewCtrl.ho_Image_Changed = null;
                window.HMouseMove -= Window_HMouseMove;
                ReleaseDisplayImage();
                window?.Dispose();
            }
            catch
            {
            }
        }

        private static void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }

        private void ReleaseDisplayImage()
        {
            if (displayImage == null) return;

            try
            {
                displayImage.Dispose();
            }
            catch
            {
            }

            displayImage = null;
        }

        private void Window_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (disposed || displayImage == null || !displayImage.IsInitialized()) return;

            try
            {
                positionText = GetPositionAndGrayText();
                RaiseMouseInfoChanged();
            }
            catch
            {
                positionText = "R:-  C:-  Value:-";
                RaiseMouseInfoChanged();
            }
        }

        private void RefreshSizeText()
        {
            sizeText = "Size  R:-   C:-";
            if (displayImage == null || !displayImage.IsInitialized()) return;

            try
            {
                HTuple width, height;
                HOperatorSet.GetImageSize(displayImage, out width, out height);
                sizeText = string.Format(CultureInfo.InvariantCulture, "Size  R:{0}   C:{1}", height, width);
            }
            catch
            {
                sizeText = "Size  R:-   C:-";
            }
        }

        private string GetPositionAndGrayText()
        {
            HTuple width, height;
            HTuple row, col, button;
            HOperatorSet.GetImageSize(displayImage, out width, out height);
            HOperatorSet.GetMposition(window.HalconWindow, out row, out col, out button);

            int rowValue = row.TupleInt();
            int colValue = col.TupleInt();
            int heightValue = height.TupleInt();
            int widthValue = width.TupleInt();
            if (rowValue < 0 || rowValue >= heightValue || colValue < 0 || colValue >= widthValue)
            {
                return "R:-  C:-  Value:-";
            }

            HObject grayImage = null;
            bool disposeGrayImage = false;
            try
            {
                HTuple channels;
                HOperatorSet.CountChannels(displayImage, out channels);
                if (channels.TupleInt() > 1)
                {
                    HOperatorSet.AccessChannel(displayImage, out grayImage, 1);
                    disposeGrayImage = true;
                }
                else
                {
                    grayImage = displayImage;
                }

                HTuple gray;
                HOperatorSet.GetGrayval(grayImage, rowValue, colValue, out gray);
                return string.Format(CultureInfo.InvariantCulture, "R:{0}  C:{1}  Value:{2}", rowValue, colValue, gray);
            }
            finally
            {
                if (disposeGrayImage && grayImage != null) grayImage.Dispose();
            }
        }

        private void RaiseMouseInfoChanged()
        {
            EventHandler<HalconImageMouseInfoEventArgs> handler = MouseInfoChanged;
            if (handler != null) handler(this, new HalconImageMouseInfoEventArgs(sizeText, positionText));
        }
    }
}
