using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using HalconDotNet;
using QrBoardMappingTool.Models;
using Forms = System.Windows.Forms;

namespace QrBoardMappingTool.Controls
{
    public sealed class HalconSmartViewer : Grid, IDisposable
    {
        private readonly WindowsFormsHost host;
        private readonly HWindowControl windowControl;
        private readonly HalconSmartControlController controller;

        public HalconSmartViewer()
        {
            windowControl = new HWindowControl
            {
                Dock = Forms.DockStyle.Fill,
                BackColor = Color.Black
            };

            host = new WindowsFormsHost
            {
                Child = windowControl,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            Children.Add(host);
            controller = new HalconSmartControlController(windowControl, true);

            SizeChanged += (s, e) => ResizeChild();
            Loaded += (s, e) => ResizeChild();
        }

        public event EventHandler RoiChanged
        {
            add => controller.RoiChanged += value;
            remove => controller.RoiChanged -= value;
        }

        public event EventHandler ImageChanged
        {
            add => controller.ImageChanged += value;
            remove => controller.ImageChanged -= value;
        }

        public HWindowControl WindowControl => windowControl;
        public HWindow HalconWindow => controller.HalconWindow;
        public HTuple HalconWindowHandle => controller.HalconWindowHandle;
        public HObject Image => controller.Image;
        public int RoiCount => controller.RoiCount;

        public void SetImage(HObject newImage, bool cloneImage = true) => controller.SetImage(newImage, cloneImage);
        public void LoadImage(string filePath) => controller.LoadImage(filePath);
        public void SetOverlay(HObject newOverlay, bool cloneOverlay = true) => controller.SetOverlay(newOverlay, cloneOverlay);
        public void FitImage() => controller.FitImage();
        public void ClearRois() => controller.ClearRois();
        public void RemoveLastRoi() => controller.RemoveLastRoi();
        public void AddRectangleRoiInteractive() => controller.AddRectangleRoiInteractive();
        public void AddCircleRoiInteractive() => controller.AddCircleRoiInteractive();
        public void AddPolygonRoiInteractive() => controller.AddPolygonRoiInteractive();
        public HObject BuildRoiRegion() => controller.BuildRoiRegion();
        public IReadOnlyList<ViewerRoiKind> GetRoiKinds() => controller.GetRoiKinds();
        public void Redraw() => controller.Redraw();
        public void Dispose() => controller.Dispose();

        private void ResizeChild()
        {
            if (ActualWidth <= 1 || ActualHeight <= 1)
            {
                return;
            }

            windowControl.Size = new System.Drawing.Size(Math.Max(1, (int)ActualWidth), Math.Max(1, (int)ActualHeight));
            controller.Redraw();
        }
    }

    public sealed class HalconSmartControlController : IDisposable
    {
        private readonly HWindowControl windowControl;
        private readonly bool ownsControl;
        private readonly List<RoiItem> rois = new List<RoiItem>();
        private HObject image;
        private HObject overlay;
        private RectangleF imagePart = new RectangleF(0, 0, 640, 480);
        private bool hasImagePart;
        private bool isDrawingRoi;
        private bool isPanning;
        private System.Drawing.Point lastMouseLocation;

        public HalconSmartControlController(HWindowControl hWindowControl, bool ownsControl = false)
        {
            windowControl = hWindowControl ?? throw new ArgumentNullException(nameof(hWindowControl));
            this.ownsControl = ownsControl;
            ConfigureWindowControl();
        }

        public event EventHandler RoiChanged;
        public event EventHandler ImageChanged;

        public HWindowControl WindowControl => windowControl;
        public HWindow HalconWindow => windowControl.HalconWindow;
        public HTuple HalconWindowHandle => windowControl.HalconID;
        public HObject Image => image;
        public int RoiCount => rois.Count;

        public void SetImage(HObject newImage, bool cloneImage = true)
        {
            image?.Dispose();
            image = newImage == null ? null : (cloneImage ? newImage.Clone() : newImage);

            if (image != null && image.IsInitialized())
            {
                FitImagePartToImage();
            }
            else
            {
                hasImagePart = false;
            }

            ImageChanged?.Invoke(this, EventArgs.Empty);
            Redraw();
        }

        public void LoadImage(string filePath)
        {
            HOperatorSet.ReadImage(out HObject loaded, filePath);
            SetImage(loaded, false);
        }

        public void SetOverlay(HObject newOverlay, bool cloneOverlay = true)
        {
            overlay?.Dispose();
            overlay = newOverlay == null ? null : (cloneOverlay ? newOverlay.Clone() : newOverlay);
            Redraw();
        }

        public void FitImage()
        {
            if (image == null || !image.IsInitialized())
            {
                return;
            }

            FitImagePartToImage();
            Redraw();
        }

        public void ClearRois()
        {
            foreach (RoiItem roi in rois)
            {
                roi.Region.Dispose();
            }

            rois.Clear();
            RoiChanged?.Invoke(this, EventArgs.Empty);
            Redraw();
        }

        public void RemoveLastRoi()
        {
            if (rois.Count == 0)
            {
                return;
            }

            RoiItem last = rois[rois.Count - 1];
            rois.RemoveAt(rois.Count - 1);
            last.Region.Dispose();
            RoiChanged?.Invoke(this, EventArgs.Empty);
            Redraw();
        }

        public void AddRectangleRoiInteractive()
        {
            EnsureImage();
            isDrawingRoi = true;
            try
            {
                HalconWindow.SetColor("orange");
                HalconWindow.SetDraw("margin");
                HalconWindow.SetLineWidth(2);
                HalconWindow.DrawRectangle1(out double row1, out double col1, out double row2, out double col2);
                HOperatorSet.GenRectangle1(out HObject region, row1, col1, row2, col2);
                AddRoi(region, ViewerRoiKind.Rectangle);
            }
            finally
            {
                isDrawingRoi = false;
                Redraw();
            }
        }

        public void AddCircleRoiInteractive()
        {
            EnsureImage();
            isDrawingRoi = true;
            try
            {
                HalconWindow.SetColor("orange");
                HalconWindow.SetDraw("margin");
                HalconWindow.SetLineWidth(2);
                HalconWindow.DrawCircle(out double row, out double col, out double radius);
                HOperatorSet.GenCircle(out HObject region, row, col, radius);
                AddRoi(region, ViewerRoiKind.Circle);
            }
            finally
            {
                isDrawingRoi = false;
                Redraw();
            }
        }

        public void AddPolygonRoiInteractive()
        {
            EnsureImage();
            isDrawingRoi = true;
            try
            {
                HalconWindow.SetColor("orange");
                HalconWindow.SetDraw("margin");
                HalconWindow.SetLineWidth(2);
                using (HRegion polygon = HalconWindow.DrawPolygon())
                {
                    AddRoi(polygon.Clone(), ViewerRoiKind.Polygon);
                }
            }
            finally
            {
                isDrawingRoi = false;
                Redraw();
            }
        }

        public HObject BuildRoiRegion()
        {
            if (rois.Count == 0)
            {
                return null;
            }

            HObject concat = null;
            HObject union = null;
            try
            {
                HOperatorSet.GenEmptyObj(out concat);
                foreach (RoiItem item in rois)
                {
                    HOperatorSet.ConcatObj(concat, item.Region, out HObject next);
                    concat.Dispose();
                    concat = next;
                }

                HOperatorSet.Union1(concat, out union);
                return union;
            }
            finally
            {
                concat?.Dispose();
            }
        }

        public IReadOnlyList<ViewerRoiKind> GetRoiKinds()
        {
            return rois.Select(r => r.Kind).ToList();
        }

        public void Redraw()
        {
            try
            {
                if (!hasImagePart)
                {
                    HalconWindow.ClearWindow();
                    return;
                }

                if (!TryApplyImagePart())
                {
                    return;
                }
                HalconWindow.ClearWindow();

                if (image != null && image.IsInitialized())
                {
                    HalconWindow.DispObj(image);
                }

                if (rois.Count > 0)
                {
                    HalconWindow.SetDraw("margin");
                    HalconWindow.SetLineWidth(2);
                    HalconWindow.SetColor("orange");
                    foreach (RoiItem roi in rois)
                    {
                        HalconWindow.DispObj(roi.Region);
                    }
                }

                if (overlay != null && overlay.IsInitialized())
                {
                    HalconWindow.SetDraw("margin");
                    HalconWindow.SetLineWidth(2);
                    HalconWindow.SetColor("green");
                    HalconWindow.DispObj(overlay);
                }
            }
            catch
            {
                // The HALCON window handle can be unavailable while WPF is hosting the WinForms control.
            }
        }

        public void Dispose()
        {
            image?.Dispose();
            overlay?.Dispose();
            foreach (RoiItem roi in rois)
            {
                roi.Region.Dispose();
            }

            rois.Clear();
            if (ownsControl)
            {
                windowControl.Dispose();
            }
        }

        private void ConfigureWindowControl()
        {
            windowControl.MouseWheel += WindowControl_MouseWheel;
            windowControl.MouseDown += WindowControl_MouseDown;
            windowControl.MouseMove += WindowControl_MouseMove;
            windowControl.MouseUp += WindowControl_MouseUp;
            windowControl.DoubleClick += (s, e) => FitImage();
            windowControl.Resize += (s, e) => Redraw();
        }

        private void FitImagePartToImage()
        {
            HOperatorSet.GetImageSize(image, out HTuple width, out HTuple height);
            imagePart = new RectangleF(0, 0, Math.Max(1, width.I), Math.Max(1, height.I));
            hasImagePart = true;
            TryApplyImagePart();
        }

        private bool TryApplyImagePart()
        {
            if (!windowControl.IsHandleCreated)
            {
                return false;
            }

            try
            {
                int row1 = (int)Math.Floor(imagePart.Y);
                int col1 = (int)Math.Floor(imagePart.X);
                int row2 = (int)Math.Ceiling(imagePart.Y + imagePart.Height - 1);
                int col2 = (int)Math.Ceiling(imagePart.X + imagePart.Width - 1);
                windowControl.ImagePart = new Rectangle(col1, row1, Math.Max(1, col2 - col1 + 1), Math.Max(1, row2 - row1 + 1));
                HalconWindow.SetPart(row1, col1, row2, col2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void WindowControl_MouseWheel(object sender, Forms.MouseEventArgs e)
        {
            if (!hasImagePart || isDrawingRoi)
            {
                return;
            }

            float factor = e.Delta > 0 ? 0.8f : 1.25f;
            PointF imagePoint = ScreenToImage(e.X, e.Y);
            float newWidth = Math.Max(8, imagePart.Width * factor);
            float newHeight = Math.Max(8, imagePart.Height * factor);
            float percentX = imagePart.Width <= 0 ? 0.5f : (imagePoint.X - imagePart.X) / imagePart.Width;
            float percentY = imagePart.Height <= 0 ? 0.5f : (imagePoint.Y - imagePart.Y) / imagePart.Height;

            imagePart = new RectangleF(
                imagePoint.X - newWidth * percentX,
                imagePoint.Y - newHeight * percentY,
                newWidth,
                newHeight);

            Redraw();
        }

        private void WindowControl_MouseDown(object sender, Forms.MouseEventArgs e)
        {
            if (isDrawingRoi || !hasImagePart)
            {
                return;
            }

            if (e.Button == Forms.MouseButtons.Left || e.Button == Forms.MouseButtons.Middle)
            {
                isPanning = true;
                lastMouseLocation = e.Location;
                windowControl.Capture = true;
            }
        }

        private void WindowControl_MouseMove(object sender, Forms.MouseEventArgs e)
        {
            if (!isPanning || isDrawingRoi || !hasImagePart)
            {
                return;
            }

            float dx = (e.X - lastMouseLocation.X) * imagePart.Width / Math.Max(1, windowControl.Width);
            float dy = (e.Y - lastMouseLocation.Y) * imagePart.Height / Math.Max(1, windowControl.Height);
            imagePart = new RectangleF(imagePart.X - dx, imagePart.Y - dy, imagePart.Width, imagePart.Height);
            lastMouseLocation = e.Location;
            Redraw();
        }

        private void WindowControl_MouseUp(object sender, Forms.MouseEventArgs e)
        {
            isPanning = false;
            windowControl.Capture = false;
        }

        private PointF ScreenToImage(int x, int y)
        {
            float col = imagePart.X + x * imagePart.Width / Math.Max(1, windowControl.Width);
            float row = imagePart.Y + y * imagePart.Height / Math.Max(1, windowControl.Height);
            return new PointF(col, row);
        }

        private void AddRoi(HObject region, ViewerRoiKind kind)
        {
            rois.Add(new RoiItem(region, kind));
            RoiChanged?.Invoke(this, EventArgs.Empty);
        }

        private void EnsureImage()
        {
            if (image == null || !image.IsInitialized())
            {
                throw new InvalidOperationException("Load image before drawing ROI.");
            }
        }

        private sealed class RoiItem
        {
            public RoiItem(HObject region, ViewerRoiKind kind)
            {
                Region = region;
                Kind = kind;
            }

            public HObject Region { get; }
            public ViewerRoiKind Kind { get; }
        }
    }
}
