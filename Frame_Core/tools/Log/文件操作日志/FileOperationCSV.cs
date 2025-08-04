using LogTool;

using NPOI.SS.Formula.Functions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    public class FileOperationCSV
    {
        //初始化CSV写入类，刷新时间3秒，关闭时间30秒
        //即持续3秒没有对一个CSV写入内容，之前数据会被写入到硬盘中，持续30秒没有对一个CSV写入内容或开始写新的CSV文件，那原来的CSV文件会被关闭释放资源占用
        private static CsvWriter writer = new CsvWriter(5000, 30000);

        private static CsvWriter writer2 = new CsvWriter(5000, 30000) {IsAppend=false };

        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteData_CSV(string PATH, string[] dataTitles, List<string[]> data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }


            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                if (!File.Exists(fileFullPath))
                {
                    string tittle = "";
                    tittle = String.Join(",", dataTitles);
                    DstTxtString.Append(tittle);
                    writer.WriteLine(fileFullPath, DstTxtString);
                }


                //写入生产统计数据
                for (int i = 0; i < data.Count; i++)
                {
                    //清空字符串
                    DstTxtString.Clear();
                    string dataStr = "";
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        dataStr += data[i][j] + ",";
                    }

                    DstTxtString.Append(dataStr);
                    writer.WriteLine(fileFullPath, DstTxtString);
                }

                //File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteData_CSV(string PATH, string[] dataTitles, List<RowData[]> data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }


            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                if (!File.Exists(fileFullPath))
                {
                    string tittle = "";
                    tittle = String.Join(",", dataTitles);
                    DstTxtString.Append(tittle);
                    writer.WriteLine(fileFullPath, DstTxtString);
                }



                //写入生产统计数据
                for (int i = 0; i < data.Count; i++)
                {
                    //清空字符串
                    DstTxtString.Clear();
                    string dataStr = "";
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        dataStr += data[i][j].dataStr + ",";
                    }

                    DstTxtString.Append(dataStr);
                    writer.WriteLine(fileFullPath, DstTxtString);
                }

                File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteData_CSV(string PATH, string[] dataTitles, RowData[] data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }

            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                if (!File.Exists(fileFullPath))
                {

                    string tittle = "";
                    tittle = String.Join(",", dataTitles);
                    DstTxtString.Append(tittle);
                    writer.WriteLine(fileFullPath, DstTxtString);

                }



                //写入生产统计数据
                //清空字符串
                DstTxtString.Clear();
                string dataStr = "";
                for (int j = 0; j < data.Length; j++)
                {
                    dataStr += data[j].dataStr + ",";
                }

                DstTxtString.Append(dataStr);
                writer.WriteLine(fileFullPath, DstTxtString);

                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {

            }
        }
        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteData_CSV(string PATH, string[] dataTitles, string[] data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }


            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                if (!File.Exists(fileFullPath))
                {
                    string tittle = "";
                    tittle = String.Join(",", dataTitles);
                    DstTxtString.Append(tittle);
                    writer.WriteLine(fileFullPath, DstTxtString);
                }



                //写入生产统计数据

                //清空字符串
                DstTxtString.Clear();

                string dataStr = "";
                dataStr = String.Join(",", data);

                DstTxtString.Append(dataStr);
                writer.WriteLine(fileFullPath, DstTxtString);

                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {

            }
        }

        static object lock_write = new object();
        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteDataLock_CSV(string PATH, string[] dataTitles, string[] data, string source)
        {
            lock (lock_write)
            {
                string fileFullPath = PATH;//csv文件路径
                if (!fileFullPath.Contains(".csv"))
                {
                    fileFullPath = fileFullPath + ".csv";//csv文件路径
                }

                //打开文件
                try
                {
                    //创建拼接字符串类
                    StringBuilder DstTxtString_tittle = new StringBuilder();
                    StringBuilder DstTxtString = new StringBuilder();

                    //写入生产统计数据表头              
                    if (!File.Exists(fileFullPath))
                    {
                        string tittle = "";
                        tittle = String.Join(",", dataTitles);
                        DstTxtString.Append(tittle);
                        writer.WriteLine(fileFullPath, DstTxtString);
                    }



                    //写入生产统计数据

                    //清空字符串
                    DstTxtString.Clear();

                    string dataStr = "";
                    dataStr = String.Join(",", data);

                    DstTxtString.Append(dataStr);
                    writer.WriteLine(fileFullPath, DstTxtString);

                    //清空字符串
                    DstTxtString.Clear();


                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(source, ex);
                }
                finally
                {

                }
            }

        }

        static object lock_write1 = new object();
        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>    
        /// <param name="PATH">路径文件夹</param>
        /// <param name="dataTitles">数据表头</param>
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteDataLock1_CSV(string PATH, string[] dataTitles, string[] data, string source)
        {
            lock (lock_write1)
            {
                string fileFullPath = PATH;//csv文件路径
                if (!fileFullPath.Contains(".csv"))
                {
                    fileFullPath = fileFullPath + ".csv";//csv文件路径
                }


                //打开文件
                try
                {
                    //创建拼接字符串类
                    StringBuilder DstTxtString_tittle = new StringBuilder();
                    StringBuilder DstTxtString = new StringBuilder();

                    //写入生产统计数据表头              
                    if (!File.Exists(fileFullPath))
                    {
                        string tittle = "";
                        tittle = String.Join(",", dataTitles);
                        DstTxtString.Append(tittle);
                        writer.WriteLine(fileFullPath, DstTxtString);
                    }



                    //写入生产统计数据

                    //清空字符串
                    DstTxtString.Clear();

                    string dataStr = "";
                    dataStr = String.Join(",", data);

                    DstTxtString.Append(dataStr);
                    writer.WriteLine(fileFullPath, DstTxtString);

                    //清空字符串
                    DstTxtString.Clear();


                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Debug(source, ex);
                }
                finally
                {

                }
            }

        }

        /// <summary>
        /// 写CSV操作生产数据
        /// </summary>
        /// <param name="PATH">路径文件夹</param>   
        /// <param name="data">数据</param>
        /// <param name="source">来源</param>
        public static void WriteData_CSV(string PATH, List<string[]> data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }


            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                //if (!File.Exists(fileFullPath))
                //{
                //    //创建文件流对象，无则创建,有则追加
                //    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
                //    //创建文件流写入对象，绑定文件流对象
                //    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
                //    File.SetAttributes(fileFullPath, FileAttributes.Normal);

                //}
                //else
                //{
                //    //创建文件流对象，无则创建,有则追加(覆盖掉原有文件)
                //    File.SetAttributes(fileFullPath, FileAttributes.Normal);
                //    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
                //    //创建文件流写入对象，绑定文件流对象
                //    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
                //}



                //写入生产统计数据
                for (int i = 0; i < data.Count; i++)
                {
                    //清空字符串
                    DstTxtString.Clear();
                    string dataStr = "";
                    dataStr = String.Join(",", data[i]);
                    DstTxtString.Append(dataStr);
                    writer2.WriteLine(fileFullPath, DstTxtString);
                }

                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {

            }
        }


        public static void WriteData_CSV2(string PATH, List<string[]> data, string source)
        {

            string fileFullPath = PATH;//csv文件路径
            if (!fileFullPath.Contains(".csv"))
            {
                fileFullPath = fileFullPath + ".csv";//csv文件路径
            }

            FileStream CsvFileStream;               //CSV文件流
            StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

            //打开文件
            try
            {
                //创建拼接字符串类
                StringBuilder DstTxtString_tittle = new StringBuilder();
                StringBuilder DstTxtString = new StringBuilder();

                //写入生产统计数据表头              
                if (!File.Exists(fileFullPath))
                {
                    //创建文件流对象，无则创建,有则追加
                    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
                    //创建文件流写入对象，绑定文件流对象
                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
                    File.SetAttributes(fileFullPath, FileAttributes.Normal);

                }
                else
                {
                    //创建文件流对象，无则创建,有则追加(覆盖掉原有文件)
                    File.SetAttributes(fileFullPath, FileAttributes.Normal);
                    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
                    //创建文件流写入对象，绑定文件流对象
                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
                }



                //写入生产统计数据
                for (int i = 0; i < data.Count; i++)
                {
                    //清空字符串
                    DstTxtString.Clear();
                    string dataStr = "";
                    dataStr = String.Join(",", data[i]);
                    DstTxtString.Append(dataStr);
                    CsvTxtWriter.WriteLine(DstTxtString);
                }

                File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
                //清空字符串
                DstTxtString.Clear();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(source, ex);
            }
            finally
            {
                if (CsvTxtWriter != null)
                {
                    //写完缓存中的数据
                    CsvTxtWriter.Flush();

                    //关闭文件
                    CsvTxtWriter.Close();

                    CsvTxtWriter.Dispose();
                }
            }
        }


        public List<String[]> ReadCSV(string filePathName)
        {
            List<String[]> ls = new List<String[]>();
            StreamReader fileReader = new StreamReader(filePathName, Encoding.UTF8);
            string strLine = "";
            while (strLine != null)
            {
                strLine = fileReader.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                    ls.Add(strLine.Split(','));
                    //Debug.WriteLine(strLine);
                }
            }
            fileReader.Close();
            return ls;
        }
    }



    //public class FileOperationCSV
    //{
    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteData_CSV(string PATH, string[] dataTitles, List<string[]> data, string source)
    //    {

    //        string fileFullPath = PATH;//csv文件路径
    //        if (!fileFullPath.Contains(".csv"))
    //        {
    //            fileFullPath = fileFullPath + ".csv";//csv文件路径
    //        }

    //        FileStream CsvFileStream;               //CSV文件流
    //        StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //        //打开文件
    //        try
    //        {
    //            //创建拼接字符串类
    //            StringBuilder DstTxtString_tittle = new StringBuilder();
    //            StringBuilder DstTxtString = new StringBuilder();

    //            //写入生产统计数据表头              
    //            if (!File.Exists(fileFullPath))
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                string tittle = "";
    //                tittle = String.Join(",", dataTitles);
    //                DstTxtString.Append(tittle);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }
    //            else
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //            }



    //            //写入生产统计数据
    //            for (int i = 0; i < data.Count; i++)
    //            {
    //                //清空字符串
    //                DstTxtString.Clear();
    //                string dataStr = "";
    //                for (int j = 0; j < data[i].Length; j++)
    //                {
    //                    dataStr += data[i][j] + ",";
    //                }

    //                DstTxtString.Append(dataStr);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }

    //            File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //            //清空字符串
    //            DstTxtString.Clear();


    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Debug(source, ex);
    //        }
    //        finally
    //        {
    //            if (CsvTxtWriter != null)
    //            {
    //                //写完缓存中的数据
    //                CsvTxtWriter.Flush();

    //                //关闭文件
    //                CsvTxtWriter.Close();

    //                CsvTxtWriter.Dispose();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteData_CSV(string PATH, string[] dataTitles, List<RowData[]> data, string source)
    //    {

    //        string fileFullPath = PATH;//csv文件路径
    //        if (!fileFullPath.Contains(".csv"))
    //        {
    //            fileFullPath = fileFullPath + ".csv";//csv文件路径
    //        }

    //        FileStream CsvFileStream;               //CSV文件流
    //        StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //        //打开文件
    //        try
    //        {
    //            //创建拼接字符串类
    //            StringBuilder DstTxtString_tittle = new StringBuilder();
    //            StringBuilder DstTxtString = new StringBuilder();

    //            //写入生产统计数据表头              
    //            if (!File.Exists(fileFullPath))
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                string tittle = "";
    //                tittle = String.Join(",", dataTitles);
    //                DstTxtString.Append(tittle);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }
    //            else
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //            }



    //            //写入生产统计数据
    //            for (int i = 0; i < data.Count; i++)
    //            {
    //                //清空字符串
    //                DstTxtString.Clear();
    //                string dataStr = "";
    //                for (int j = 0; j < data[i].Length; j++)
    //                {
    //                    dataStr += data[i][j].dataStr + ",";
    //                }

    //                DstTxtString.Append(dataStr);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }

    //            File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //            //清空字符串
    //            DstTxtString.Clear();


    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Debug(source, ex);
    //        }
    //        finally
    //        {
    //            if (CsvTxtWriter != null)
    //            {
    //                //写完缓存中的数据
    //                CsvTxtWriter.Flush();

    //                //关闭文件
    //                CsvTxtWriter.Close();

    //                CsvTxtWriter.Dispose();
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteData_CSV(string PATH, string[] dataTitles, RowData[] data, string source)
    //    {


    //        string fileFullPath = PATH;//csv文件路径
    //        if (!fileFullPath.Contains(".csv"))
    //        {
    //            fileFullPath = fileFullPath + ".csv";//csv文件路径
    //        }

    //        FileStream CsvFileStream;               //CSV文件流
    //        StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //        //打开文件
    //        try
    //        {
    //            //创建拼接字符串类
    //            StringBuilder DstTxtString_tittle = new StringBuilder();
    //            StringBuilder DstTxtString = new StringBuilder();

    //            //写入生产统计数据表头              
    //            if (!File.Exists(fileFullPath))
    //            {

    //                //创建文件流对象，无则创建,有则追加
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                string tittle = "";
    //                tittle = String.Join(",", dataTitles);
    //                DstTxtString.Append(tittle);
    //                CsvTxtWriter.WriteLine(DstTxtString);

    //            }
    //            else
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码

    //            }



    //            //写入生产统计数据
    //            //清空字符串
    //            DstTxtString.Clear();
    //            string dataStr = "";
    //            for (int j = 0; j < data.Length; j++)
    //            {
    //                dataStr += data[j].dataStr + ",";
    //            }

    //            DstTxtString.Append(dataStr);
    //            CsvTxtWriter.WriteLine(DstTxtString);

    //            File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //            //清空字符串
    //            DstTxtString.Clear();


    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Debug(source, ex);
    //        }
    //        finally
    //        {
    //            if (CsvTxtWriter != null)
    //            {
    //                //写完缓存中的数据
    //                CsvTxtWriter.Flush();

    //                //关闭文件
    //                CsvTxtWriter.Close();

    //                CsvTxtWriter.Dispose();
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteData_CSV(string PATH, string[] dataTitles, string[] data, string source)
    //    {

    //        string fileFullPath = PATH;//csv文件路径
    //        if (!fileFullPath.Contains(".csv"))
    //        {
    //            fileFullPath = fileFullPath + ".csv";//csv文件路径
    //        }

    //        FileStream CsvFileStream;               //CSV文件流
    //        StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //        //打开文件
    //        try
    //        {
    //            //创建拼接字符串类
    //            StringBuilder DstTxtString_tittle = new StringBuilder();
    //            StringBuilder DstTxtString = new StringBuilder();

    //            //写入生产统计数据表头              
    //            if (!File.Exists(fileFullPath))
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                string tittle = "";
    //                tittle = String.Join(",", dataTitles);
    //                DstTxtString.Append(tittle);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }
    //            else
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //            }



    //            //写入生产统计数据

    //            //清空字符串
    //            DstTxtString.Clear();

    //            string dataStr = "";
    //            dataStr = String.Join(",", data);

    //            DstTxtString.Append(dataStr);
    //            CsvTxtWriter.WriteLine(DstTxtString);

    //            File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //            //清空字符串
    //            DstTxtString.Clear();


    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Debug(source, ex);
    //        }
    //        finally
    //        {
    //            if (CsvTxtWriter != null)
    //            {
    //                //写完缓存中的数据
    //                CsvTxtWriter.Flush();

    //                //关闭文件
    //                CsvTxtWriter.Close();

    //                CsvTxtWriter.Dispose();
    //            }
    //        }
    //    }

    //    static object lock_write =new object();
    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteDataLock_CSV(string PATH, string[] dataTitles, string[] data, string source)
    //    {
    //        lock (lock_write)
    //        {
    //            string fileFullPath = PATH;//csv文件路径
    //            if (!fileFullPath.Contains(".csv"))
    //            {
    //                fileFullPath = fileFullPath + ".csv";//csv文件路径
    //            }

    //            FileStream CsvFileStream;               //CSV文件流
    //            StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //            //打开文件
    //            try
    //            {
    //                //创建拼接字符串类
    //                StringBuilder DstTxtString_tittle = new StringBuilder();
    //                StringBuilder DstTxtString = new StringBuilder();

    //                //写入生产统计数据表头              
    //                if (!File.Exists(fileFullPath))
    //                {
    //                    //创建文件流对象，无则创建,有则追加
    //                    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                    //创建文件流写入对象，绑定文件流对象
    //                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                    File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                    string tittle = "";
    //                    tittle = String.Join(",", dataTitles);
    //                    DstTxtString.Append(tittle);
    //                    CsvTxtWriter.WriteLine(DstTxtString);
    //                }
    //                else
    //                {
    //                    //创建文件流对象，无则创建,有则追加
    //                    File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                    CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                    //创建文件流写入对象，绑定文件流对象
    //                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                }



    //                //写入生产统计数据

    //                //清空字符串
    //                DstTxtString.Clear();

    //                string dataStr = "";
    //                dataStr = String.Join(",", data);

    //                DstTxtString.Append(dataStr);
    //                CsvTxtWriter.WriteLine(DstTxtString);

    //                File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //                //清空字符串
    //                DstTxtString.Clear();


    //            }
    //            catch (Exception ex)
    //            {
    //                Log4netRecord.ErrorLog.Debug(source, ex);
    //            }
    //            finally
    //            {
    //                if (CsvTxtWriter != null)
    //                {
    //                    //写完缓存中的数据
    //                    CsvTxtWriter.Flush();

    //                    //关闭文件
    //                    CsvTxtWriter.Close();

    //                    CsvTxtWriter.Dispose();
    //                }
    //            }
    //        }

    //    }

    //    static object lock_write1 = new object();
    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>    
    //    /// <param name="PATH">路径文件夹</param>
    //    /// <param name="dataTitles">数据表头</param>
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteDataLock1_CSV(string PATH, string[] dataTitles, string[] data, string source)
    //    {
    //        lock (lock_write1)
    //        {
    //            string fileFullPath = PATH;//csv文件路径
    //            if (!fileFullPath.Contains(".csv"))
    //            {
    //                fileFullPath = fileFullPath + ".csv";//csv文件路径
    //            }

    //            FileStream CsvFileStream;               //CSV文件流
    //            StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //            //打开文件
    //            try
    //            {
    //                //创建拼接字符串类
    //                StringBuilder DstTxtString_tittle = new StringBuilder();
    //                StringBuilder DstTxtString = new StringBuilder();

    //                //写入生产统计数据表头              
    //                if (!File.Exists(fileFullPath))
    //                {
    //                    //创建文件流对象，无则创建,有则追加
    //                    CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                    //创建文件流写入对象，绑定文件流对象
    //                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                    File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                    string tittle = "";
    //                    tittle = String.Join(",", dataTitles);
    //                    DstTxtString.Append(tittle);
    //                    CsvTxtWriter.WriteLine(DstTxtString);
    //                }
    //                else
    //                {
    //                    //创建文件流对象，无则创建,有则追加
    //                    File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                    CsvFileStream = new FileStream(fileFullPath, FileMode.Append, FileAccess.Write);
    //                    //创建文件流写入对象，绑定文件流对象
    //                    CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                }



    //                //写入生产统计数据

    //                //清空字符串
    //                DstTxtString.Clear();

    //                string dataStr = "";
    //                dataStr = String.Join(",", data);

    //                DstTxtString.Append(dataStr);
    //                CsvTxtWriter.WriteLine(DstTxtString);

    //                File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //                //清空字符串
    //                DstTxtString.Clear();


    //            }
    //            catch (Exception ex)
    //            {
    //                Log4netRecord.ErrorLog.Debug(source, ex);
    //            }
    //            finally
    //            {
    //                if (CsvTxtWriter != null)
    //                {
    //                    //写完缓存中的数据
    //                    CsvTxtWriter.Flush();

    //                    //关闭文件
    //                    CsvTxtWriter.Close();

    //                    CsvTxtWriter.Dispose();
    //                }
    //            }
    //        }

    //    }

    //    /// <summary>
    //    /// 写CSV操作生产数据
    //    /// </summary>
    //    /// <param name="PATH">路径文件夹</param>   
    //    /// <param name="data">数据</param>
    //    /// <param name="source">来源</param>
    //    public static void WriteData_CSV(string PATH, List<string[]> data, string source)
    //    {

    //        string fileFullPath = PATH;//csv文件路径
    //        if (!fileFullPath.Contains(".csv"))
    //        {
    //            fileFullPath = fileFullPath + ".csv";//csv文件路径
    //        }

    //        FileStream CsvFileStream;               //CSV文件流
    //        StreamWriter CsvTxtWriter = null;              //CSV TXT文件操作类    

    //        //打开文件
    //        try
    //        {
    //            //创建拼接字符串类
    //            StringBuilder DstTxtString_tittle = new StringBuilder();
    //            StringBuilder DstTxtString = new StringBuilder();

    //            //写入生产统计数据表头              
    //            if (!File.Exists(fileFullPath))
    //            {
    //                //创建文件流对象，无则创建,有则追加
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);

    //            }
    //            else
    //            {
    //                //创建文件流对象，无则创建,有则追加(覆盖掉原有文件)
    //                File.SetAttributes(fileFullPath, FileAttributes.Normal);
    //                CsvFileStream = new FileStream(fileFullPath, FileMode.Create, FileAccess.Write);
    //                //创建文件流写入对象，绑定文件流对象
    //                CsvTxtWriter = new StreamWriter(CsvFileStream, Encoding.UTF8);//中文编码
    //            }



    //            //写入生产统计数据
    //            for (int i = 0; i < data.Count; i++)
    //            {
    //                //清空字符串
    //                DstTxtString.Clear();
    //                string dataStr = "";
    //                dataStr = String.Join(",", data[i]);
    //                DstTxtString.Append(dataStr);
    //                CsvTxtWriter.WriteLine(DstTxtString);
    //            }

    //            File.SetAttributes(fileFullPath, FileAttributes.ReadOnly);
    //            //清空字符串
    //            DstTxtString.Clear();


    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Debug(source, ex);
    //        }
    //        finally
    //        {
    //            if (CsvTxtWriter != null)
    //            {
    //                //写完缓存中的数据
    //                CsvTxtWriter.Flush();

    //                //关闭文件
    //                CsvTxtWriter.Close();

    //                CsvTxtWriter.Dispose();
    //            }
    //        }
    //    }

    //    public List<String[]> ReadCSV(string filePathName)
    //    {
    //        List<String[]> ls = new List<String[]>();
    //        StreamReader fileReader = new StreamReader(filePathName, Encoding.UTF8);
    //        string strLine = "";
    //        while (strLine != null)
    //        {
    //            strLine = fileReader.ReadLine();
    //            if (strLine != null && strLine.Length > 0)
    //            {
    //                ls.Add(strLine.Split(','));
    //                //Debug.WriteLine(strLine);
    //            }
    //        }
    //        fileReader.Close();
    //        return ls;
    //    }
    //}



}
