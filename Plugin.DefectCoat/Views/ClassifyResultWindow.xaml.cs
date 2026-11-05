using DefectRuleClassifier.Core.Models;
using HalconDotNet;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Plugin.DefectCoat.Views
{
    /// <summary>
    /// 分类结果独立窗口。
    /// 左侧结果表格，右侧显示选中缺陷的推理效果（小图 + 类别/序号/置信度标注），
    /// 底部支持后台“续接训练”（隐藏 Python 进程，不打开 Python 界面）。
    /// </summary>
    public partial class ClassifyResultWindow : Window
    {
        /// <summary>数据集根目录（续接训练用）。</summary>
        public string TrainingDataRoot { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "测试");

        /// <summary>模型输出目录（续接训练用，含 model.onnx / labels.txt / checkpoint.pt）。</summary>
        public string TrainingRunsDir { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "runs");

        /// <summary>Python 可执行文件路径。</summary>
        public string PythonExe { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python312", "python.exe");

        /// <summary>训练脚本路径。</summary>
        public string TrainScript { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "train_demo.py");

        /// <summary>训练完成后回调（通常用于重新 InitObj 加载新模型）。</summary>
        public Action TrainingCompleted { get; set; }

        private Process trainingProcess;

        public ClassifyResultWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                TrainingDataRootTextBox.Text = TrainingDataRoot;
                if (!string.IsNullOrWhiteSpace(TrainingRunsDir))
                {
                    TrainingLogTextBox.AppendText("模型目录：" + TrainingRunsDir + Environment.NewLine);
                }
            };
        }

        /// <summary>关闭按钮事件，只关闭窗口，不释放检测结果对象。</summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                if (MessageBox.Show("续接训练仍在进行，确定要关闭窗口吗？（训练进程会被终止）",
                        "退出确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
                try
                {
                    trainingProcess.Kill();
                }
                catch
                {
                }
            }
            base.OnClosing(e);
        }

        // ------------------------------------------------------------------
        // 推理效果预览
        // ------------------------------------------------------------------

        private void ResultGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NgDefectResult result = ResultGrid.SelectedItem as NgDefectResult;
            if (result == null)
            {
                PreviewImage.Source = null;
                PreviewInfoTextBlock.Text = "未选择缺陷。";
                PreviewHitRuleTextBlock.Text = "";
                return;
            }

            PreviewInfoTextBlock.Text = string.Format(
                "缺陷 #{0}：{1}   标签序号 {2}   置信度 {3:P2}   来源 {4}",
                result.Index, result.ClassName, result.ClassIndex, result.Confidence, result.Source);
            PreviewHitRuleTextBlock.Text = result.HitRule;
            PreviewImage.Source = BuildAnnotatedPreview(result);
        }

        /// <summary>把缺陷小图转成带标注的 BitmapImage（类别/序号/置信度画在图上）。</summary>
        private static BitmapImage BuildAnnotatedPreview(NgDefectResult result)
        {
            try
            {
                using (Bitmap bitmap = CropToBitmap(result))
                {
                    if (bitmap == null)
                        return null;
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        string text = string.Format("{0}  #{1}  {2:P1}",
                            result.ClassName, result.ClassIndex, result.Confidence);
                        using (Font font = new Font("Microsoft YaHei", 18f, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel))
                        using (Brush backBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                        using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 80, 80)))
                        {
                            SizeF size = g.MeasureString(text, font);
                            float w = size.Width + 12f;
                            float h = size.Height + 8f;
                            g.FillRectangle(backBrush, 0f, 0f, w, h);
                            g.DrawString(text, font, textBrush, 6f, 4f);
                        }
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = stream;
                        image.EndInit();
                        image.Freeze();
                        return image;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>把结果对象的 HALCON 小图转成 System.Drawing.Bitmap（RGB）。只读借用，不释放候选小图。</summary>
        private static unsafe Bitmap CropToBitmap(NgDefectResult result)
        {
            if (result == null || result.CropImage == null || !result.CropImage.IsInitialized())
                return null;

            HImage image = result.CropImage as HImage;
            bool owns = false;
            try
            {
                if (image == null)
                {
                    image = new HImage(result.CropImage);
                    owns = true;
                }
                if (image.CountChannels().I == 1)
                {
                    HImage three = image.Compose3(image, image);
                    if (owns) image.Dispose();
                    image = three;
                    owns = true;
                }
                if (!string.Equals(image.GetImageType().S, "byte", StringComparison.OrdinalIgnoreCase))
                {
                    HImage byteImage = image.ConvertImageType("byte");
                    if (owns) image.Dispose();
                    image = byteImage;
                    owns = true;
                }

                int width;
                int height;
                IntPtr red;
                IntPtr green;
                IntPtr blue;
                string type;
                image.GetImagePointer3(out red, out green, out blue, out type, out width, out height);

                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                byte* rp = (byte*)red;
                byte* gp = (byte*)green;
                byte* bp = (byte*)blue;
                byte* dst = (byte*)data.Scan0;
                int stride = data.Stride;
                for (int y = 0; y < height; y++)
                {
                    byte* row = dst + y * stride;
                    int srcRow = y * width;
                    for (int x = 0; x < width; x++)
                    {
                        int p = srcRow + x;
                        row[x * 3] = rp[p];
                        row[x * 3 + 1] = gp[p];
                        row[x * 3 + 2] = bp[p];
                    }
                }
                bitmap.UnlockBits(data);
                return bitmap;
            }
            finally
            {
                if (owns) image?.Dispose();
            }
        }

        // ------------------------------------------------------------------
        // 续接训练（后台隐藏 Python 进程，不打开 Python 界面）
        // ------------------------------------------------------------------

        private void TrainingButton_Click(object sender, RoutedEventArgs e)
        {
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                MessageBox.Show("续接训练已在运行，请等待完成。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string dataRoot = TrainingDataRootTextBox.Text.Trim();
            int epochs;
            if (string.IsNullOrWhiteSpace(dataRoot) || !Directory.Exists(dataRoot))
            {
                MessageBox.Show("请填写有效的数据集根目录（类别文件夹=标签）。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(TrainingEpochsTextBox.Text.Trim(), out epochs) || epochs <= 0)
                epochs = 30;

            string runsDir = TrainingRunsDir;
            if (string.IsNullOrWhiteSpace(runsDir) || !Directory.Exists(runsDir))
            {
                MessageBox.Show("模型输出目录不存在：" + runsDir, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool resume = TrainingResumeCheckBox.IsChecked == true;
            TrainingButton.IsEnabled = false;
            TrainingLogTextBox.Clear();
            AppendLog(">> 启动续接训练（后台进程，不打开 Python 界面）…");
            AppendLog(">> 数据集：" + dataRoot);
            AppendLog(">> 输出目录：" + runsDir + "  轮数：" + epochs + (resume ? "（续接）" : "（全新）"));

            var startInfo = new ProcessStartInfo
            {
                FileName = PythonExe,
                Arguments = string.Format(
                    "\"{0}\" --data \"{1}\" --out \"{2}\" --epochs {3} --all-data {4}",
                    TrainScript, dataRoot, runsDir, epochs, resume ? "--resume" : string.Empty),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(TrainScript) ?? AppDomain.CurrentDomain.BaseDirectory
            };

            trainingProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
            trainingProcess.OutputDataReceived += (s, ev) => { if (!string.IsNullOrEmpty(ev.Data)) AppendLog(ev.Data); };
            trainingProcess.ErrorDataReceived += (s, ev) => { if (!string.IsNullOrEmpty(ev.Data)) AppendLog("[训练] " + ev.Data); };
            trainingProcess.Exited += (s, ev) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    TrainingButton.IsEnabled = true;
                    AppendLog(">> 训练进程退出码：" + trainingProcess.ExitCode);
                    trainingProcess = null;
                    try
                    {
                        TrainingCompleted?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        AppendLog(">> 重新加载模型失败：" + ex.Message);
                    }
                }));
            };
            trainingProcess.Start();
            trainingProcess.BeginOutputReadLine();
            trainingProcess.BeginErrorReadLine();
        }

        private void AppendLog(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                TrainingLogTextBox.AppendText(text + Environment.NewLine);
                TrainingLogTextBox.ScrollToEnd();
            }));
        }
    }
}