using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using LogTool;

namespace Frame_Core
{
     public class FileOperationXls
    {
        /// <summary>
        /// 修改Xls文件数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetNum"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        /// <param name="value"></param>
        public void ChangeXls(string fileName, int sheetNum, int[] rowNum, int[] colNum, string[] value)
        {
            try
            {

                if (File.Exists(fileName))
                {
                    FileStream Fs;
                    HSSFWorkbook WorkBook;

                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//读取流
                    POIFSFileSystem ps = new POIFSFileSystem(fs);//需using NPOI.POIFS.FileSystem;
                    fs.Close();
                    fs.Dispose();
                    WorkBook = new HSSFWorkbook(ps);
                    Fs = new FileStream(fileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);//写入流

                    HSSFSheet sheet;
                    sheet = (HSSFSheet)WorkBook.GetSheet("sheet");
                    for (int i = 0; i < rowNum.Length; i++)
                    {
                        sheet.GetRow(rowNum[i]).GetCell(colNum[i]).SetCellValue(value[i]);
                    }
                    Fs.Flush();
                    WorkBook.Write(Fs);//写入文件  
                    Fs.Close();
                    Fs.Dispose();
                }

            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);           
            }
        }

        /// <summary>
        /// 写入XLS文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="NPOI_DataTitle">表头只写一次</param>
        /// <param name="NPOI_DataLine">行数据</param>
        public void WriteXls(string FilePath, List<string[]> NPOI_DataTitle, string[] NPOI_DataLine)
        {
            string TN = DateTime.Now.ToString("yyyyMMdd HH-mm-ss-fff");
            FileStream Fs;
            HSSFWorkbook WorkBook;
            IRow Row;
            ICell Cell;
            int sheetNum = 1;
            ISheet[] Sheet = new ISheet[sheetNum];
            int[] LastRowNum = new int[sheetNum];
            try
            {
                if (FilePath.Contains(".CSV"))
                    FilePath = FilePath.Replace(".CSV", ".xls");
                if (FilePath.Contains(".csv"))
                    FilePath = FilePath.Replace(".csv", ".xls");

                if (!File.Exists(FilePath))
                {
                    Fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    //===========================================================================================================================================
                    //=======【】
                    #region >>>设置表头
                    WorkBook = new HSSFWorkbook();//创建工作簿
                    for (int kk = 0; kk < NPOI_DataTitle.Count; kk++)
                    {

                        Sheet[kk] = WorkBook.CreateSheet("sheet");//创建新的工作表

                        for (int iRow = 0; iRow < NPOI_DataTitle[kk].Length; iRow++)//行
                        {
                            string[] array = NPOI_DataTitle[kk][iRow].Split(',');
                            Row = Sheet[kk].CreateRow(iRow);//创建新的一行
                            for (int iCol = 0; iCol < array.Length; iCol++)//列
                            {

                                Cell = Row.CreateCell(iCol);//为单元格赋值前要创建单元格
                                Cell.SetCellValue(array[iCol]);
                            }

                        }

                    }
                    #endregion
                }
                else
                {
                    //===========================================================================================================================================
                    //=======【】
                    #region 存在文件
                    FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);//读取流
                    POIFSFileSystem ps = new POIFSFileSystem(fs);//需using NPOI.POIFS.FileSystem;
                    fs.Close();
                    fs.Dispose();
                    WorkBook = new HSSFWorkbook(ps);
                    Fs = new FileStream(FilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);//写入流
                    #endregion
                }
                //===========================================================================================================================================
                //=======【】
                #region >>>设置单元格样式（红色背景）
                //HSSFPalette palette = WorkBook.GetCustomPalette(); //调色板实例
                //HSSFColor hssFColor = palette.FindColor((byte)255, (byte)0, (byte)0);
                //ICellStyle RedBackground = WorkBook.CreateCellStyle();//创建样式是基于HSSFWorkbook，而不是ISheet
                //RedBackground.FillPattern = FillPattern.SolidForeground;//老版本写FillPatternType.SOLID_FOREGROUND,新版本写FillPattern.SolidForeground
                //RedBackground.FillForegroundColor = hssFColor.Indexed;
                #endregion
                //===========================================================================================================================================
                //=======【】
                double tempdata = 0;
                bool bDouble = false;
                bool bInt = false;
                for (int iSheet = 0; iSheet < Sheet.Length; iSheet++)
                {
                    for (int iRow = 0; iRow < NPOI_DataLine.Length; iRow++)
                    {
                        try
                        {
                            string[] sTmps = NPOI_DataLine[iRow].Split(',');
                            if (sTmps.Length < 1) continue;
                            Sheet[iSheet] = WorkBook.GetSheet("sheet");//创建新的工作表
                            LastRowNum[iSheet] = Sheet[iSheet].LastRowNum;
                            Row = Sheet[iSheet].CreateRow(LastRowNum[iSheet] + 1);
                            for (int iCol = 0; iCol < sTmps.Length; iCol++)
                            {
                                bDouble = Regex.IsMatch(sTmps[iCol], @"^[+-]?\d*[.]?\d*$");//判断是否为double类型数字
                                bInt = Regex.IsMatch(sTmps[iCol], @"^[+-]?\d*[.]?\d*$");//判断是否为int类型数字
                                if (sTmps[iCol] == "")
                                {
                                    bDouble = false;
                                    bInt = false;
                                }
                                if (bDouble || bInt)
                                {

                                    tempdata = StringToDouble(sTmps[iCol]);
                                    Cell = Row.CreateCell(iCol);
                                    Cell.SetCellValue(tempdata);
                                }
                                else
                                {
                                    Cell = Row.CreateCell(iCol);
                                    Cell.SetCellValue(sTmps[iCol]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                             Log4netRecord.ErrorLog.Debug(ex);
                        }
                    }

                    //===========================================================================================================================================
                    //=======【】
                    if (!File.Exists(FilePath))
                    {
                        MemoryStream ms = new MemoryStream();//创建内存流用于写入文件       
                        WorkBook.Write(ms);//将Excel写入流
                        ms.Flush();
                        ms.Position = 0;
                        ms.WriteTo(Fs);//将流写入文件
                        Fs.Close();
                        Fs.Dispose();
                        ms.Close();
                        ms.Dispose();
                    }
                    else
                    {
                        Fs.Flush();
                        WorkBook.Write(Fs);//写入文件                                   
                    }
                }

                //===========================================================================================================================================
                //=======【】
                Fs.Close();
                Fs.Dispose();

            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug(ex);
            }
            finally
            {

            }

        }
        /// <summary>
        /// 字符串转为double类型数字
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        private  double StringToDouble(string inputStr)
        {
            double re = 0;
            try
            {
                re = Convert.ToDouble(inputStr);
            }
            catch (Exception ex)
            {
                re = -999999;
                //LogDebug.Debug(ex);
            }
            return re;
        }
    }
}
