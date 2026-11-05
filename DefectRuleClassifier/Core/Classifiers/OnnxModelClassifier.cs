using DefectRuleClassifier.Core.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DefectRuleClassifier.Core.Classifiers
{
    /// <summary>
    /// 传统分类器接口（IModelDefectClassifier）的 ONNX 真实实现。
    ///
    /// 设计要点：
    /// 1. 每个实例独立持有 InferenceSession——多个插件对象/多个模块即使共用同一个 .onnx，
    ///    彼此也完全隔离，互不干扰；
    /// 2. Classify 线程安全（InferenceSession.Run 支持并发），配合流水线 Parallel.For 对多个
    ///    候选小图并行推理，解决“图片多、一张张太耗时”的问题；
    /// 3. 输入为与 HALCON 解耦的 ModelImageInput（RGB 像素 + 宽高）：等比例缩放 + 居中补边
    ///    （OpenCV 算法，与训练端一致，不拉伸主体）→ 归一化到 [-1,1] → ONNX 推理；
    /// 4. 输出携带 ClassName（标签名）、ClassIndex（标签序号，labels.txt 顺序，0 起）、
    ///    Confidence（置信度），供界面显示和序号绑定表映射到特征值标签。
    /// </summary>
    public sealed class OnnxModelClassifier : IModelDefectClassifier, IDisposable
    {
        private readonly ModelClassifierConfig config;
        private readonly object initLock = new object();
        private InferenceSession session;
        private string[] labels = new string[0];
        private string inputName = "input";
        private int inputSize = 128;
        private int inputChannels = 3;
        private bool initialized;
        private bool disposed;

        /// <summary>模型类别名称（labels.txt 顺序，0 起）。</summary>
        public string[] Labels
        {
            get { return labels; }
        }

        /// <summary>模型输入边长（从 ONNX 自动读取）。</summary>
        public int InputSize
        {
            get { return inputSize; }
        }

        /// <summary>模型输入通道数（从 ONNX 自动读取）。</summary>
        public int InputChannels
        {
            get { return inputChannels; }
        }

        public OnnxModelClassifier(ModelClassifierConfig config)
        {
            this.config = config ?? new ModelClassifierConfig();
        }

        /// <summary>按模型路径初始化 ONNX 会话并读取标签；重复调用会重新加载。</summary>
        public bool Init(string modelPath, out string message)
        {
            lock (initLock)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
                    {
                        initialized = false;
                        message = "分类器模型未加载：模型路径为空或文件不存在：" + (modelPath ?? "");
                        return false;
                    }

                    session?.Dispose();
                    session = new InferenceSession(modelPath);

                    var inputMeta = session.InputMetadata.First();
                    inputName = inputMeta.Key;
                    var dims = inputMeta.Value.Dimensions; // int[]，-1 表示动态维度
                    int size = 128;
                    int channels = 3;
                    if (dims != null && dims.Length >= 4)
                    {
                        if (dims[1] > 0) channels = dims[1];
                        if (dims[2] > 0) size = dims[2];
                    }
                    inputSize = size;
                    inputChannels = channels;

                    labels = LoadLabels(modelPath);
                    initialized = labels.Length > 0;
                    message = string.Format(
                        "分类器模型已初始化：{0}；输入 {1}x{2}x{3}；类别数 {4}{5}",
                        modelPath, inputChannels, inputSize, inputSize, labels.Length,
                        labels.Length == 0 ? "（未找到标签文件，推理仍可执行但无标签名）" : "");
                    return true;
                }
                catch (Exception ex)
                {
                    initialized = false;
                    message = "分类器模型初始化失败：" + ex.Message;
                    return false;
                }
            }
        }

        /// <summary>
        /// 对单个候选缺陷像素输入执行分类。线程安全，可并行调用。
        /// 推理失败时返回 SkipClassifier=true，由流水线继续交给后续分类层或按未分类输出。
        /// </summary>
        public DefectClassifyDecision Classify(ModelImageInput input)
        {
            if (!initialized || session == null)
            {
                return BuildFailure("分类器模型未加载", "模型未初始化或加载失败，无法执行 ONNX 推理");
            }
            if (input == null || input.Rgb == null || input.Rgb.Length == 0 || input.Width <= 0 || input.Height <= 0)
            {
                return BuildFailure("分类器无图像", "缺少有效的 RGB 像素输入（ModelImageInput），无法推理");
            }

            try
            {
                float[] data = LetterboxAndNormalize(input.Rgb, input.Width, input.Height, inputSize, inputChannels);
                var tensor = new DenseTensor<float>(data, new[] { 1, inputChannels, inputSize, inputSize });
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor(inputName, tensor)
                };

                using (var outputs = session.Run(inputs, new[] { "logits" }))
                {
                    var logits = outputs.First().AsTensor<float>().ToArray();
                    var scores = Softmax(logits);
                    int best = ArgMax(scores);
                    string className = best >= 0 && best < labels.Length ? labels[best] : "类别" + best;
                    return new DefectClassifyDecision
                    {
                        ClassName = className,
                        ClassIndex = best,
                        Confidence = scores[best],
                        Scores = scores,
                        Priority = 50,
                        Source = "分类器分类",
                        HitRule = string.Format(
                            "模型序号={0}，模型标签={1}，置信度={2:P2}，输入={3}x{4}{5}",
                            best, className, scores[best], inputSize, inputSize,
                            string.IsNullOrWhiteSpace(input.Source) ? "" : "；来源=" + input.Source),
                        SkipClassifier = false
                    };
                }
            }
            catch (Exception ex)
            {
                return BuildFailure("分类器推理异常", ex.Message);
            }
        }

        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            lock (initLock)
            {
                session?.Dispose();
                session = null;
                initialized = false;
            }
        }

        private static DefectClassifyDecision BuildFailure(string className, string hitRule)
        {
            return new DefectClassifyDecision
            {
                ClassName = className,
                ClassIndex = -1,
                Confidence = 0,
                Priority = 999,
                Source = className,
                HitRule = hitRule,
                SkipClassifier = true
            };
        }

        /// <summary>
        /// 按 UTF-8（严格）解析标签文件，失败自动按 GBK(936) 解析。
        /// net472 原生支持 GBK，兼容 Notepad 另存为 ANSI 的 labels.txt。
        /// </summary>
        internal static string[] ReadLabelsRobust(string path)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                string text;
                if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                {
                    // UTF-8 带 BOM
                    text = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
                }
                else if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                {
                    // UTF-16 LE（记事本“Unicode”）
                    text = Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
                }
                else if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                {
                    // UTF-16 BE
                    text = Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);
                }
                else
                {
                    try
                    {
                        text = new UTF8Encoding(false, true).GetString(bytes);
                    }
                    catch (DecoderFallbackException)
                    {
                        text = Encoding.GetEncoding(936).GetString(bytes);
                    }
                }
                return text
                    .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0)
                    .ToArray();
            }
            catch
            {
                return new string[0];
            }
        }

        private string[] LoadLabels(string modelPath)
        {
            var candidates = new List<string>();
            if (!string.IsNullOrWhiteSpace(config.LabelsPath))
                candidates.Add(config.LabelsPath);
            string directory = Path.GetDirectoryName(modelPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                string baseName = Path.GetFileNameWithoutExtension(modelPath);
                candidates.Add(Path.Combine(directory, baseName + ".labels.txt"));
                candidates.Add(Path.Combine(directory, "labels.txt"));
            }

            foreach (string path in candidates)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        return ReadLabelsRobust(path);
                    }
                    catch
                    {
                        // 标签文件损坏时尝试下一个候选路径。
                    }
                }
            }
            return new string[0];
        }

        // ------------------------------------------------------------------
        // RGB 像素 → ONNX 输入（与训练端预处理一致，不依赖 HALCON）
        // ------------------------------------------------------------------

        private static float[] LetterboxAndNormalize(byte[] rgb, int width, int height, int size, int channels)
        {
            byte[] letterboxed = LetterboxRgb(rgb, width, height, size);
            return NormalizeToNchw(letterboxed, size, channels);
        }

        /// <summary>OpenCV 版等比例缩放 + 居中补边，与 Python 训练端 cv2 完全一致。</summary>
        private static byte[] LetterboxRgb(byte[] rgb, int width, int height, int size)
        {
            using (var source = new Mat(height, width, MatType.CV_8UC3))
            {
                for (var y = 0; y < height; y++)
                {
                    Marshal.Copy(rgb, y * width * 3, source.Ptr(y), width * 3);
                }

                var scale = Math.Min((double)size / width, (double)size / height);
                var resizedWidth = Math.Max(1, Math.Min(size, (int)Math.Round(width * scale)));
                var resizedHeight = Math.Max(1, Math.Min(size, (int)Math.Round(height * scale)));

                using (var resized = new Mat())
                {
                    Cv2.Resize(source, resized, new Size(resizedWidth, resizedHeight), 0, 0,
                        scale < 1.0 ? InterpolationFlags.Area : InterpolationFlags.Linear);

                    using (var canvas = new Mat(size, size, MatType.CV_8UC3, new Scalar(0, 0, 0)))
                    {
                        var rowOffset = (size - resizedHeight) / 2;
                        var columnOffset = (size - resizedWidth) / 2;
                        using (var roi = new Mat(canvas, new Rect(columnOffset, rowOffset, resizedWidth, resizedHeight)))
                        {
                            resized.CopyTo(roi);
                        }

                        var result = new byte[size * size * 3];
                        for (var y = 0; y < size; y++)
                        {
                            Marshal.Copy(canvas.Ptr(y), result, y * size * 3, size * 3);
                        }
                        return result;
                    }
                }
            }
        }

        /// <summary>RGB byte → NCHW float，归一化 (x/127.5 - 1)，与 PyTorch 训练一致。</summary>
        private static float[] NormalizeToNchw(byte[] rgb, int size, int channels)
        {
            var data = new float[channels * size * size];
            var pixels = size * size;
            for (var i = 0; i < pixels; i++)
            {
                var offset = i * 3;
                data[i] = rgb[offset] / 127.5f - 1.0f;                  // R
                data[pixels + i] = rgb[offset + 1] / 127.5f - 1.0f;    // G
                data[2 * pixels + i] = rgb[offset + 2] / 127.5f - 1.0f; // B
            }
            return data;
        }

        private static float[] Softmax(float[] logits)
        {
            var max = logits.Max();
            var exp = new float[logits.Length];
            double sum = 0;
            for (var i = 0; i < logits.Length; i++)
            {
                exp[i] = (float)Math.Exp(logits[i] - max);
                sum += exp[i];
            }
            for (var i = 0; i < exp.Length; i++)
            {
                exp[i] = (float)(exp[i] / sum);
            }
            return exp;
        }

        private static int ArgMax(float[] values)
        {
            var best = 0;
            for (var i = 1; i < values.Length; i++)
            {
                if (values[i] > values[best])
                    best = i;
            }
            return best;
        }
    }
}