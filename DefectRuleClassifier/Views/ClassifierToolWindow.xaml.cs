using DefectRuleClassifier.Core.Classifiers;
using DefectRuleClassifier.Core.Models;
using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;

namespace DefectRuleClassifier.Views
{
    /// <summary>序号绑定表行（序号 → 类别名 → 映射特征标签）。</summary>
    public sealed class IndexTableRow
    {
        public int Index { get; set; }
        public string ClassName { get; set; }
        public string FeatureLabel { get; set; }
    }

    /// <summary>批量测试结果行。</summary>
    public sealed class TestResultRow
    {
        public string ImageName { get; set; }
        public string TrueClass { get; set; }
        public string ClassName { get; set; }
        public int ClassIndex { get; set; }
        public double Confidence { get; set; }
        public bool Correct { get; set; }
    }

    /// <summary>
    /// ONNX 分类器完整工具界面：设置（模型/标签/序号绑定表）、推理（加载图片看图）、
    /// 测试（批量文件夹测试 + CSV）、训练（后台续接训练，不打开 Python 界面）。
    /// 序号绑定表：本界面独立读取特征值分类标签（规则文件），用户配置“模型序号 → 特征值标签”；
    /// 运行时只用绑定缓存 O(1) 查表输出，未绑定统一输出超限类别（默认涂布不良），与特征值分类参数解耦。
    /// </summary>
    public partial class ClassifierToolWindow : System.Windows.Window
    {
        private readonly ModelClassifierConfig config;
        private OnnxModelClassifier classifier;
        private string[] labels = new string[0];
        private Process trainingProcess;
        private readonly ObservableCollection<IndexTableRow> bindingRows = new ObservableCollection<IndexTableRow>();

        /// <summary>绑定保存完成后回调（由打开方提供，用于通知插件重新加载配置）。</summary>
        public Action OnBindingSaved { get; set; }

        /// <summary>序号绑定表“映射特征标签”下拉选项（从特征值分类规则文件独立读取）。</summary>
        public ObservableCollection<string> FeatureLabelOptions { get; } = new ObservableCollection<string>();

        private static readonly string DefaultModel = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "runs", "model.onnx");
        private static readonly string DefaultLabels = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "runs", "labels.txt");
        private static readonly string PythonExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Python312", "python.exe");
        private static readonly string TrainScript = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OpenCV分类Demo", "train_demo.py");

        public ClassifierToolWindow(
            ModelClassifierConfig config = null,
            IEnumerable<string> featureLabels = null,
            string featureRulePath = null)
        {
            InitializeComponent();
            this.config = config ?? new ModelClassifierConfig();
            Loaded += (s, e) =>
            {
                ModelPathTextBox.Text = !string.IsNullOrWhiteSpace(this.config.ModelPath) ? this.config.ModelPath : DefaultModel;
                LabelsPathTextBox.Text = !string.IsNullOrWhiteSpace(this.config.LabelsPath) ? this.config.LabelsPath : DefaultLabels;
                FeatureRulePathTextBox.Text = featureRulePath ?? "";
                if (!string.IsNullOrWhiteSpace(featureRulePath))
                {
                    // 路径可能不带 .json 扩展名（框架路径），直接交给加载逻辑探测。
                    FeatureRulePathTextBox.Text = featureRulePath;
                    LoadFeatureLabelsFromFile(featureRulePath);
                }
                else
                {
                    // 自动到默认配置目录查找特征值分类规则文件。
                    string defaultRule = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Plugin.DefectCoat", "FeatureRuleClassifierConfig.json");
                    if (File.Exists(defaultRule))
                    {
                        FeatureRulePathTextBox.Text = defaultRule;
                        LoadFeatureLabelsFromFile(defaultRule);
                    }
                }
                if (featureLabels != null)
                    SetFeatureLabelOptions(featureLabels);
                if (FeatureLabelOptions.Count == 0)
                    ModelStatusTextBlock.Text = "未读取到特征值分类标签：可直接在“映射特征标签”下拉框输入，或选择特征值分类规则文件加载。";
                RefreshBindingTable();
                LoadModel();
            };
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                if (MessageBox.Show("续接训练仍在进行，确定要关闭吗？（训练进程会被终止）",
                        "退出确认", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
                try { trainingProcess.Kill(); } catch { }
            }
            classifier?.Dispose();
            base.OnClosing(e);
        }

        // ------------------------------------------------------------------
        // 设置：模型 / 标签 / 特征值标签 / 序号绑定表
        // ------------------------------------------------------------------

        private void BrowseModelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "ONNX 模型|*.onnx|所有文件|*.*", Title = "选择 ONNX 模型" };
            if (dialog.ShowDialog() == true)
            {
                ModelPathTextBox.Text = dialog.FileName;
                config.ModelPath = dialog.FileName;
            }
        }

        private void BrowseLabelsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { Filter = "标签文件|*.txt|所有文件|*.*", Title = "选择 labels.txt（行序=序号 0 起）" };
            if (dialog.ShowDialog() == true)
            {
                LabelsPathTextBox.Text = dialog.FileName;
                config.LabelsPath = dialog.FileName;
                RefreshBindingTable();
            }
        }

        private void BrowseFeatureRuleButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "分类规则参数|*.json|所有文件|*.*",
                Title = "选择特征值分类规则文件（独立读取其特征标签）"
            };
            if (dialog.ShowDialog() == true)
            {
                FeatureRulePathTextBox.Text = dialog.FileName;
                LoadFeatureLabelsFromFile(dialog.FileName);
            }
        }

        private void LoadFeatureRuleButton_Click(object sender, RoutedEventArgs e)
        {
            LoadFeatureLabelsFromFile(FeatureRulePathTextBox.Text);
        }

        /// <summary>独立读取特征值分类规则文件中的标签（NgTypes 名称）。</summary>
        private void LoadFeatureLabelsFromFile(string rulePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rulePath))
                {
                    ModelStatusTextBlock.Text = "特征值分类规则文件路径为空。";
                    return;
                }
                string probe = rulePath;
                if (!File.Exists(probe) && !probe.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    probe = probe + ".json";
                if (!File.Exists(probe))
                {
                    ModelStatusTextBlock.Text = "特征值分类规则文件不存在：" + rulePath;
                    return;
                }
                FeatureRuleClassifierConfig rule = FeatureRuleClassifierConfig.LoadOrDefault(probe);
                var names = rule.NgTypes == null
                    ? new List<string>()
                    : rule.NgTypes.Where(r => r != null && !string.IsNullOrWhiteSpace(r.Name))
                        .Select(r => r.Name.Trim())
                        .ToList();
                SetFeatureLabelOptions(names);
                ModelStatusTextBlock.Text = "已读取特征值分类标签 " + names.Count + " 个：" + string.Join("、", names);
            }
            catch (Exception ex)
            {
                ModelStatusTextBlock.Text = "读取特征值分类标签失败：" + ex.Message;
            }
        }

        private void SetFeatureLabelOptions(IEnumerable<string> names)
        {
            FeatureLabelOptions.Clear();
            if (names != null)
            {
                foreach (string name in names.Distinct(StringComparer.OrdinalIgnoreCase))
                    FeatureLabelOptions.Add(name);
            }
        }

        private void LoadModelButton_Click(object sender, RoutedEventArgs e)
        {
            LoadModel();
        }

        private void LoadModel()
        {
            try
            {
                classifier?.Dispose();
                classifier = new OnnxModelClassifier(config);
                string message;
                bool ok = classifier.Init(ModelPathTextBox.Text, out message);
                labels = classifier.Labels;
                config.ModelClassCount = labels != null ? labels.Length : 0;
                config.RebuildResolvedLabels();
                ModelStatusTextBlock.Text = ok
                    ? "模型已加载：输入 " + classifier.InputChannels + "x" + classifier.InputSize + "x" + classifier.InputSize + "，类别数 " + labels.Length
                    : message;
                RefreshBindingTable();
            }
            catch (Exception ex)
            {
                ModelStatusTextBlock.Text = "模型加载失败：" + ex.Message;
            }
        }

        /// <summary>重建序号绑定表（序号、类别名、当前绑定的特征标签）。</summary>
        private void RefreshBindingTable()
        {
            bindingRows.Clear();
            string labelsPath = LabelsPathTextBox.Text;
            string[] names = new string[0];
            if (!string.IsNullOrWhiteSpace(labelsPath) && File.Exists(labelsPath))
            {
                names = OnnxModelClassifier.ReadLabelsRobust(labelsPath);
            }
            for (int i = 0; i < names.Length; i++)
            {
                bindingRows.Add(new IndexTableRow
                {
                    Index = i,
                    ClassName = names[i],
                    FeatureLabel = config.GetFeatureLabelByIndex(i) ?? ""
                });
            }
            IndexTableGrid.ItemsSource = bindingRows;
        }

        /// <summary>编辑绑定表后同步到 config.IndexToFeatureLabel。</summary>
        private void IndexTableGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;
            IndexTableRow row = e.Row.Item as IndexTableRow;
            if (row == null)
                return;
            SyncBindingRow(row);
        }

        private void SyncBindingRow(IndexTableRow row)
        {
            var existing = config.IndexToFeatureLabel.FirstOrDefault(m => m != null && m.Index == row.Index);
            string value = string.IsNullOrWhiteSpace(row.FeatureLabel) ? null : row.FeatureLabel.Trim();
            if (existing != null)
            {
                existing.FeatureLabel = value ?? "";
            }
            else
            {
                config.IndexToFeatureLabel.Add(new ModelLabelMapItem { Index = row.Index, FeatureLabel = value ?? "" });
            }
        }

        /// <summary>按同名自动绑定：模型类别名与特征值标签同名时自动填入绑定。</summary>
        private void AutoBindSameNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (FeatureLabelOptions.Count == 0)
            {
                MessageBox.Show("请先加载特征值分类标签（选择并加载特征值分类规则文件）。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (IndexTableRow row in bindingRows)
            {
                if (string.IsNullOrWhiteSpace(row.FeatureLabel))
                {
                    string match = FeatureLabelOptions.FirstOrDefault(o => string.Equals(o, row.ClassName, StringComparison.OrdinalIgnoreCase));
                    if (match != null) row.FeatureLabel = match;
                }
            }
            foreach (IndexTableRow row in bindingRows)
                SyncBindingRow(row);
            config.RebuildResolvedLabels();
            RefreshBindingTable();
            ModelStatusTextBlock.Text = "已按同名自动绑定；未绑定的序号输出 " + config.GetOverflowClassName();
        }

        /// <summary>保存绑定：重建 O(1) 查表缓存，并写入配置 JSON。</summary>
        private void SaveBindingButton_Click(object sender, RoutedEventArgs e)
        {
            // 先提交正在编辑的单元格。
            IndexTableGrid.CommitEdit();
            foreach (IndexTableRow row in bindingRows)
                SyncBindingRow(row);
            config.RebuildResolvedLabels();

            string savePath = config.ConfigPath;
            if (string.IsNullOrWhiteSpace(savePath))
            {
                var dialog = new SaveFileDialog { Filter = "分类器参数|*.json", FileName = "ModelClassifierConfig.json", Title = "保存分类器绑定配置" };
                if (dialog.ShowDialog() != true) return;
                savePath = dialog.FileName;
                config.ConfigPath = savePath;
            }
            try
            {
                config.Save(savePath);
                ModelStatusTextBlock.Text = "绑定已保存：" + savePath + "；运行时按绑定缓存 O(1) 查表输出，未绑定统一为 " + config.GetOverflowClassName();
                try { OnBindingSaved?.Invoke(); }
                catch (Exception ex) { ModelStatusTextBlock.Text += "；通知插件重载失败：" + ex.Message; }
            }
            catch (Exception ex)
            {
                ModelStatusTextBlock.Text = "保存失败：" + ex.Message;
            }
        }

        private void ExportIndexTableButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog { Filter = "文本文件|*.txt", FileName = "序号对照表.txt", Title = "导出序号对照表" };
            if (dialog.ShowDialog() != true) return;
            var builder = new StringBuilder();
            builder.AppendLine("序号\t类别名\t映射特征标签\t未绑定时输出");
            foreach (IndexTableRow row in bindingRows)
                builder.AppendLine(row.Index + "\t" + row.ClassName + "\t" + (row.FeatureLabel ?? "") + "\t" + config.GetOverflowClassName());
            File.WriteAllText(dialog.FileName, builder.ToString(), new UTF8Encoding(true));
            MessageBox.Show("已导出：" + dialog.FileName, "导出完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ------------------------------------------------------------------
        // 推理：加载图片 → 显示分类类型
        // ------------------------------------------------------------------

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "图像文件|*.bmp;*.jpg;*.jpeg;*.png;*.tif;*.tiff|所有文件|*.*",
                Title = "选择待分类图片"
            };
            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
                SourcePreviewImage.Source = LoadBitmap(dialog.FileName);
                ResultPreviewImage.Source = null;
                ResultClassTextBlock.Text = "--";
                ResultIndexTextBlock.Text = "--";
                ResultConfidenceTextBlock.Text = "--";
                TopKListBox.ItemsSource = null;
            }
        }

        private void InferButton_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = ImagePathTextBox.Text;
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                MessageBox.Show("请先选择一张图片。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (classifier == null)
            {
                MessageBox.Show("请先加载模型。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var input = LoadModelImageInput(imagePath);
                var decision = classifier.Classify(input);
                config.RebuildResolvedLabels();
                ResultClassTextBlock.Text = config.GetResolvedLabel(decision.ClassIndex);
                ResultIndexTextBlock.Text = decision.ClassIndex.ToString();
                ResultConfidenceTextBlock.Text = decision.Confidence.ToString("P2");
                TopKListBox.ItemsSource = EnumerateScores(decision);
                ResultPreviewImage.Source = BuildAnnotatedPreview(input, decision);
            }
            catch (Exception ex)
            {
                MessageBox.Show("推理失败：" + ex.Message, "推理异常", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<string> EnumerateScores(DefectClassifyDecision decision)
        {
            var items = new List<string>();
            if (decision.Scores == null) return items;
            var order = Enumerable.Range(0, decision.Scores.Length)
                .OrderByDescending(i => decision.Scores[i]);
            foreach (int i in order)
                items.Add(i + "  " + (i < labels.Length ? labels[i] : ("类别" + i)) + "  " + decision.Scores[i].ToString("P2"));
            return items;
        }

        // ------------------------------------------------------------------
        // 测试：批量文件夹测试
        // ------------------------------------------------------------------

        private void BrowseTestRootButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new Forms.FolderBrowserDialog { Description = "选择测试目录（一级子目录名=真实标签）" })
            {
                if (dialog.ShowDialog() == Forms.DialogResult.OK)
                    TestRootTextBox.Text = dialog.SelectedPath;
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            string root = TestRootTextBox.Text;
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                MessageBox.Show("请先选择测试目录。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (classifier == null)
            {
                MessageBox.Show("请先加载模型。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            int workers;
            if (!int.TryParse(TestWorkersTextBox.Text.Trim(), out workers) || workers <= 0) workers = 4;
            TestButton.IsEnabled = false;
            TestSummaryTextBlock.Text = "正在测试…";
            try
            {
                var images = EnumerateImages(root).ToList();
                if (images.Count == 0)
                {
                    TestSummaryTextBlock.Text = "目录中没有图片。";
                    return;
                }

                var rows = new TestResultRow[images.Count];
                await Task.Run(() =>
                {
                    Parallel.For(0, images.Count, new ParallelOptions { MaxDegreeOfParallelism = workers }, i =>
                    {
                        string path = images[i];
                        var input = LoadModelImageInput(path);
                        var decision = classifier.Classify(input);
                        string trueClass = GetTrueClass(root, path);
                        rows[i] = new TestResultRow
                        {
                            ImageName = Path.GetFileName(path),
                            TrueClass = trueClass,
                            ClassName = decision.ClassName,
                            ClassIndex = decision.ClassIndex,
                            Confidence = decision.Confidence,
                            Correct = string.Equals(decision.ClassName, trueClass, StringComparison.OrdinalIgnoreCase)
                        };
                    });
                });

                int correct = rows.Count(r => r.Correct);
                TestSummaryTextBlock.Text = string.Format("共 {0} 张，正确 {1} 张，准确率 {2:P2}", rows.Length, correct, (double)correct / rows.Length);
                TestResultGrid.ItemsSource = rows;
                lastTestRows = rows;
            }
            catch (Exception ex)
            {
                TestSummaryTextBlock.Text = "测试失败：" + ex.Message;
            }
            finally
            {
                TestButton.IsEnabled = true;
            }
        }

        private TestResultRow[] lastTestRows;

        private void ExportTestCsvButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastTestRows == null || lastTestRows.Length == 0)
            {
                MessageBox.Show("请先执行一次测试。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var dialog = new SaveFileDialog { Filter = "CSV 文件|*.csv", FileName = "测试结果.csv" };
            if (dialog.ShowDialog() != true) return;
            var builder = new StringBuilder();
            builder.AppendLine("图片,真实类别,预测类别,标签序号,置信度,是否正确");
            foreach (var row in lastTestRows)
                builder.AppendLine(string.Format("{0},{1},{2},{3},{4:F4},{5}",
                    row.ImageName, row.TrueClass, row.ClassName, row.ClassIndex, row.Confidence, row.Correct ? "是" : "否"));
            File.WriteAllText(dialog.FileName, builder.ToString(), new UTF8Encoding(true));
            MessageBox.Show("已导出：" + dialog.FileName, "导出完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ------------------------------------------------------------------
        // 训练：后台续接训练
        // ------------------------------------------------------------------

        private void BrowseTrainDataButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new Forms.FolderBrowserDialog { Description = "选择数据集根目录（类别文件夹=标签）" })
            {
                if (dialog.ShowDialog() == Forms.DialogResult.OK)
                    TrainDataRootTextBox.Text = dialog.SelectedPath;
            }
        }

        private void TrainButton_Click(object sender, RoutedEventArgs e)
        {
            if (trainingProcess != null && !trainingProcess.HasExited)
            {
                MessageBox.Show("续接训练已在运行，请等待完成。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string dataRoot = TrainDataRootTextBox.Text.Trim();
            int epochs;
            if (string.IsNullOrWhiteSpace(dataRoot) || !Directory.Exists(dataRoot))
            {
                MessageBox.Show("请选择有效的数据集根目录。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(TrainEpochsTextBox.Text.Trim(), out epochs) || epochs <= 0) epochs = 30;

            string runsDir = Path.GetDirectoryName(ModelPathTextBox.Text);
            if (string.IsNullOrWhiteSpace(runsDir) || !Directory.Exists(runsDir))
            {
                MessageBox.Show("模型输出目录不存在：" + runsDir, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool resume = TrainResumeCheckBox.IsChecked == true;
            TrainButton.IsEnabled = false;
            TrainLogTextBox.Clear();
            AppendLog(">> 启动续接训练（后台隐藏进程，不打开 Python 界面）…");
            AppendLog(">> 数据集：" + dataRoot + "  输出目录：" + runsDir + "  轮数：" + epochs + (resume ? "（续接）" : "（全新）"));

            var startInfo = new ProcessStartInfo
            {
                FileName = PythonExe,
                Arguments = string.Format("\"{0}\" --data \"{1}\" --out \"{2}\" --epochs {3} --all-data {4}",
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
                    TrainButton.IsEnabled = true;
                    AppendLog(">> 训练进程退出码：" + trainingProcess.ExitCode);
                    trainingProcess = null;
                    try { LoadModel(); } catch { }
                    AppendLog(">> 模型已重新加载。");
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
                TrainLogTextBox.AppendText(text + Environment.NewLine);
                TrainLogTextBox.ScrollToEnd();
            }));
        }

        // ------------------------------------------------------------------
        // 工具
        // ------------------------------------------------------------------

        private static ModelImageInput LoadModelImageInput(string imagePath)
        {
            using (var src = new Mat(imagePath, ImreadModes.Color))
            {
                using (var rgbMat = new Mat())
                {
                    Cv2.CvtColor(src, rgbMat, ColorConversionCodes.BGR2RGB);
                    int w = rgbMat.Width, h = rgbMat.Height;
                    byte[] rgb = new byte[w * h * 3];
                    int step = (int)rgbMat.Step();
                    unsafe
                    {
                        byte* p = (byte*)rgbMat.Data;
                        int idx = 0;
                        for (int y = 0; y < h; y++)
                        {
                            byte* row = p + y * step;
                            for (int x = 0; x < w; x++)
                            {
                                rgb[idx++] = row[x * 3];
                                rgb[idx++] = row[x * 3 + 1];
                                rgb[idx++] = row[x * 3 + 2];
                            }
                        }
                    }
                    return ModelImageInput.FromRgb(rgb, w, h, Path.GetFileName(imagePath));
                }
            }
        }

        private static BitmapImage LoadBitmap(string path)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(path)))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        private static BitmapImage BuildAnnotatedPreview(ModelImageInput input, DefectClassifyDecision decision)
        {
            try
            {
                using (var bitmap = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb))
                {
                    BitmapData data = bitmap.LockBits(new Rectangle(0, 0, input.Width, input.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                    byte[] rgb = input.Rgb;
                    unsafe
                    {
                        byte* dst = (byte*)data.Scan0;
                        int stride = data.Stride;
                        int idx = 0;
                        for (int y = 0; y < input.Height; y++)
                        {
                            byte* row = dst + y * stride;
                            for (int x = 0; x < input.Width; x++)
                            {
                                row[x * 3] = rgb[idx++];
                                row[x * 3 + 1] = rgb[idx++];
                                row[x * 3 + 2] = rgb[idx++];
                            }
                        }
                    }
                    bitmap.UnlockBits(data);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        string text = string.Format("{0}  #{1}  {2:P1}", decision.ClassName, decision.ClassIndex, decision.Confidence);
                        using (Font font = new Font("Microsoft YaHei", 18f, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel))
                        using (Brush backBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                        using (Brush textBrush = new SolidBrush(Color.FromArgb(255, 255, 80, 80)))
                        {
                            SizeF size = g.MeasureString(text, font);
                            g.FillRectangle(backBrush, 0f, 0f, size.Width + 12f, size.Height + 8f);
                            g.DrawString(text, font, textBrush, 6f, 4f);
                        }
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        var image = new BitmapImage();
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

        private static IEnumerable<string> EnumerateImages(string root)
        {
            var extensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff" };
            var result = new List<string>();
            Walk(root, extensions, result);
            return result;
        }

        private static void Walk(string directory, string[] extensions, List<string> result)
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                if (extensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                    result.Add(file);
            }
            foreach (string sub in Directory.EnumerateDirectories(directory))
            {
                string name = Path.GetFileName(sub);
                if (name.StartsWith("_") || name.StartsWith(".") || name.StartsWith("评估输出"))
                    continue;
                Walk(sub, extensions, result);
            }
        }

        private static string GetTrueClass(string root, string imagePath)
        {
            string directory = Path.GetDirectoryName(imagePath) ?? "";
            if (string.Equals(Path.GetFullPath(directory), Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase))
                return "";
            return Path.GetFileName(directory);
        }
    }
}