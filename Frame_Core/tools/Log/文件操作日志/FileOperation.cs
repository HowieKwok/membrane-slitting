using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public class FileOperation
    {

        /// <summary>
        /// 初始化文件路径
        /// </summary>
        public void initPath()
        {
            //获取当前程序的工作目录
            string path;
            path = Application.StartupPath;
            path = AppDomain.CurrentDomain.BaseDirectory;
            path = Environment.CurrentDirectory;
            path = Directory.GetCurrentDirectory();

        }
        /// <summary>
        /// 复制文件到新路径
        /// </summary>
        public void CopyFile(string oldPath, string newPath)
        {
            try
            {
                FileInfo file_Data;

                if (File.Exists(oldPath))
                {
                    file_Data = new FileInfo(oldPath);
                    file_Data.CopyTo(newPath, true);
                }

            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Debug("Excel CopyFile" + ex);
            }
        }


        public void Temp()
        {
            string path="";
            OpenFileDialog openFileDialog = new OpenFileDialog();//打开文件对话框
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();// 打开文件夹浏览器


            if (openFileDialog.ShowDialog()==DialogResult.OK)
            {
                string sPath = openFileDialog.FileName;
            }

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string sPath = folderBrowserDialog.SelectedPath;
                string[] jpgfiles = System.IO.Directory.GetFiles(sPath, "*.jpg");
            }

            if (File.Exists(path))
            {

            }

        }
           
    }
}
