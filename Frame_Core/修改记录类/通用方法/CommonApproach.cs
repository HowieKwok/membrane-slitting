using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.AxHost;

namespace Frame_Core
{
    /// <summary>
    /// 放一些比较通用的方法
    /// </summary>
    public  class CommonApproach
    {


        /// <summary>
        /// 且运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool LogicalAnd(bool a, bool b);
        /// <summary>
        /// 或运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        delegate bool LogicalOr(bool a, bool b);
        /// <summary>
        /// 逻辑运算
        /// </summary>
        /// <param name="conditionA">条件1</param>
        /// <param name="conditionB">条件2</param>
        /// <param name="operatorInput">运算符</param>
        /// <returns></returns>
        public static bool LogicalOperation(bool conditionA, bool conditionB, string operatorInput)
        {
            //不可漏 可以过杀
            bool result =true;
            try
            {
                LogicalAnd logicalAnd = null;
                LogicalOr logicalOr = null;

                if (operatorInput == "且")
                {
                    logicalAnd = (a, b) => a && b;
                }
                else if (operatorInput == "或")
                {
                    logicalOr = (a, b) => a || b;
                }
                else
                {
                     Log4netRecord.ErrorLog.Fatal("选择错误，请输入正确选项。");
                    return true;
                }



                if (logicalAnd != null)
                {
                    result = logicalAnd(conditionA, conditionB);
                }
                else if (logicalOr != null)
                {
                    result = logicalOr(conditionA, conditionB);
                }
                else
                {
                    // 处理不支持的选项
                    return true;
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);
                return true;
            }
            return result;
        }

        /// <summary>
        /// 创建文件夹及文件
        /// </summary>
        /// <param name="filepath">路径</param>
        public static void DetectIsExist(string filepath)
        {
            try
            {
                //获取路径文件名
                string str = Path.GetFileName(filepath);
                //如果没有文件名再看是否有目录路径
                if (str.Length == 0)
                {
                    //判断路径的目录是否存在
                    string path = Path.GetDirectoryName(filepath);
                    //如果目录也没存在
                    if (!Directory.Exists(path))
                    {
                        //没有则创建目录
                        Directory.CreateDirectory(path);
                    }
                }
                //如果有文件名
                else
                {
                    //返回目录路径信息
                    string path = Path.GetDirectoryName(filepath);
                    //判断是否有目录
                    if (!Directory.Exists(path))
                    {
                        //没有则创建目录
                        Directory.CreateDirectory(path);
                    }
                    string fillpath = Path.Combine(path, str);
                    if (!File.Exists(fillpath))
                        File.Create(fillpath).Close();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal("DetectIsExist 错误:" + ex.Message);
            }
        }





        /// <summary>
        /// 错误日志读写锁
        /// </summary>
        private static ReaderWriterLockSlim lockObject = new ReaderWriterLockSlim();
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void LogError(Exception ex)
        {
            StackTrace stackTrace = new StackTrace(ex, true);
            StackFrame frame = stackTrace.GetFrame(0);

            MethodBase callingMethod = frame.GetMethod();
            string className = callingMethod.ReflectedType.FullName;
            string methodName = callingMethod.Name;
            int lineNumber = frame.GetFileLineNumber();
            string fileName = frame.GetFileName();

            string errorMessage = string.Format("类 {0} 中的 {1} 方法第 {2} 行代码错误：", className, methodName, lineNumber);

            string codeSnippet = GetCodeSnippet(fileName, lineNumber);

            lockObject.EnterWriteLock();
            try
            {
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
                 Log4netRecord.ErrorLog.Fatal("错误的代码：\n" + codeSnippet);
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取当前方法名
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string getMethodName(Exception ex)
        {
            string methodName = "获取失败";
            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame frame = stackTrace.GetFrame(0);
                MethodBase callingMethod = frame.GetMethod();
                string className = callingMethod.ReflectedType.FullName;
                methodName = callingMethod.Name;
                return methodName;
            }
            catch
            {
                return methodName;
            }
            finally
            {
            }   
        }

        private static string GetCodeSnippet(string fileName, int lineNumber)
        {
            string[] lines = File.ReadAllLines(fileName);

            string codeSnippet = lines[lineNumber - 1];

            return codeSnippet;
        }


        /// <summary>
        /// 判断数据组是否与区间有交集
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool DoDataAndIntervalIntersect(double[] data, double start, double end)
        {
            
            if (data[0] <= end && data[1] >= start)
            {
                return true;
            }

            return false;
        }





        /// <summary>
        /// 进行像素与物理坐标的一维标定
        /// </summary>
        /// <param name="ImagePoint"></param>
        /// <param name="ArmPoint"></param>
        /// <param name="slope">斜率</param>
        /// <param name="intercept">截距</param>
        /// <param name="precision">误差</param>
        public static void PerformCalibration(double[] ImagePoint, double[] ArmPoint, out double slope, out double intercept, out double precision)
        {
            int n = ImagePoint.Length;

            // 计算均值
            double sumX = 0.0;
            double sumY = 0.0;
            for (int i = 0; i < n; i++)
            {
                sumX += ImagePoint[i];
                sumY += ArmPoint[i];
            }
            double xMean = sumX / n;
            double yMean = sumY / n;

            // 计算系数
            double sumXY = 0.0;
            double sumX2 = 0.0;
            for (int i = 0; i < n; i++)
            {
                sumXY += ImagePoint[i] * ArmPoint[i];
                sumX2 += ImagePoint[i] * ImagePoint[i];
            }
            slope = (sumXY - n * xMean * yMean) / (sumX2 - n * xMean * xMean);
            intercept = yMean - slope * xMean;

            // 计算精度估算
            double sumResidualsSquared = 0.0;
            for (int i = 0; i < n; i++)
            {
                double predictedY = slope * ImagePoint[i] + intercept;
                double residual = ArmPoint[i] - predictedY;
                sumResidualsSquared += residual * residual;
            }
            double meanResidualsSquared = sumResidualsSquared / n;
            precision = Math.Sqrt(meanResidualsSquared);
        }

        /// <summary>
        /// 将像素位置转换为物理位置
        /// </summary>
        /// <param name="ArmPoint"></param>
        /// <param name="slope">斜率</param>
        /// <param name="intercept">截距</param>
        /// <returns></returns>
        public static double ConvertPixelToPhysical(double ArmPoint, double slope, double intercept)
        {
            return slope * ArmPoint + intercept;
        }



        public static void test()
        {
            // 物理长度数组（单位为毫米）
            double[] physicalLengths = { 0, 10, 20, 30, 40, 50 };

            // 像素位置数组（例如在图像上测量的x坐标）
            double[] pixelPositions = { 101, 201, 301, 401, 501, 601 };

            // 标定参数
            double slope;
            double intercept;
            double precision;

            // 进行像素-物理坐标的一维标定
            PerformCalibration(pixelPositions, physicalLengths, out slope, out intercept, out precision);

            // 打印标定参数及精度估算
            Console.WriteLine("One-dimensional Calibration Parameters:");
            Console.WriteLine($"Slope: {slope}");
            Console.WriteLine($"Intercept: {intercept}");
            Console.WriteLine($"Precision: {precision}");

            // 测试像素位置
            double testPixelPosition = 300;
            double testPhysicalPosition = ConvertPixelToPhysical(testPixelPosition, slope, intercept);
        }


        /// <summary>
        /// 获取起始 到终止 时间
        /// </summary>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <returns></returns>
        public static double getTime(HTuple startTime,HTuple endTime)
        {
            double time = 0;
            time = (endTime - startTime)*1000;
            return time;
        }



        /// <summary>
        /// 缺陷检测用 字典中插入List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segments">字典</param>
        /// <param name="segmentNumber">Key值</param>
        /// <param name="defectType">Value值</param>
        /// <returns></returns>
        public static Dictionary<int, List<T>> InsertDefectType<T>(Dictionary<int, List<T>> segments, int segmentNumber, T defectType)
        {
            try
            {
                if (!segments.ContainsKey(segmentNumber))
                {
                    segments[segmentNumber] = new List<T> { defectType };
                }
                else
                {
                    segments[segmentNumber].Add(defectType);
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);
            }
            return segments;
        }


    }

    /// <summary>
    /// 卡尺找直线参数类
    /// </summary>
    public class FindLineParameter
    {
        /// <summary>
        /// 沿Y轴搜索,或者X轴
        /// </summary>
        public HTuple inputXorY = "Y";
        /// <summary>
        /// 间隔
        /// </summary>
        public HTuple inputInterval = 30;
        /// <summary>
        /// 方向 L2R_New从左到右,U2B_New从上到下
        /// </summary>
        public HTuple inputDirection = "L2R_New";
        /// <summary>
        /// 卡尺ROI长度
        /// </summary>
        public HTuple inputRoiWidthLen = 100;
        /// <summary>
        /// 平滑系数
        /// </summary>
        public HTuple inputSigma = 1;
        /// <summary>
        /// 阈值
        /// </summary>
        public HTuple inputAmplitudeThreshold = 10;
        /// <summary>
        /// ’negative‘从亮到暗，‘positive’从暗到亮，‘all’,
        /// </summary>
        public HTuple inputTransition = "negative";
        /// <summary>
        /// 'first'第一点,'last'最后一个,'all' 所有的边缘,取点方法
        /// </summary>
        public HTuple inputPosition = "first";

    }


    /// <summary>
    /// 直线类
    /// </summary>
    public class Line
    {
        /// <summary>
        /// 起点Y
        /// </summary>
        public double RowBegin;
        /// <summary>
        /// 起点X
        /// </summary>
        public double ColBegin;
        /// <summary>
        /// 终点Y
        /// </summary>
        public double RowEnd;
        /// <summary>
        /// 终点X
        /// </summary>
        public double ColEnd;

        /// <summary>
        /// 直线状态
        /// </summary>
        public bool state;


        public Line()
        {
            RowBegin = 0;
            ColBegin = 0;
            RowEnd = 0;
            ColEnd = 0;
            state = false;
        }


        // 拷贝构造函数
        public Line(Line other)
        {
            this.RowBegin = other.RowBegin;
            this.ColBegin = other.ColBegin;
            this.RowEnd = other.RowEnd;
            this.ColEnd = other.ColEnd;
            this.state = other.state;
        }

        // 克隆方法
        public Line Clone()
        {
            return new Line(this);
        }

    }


    /// <summary>
    /// 区域顶点类
    /// </summary>
    public class Rectangle_Point
    {
        /// <summary>
        /// 是否成功状态
        /// </summary>
        public bool state;

        /// <summary>
        /// 左上Y
        /// </summary>
        public double TopLeft_Row;
        /// <summary>
        /// 左上X
        /// </summary>
        public double TopLeft_Col;

        /// <summary>
        /// 右上Y
        /// </summary>
        public double TopRight_Row;
        /// <summary>
        /// 右上X
        /// </summary>
        public double TopRight_Col;


        /// <summary>
        /// 左下Y
        /// </summary>
        public double LowerLeft_Row;
        /// <summary>
        /// 左下X
        /// </summary>
        public double LowerLeft_Col;


        /// <summary>
        /// 右下Y
        /// </summary>
        public double LowerRight_Row;
        /// <summary>
        /// 右下X
        /// </summary>
        public double LowerRight_Col;

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width;

        /// <summary>
        /// 高度
        /// </summary>
        public double Height;

        /// <summary>
        /// V角带料
        /// </summary>
        public bool VJ_DaiLiao;

        /// <summary>
        /// V角带料区域
        /// </summary>
        public HObject VJ_DaiLiao_Region;

        /// <summary>
        /// V角带料面积
        /// </summary>
        public double VJ_DaiLiao_Area;

        public Rectangle_Point()
        {
            state = false;
            TopLeft_Row = 0;
            TopLeft_Col = 0;
            TopRight_Row = 0;
            TopRight_Col = 0;
            LowerRight_Row = 0;
            LowerRight_Col = 0;
            LowerLeft_Row = 0;
            LowerLeft_Col = 0;
            Width = 0;
            Height = 0;
            VJ_DaiLiao = false;

            HOperatorSet.GenEmptyObj(out VJ_DaiLiao_Region);
            VJ_DaiLiao_Area = 0;
        }

        ~Rectangle_Point()
        {
            VJ_DaiLiao_Region?.Dispose();

        }


    }









}
