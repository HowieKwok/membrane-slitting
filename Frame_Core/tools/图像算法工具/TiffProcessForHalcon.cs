using BitMiracle.LibTiff.Classic;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class TiffProcessForHalcon
    {
        /// <summary>
        /// 将点云数据转化为halcon可读的tif格式图片，并保存到文件中
        /// </summary>
        /// <param name="inputData">三通道的XYZ数据，inputData的尺寸为M*N*3</param>
        /// <param name="filePath">tiff图像的路径</param>
        public static void WriteTiff(float[,,] inputData, string filePath)
        {
            if (filePath.Substring(filePath.Length - 4, 4) != ".tif")
            {
                return;
            }
            int width = inputData.GetLength(1);
            int height = inputData.GetLength(0);
            string fileName = filePath;
            using (Tiff output = Tiff.Open(fileName, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.PHOTOMETRIC, 1);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.SAMPLEFORMAT, 3);


                for (int i = 0; i < height; ++i)
                {
                    byte[] buf = new byte[width * 3 * sizeof(float)];
                    float[] tempData = new float[width * 3];
                    for (int j = 0; j < width; ++j)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            tempData[j * 3 + k] = inputData[i, j, k + 3] * 1000;
                        }
                    }
                    Buffer.BlockCopy(tempData, 0, buf, 0, buf.Length);
                    output.WriteScanline(buf, i);
                }
                output.Close();
            }
        }

        /// <summary>
        /// 将点云数据转化为halcon可读的tif格式图片，并保存到文件中
        /// </summary>
        /// <param name="dataX">X轴数据，数据格式为M*N*1</param>
        /// <param name="dataY">Y轴数据，数据格式为M*N*1</param>
        /// <param name="dataZ">Z轴数据，数据格式为M*N*1</param>
        /// <param name="filePath">tiff图像的路径</param>
        public static void WriteTiff(float[,,] dataX, float[,,] dataY, float[,,] dataZ, string filePath)
        {
            if (filePath.Substring(filePath.Length - 4, 4) != ".tif")
            {
                return;
            }
            int width = dataX.GetLength(1);
            int height = dataX.GetLength(0);
            string fileName = filePath;
            using (Tiff output = Tiff.Open(fileName, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, width);
                output.SetField(TiffTag.IMAGELENGTH, height);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                output.SetField(TiffTag.BITSPERSAMPLE, 32);
                output.SetField(TiffTag.PHOTOMETRIC, 1);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.SAMPLEFORMAT, 3);


                for (int i = 0; i < height; ++i)
                {
                    byte[] buf = new byte[width * 3 * sizeof(float)];
                    float[] tempData = new float[width * 3];
                    for (int j = 0; j < width; ++j)
                    {
                        tempData[j * 3 + 0] = dataX[i, j, 0];
                        tempData[j * 3 + 1] = dataY[i, j, 0];
                        tempData[j * 3 + 2] = dataZ[i, j, 0];
                    }
                    Buffer.BlockCopy(tempData, 0, buf, 0, buf.Length);
                    output.WriteScanline(buf, i);
                }
                output.Close();
            }
        }


       

        /// <summary>
        /// 将某个通道数据转为 halcon数据
        /// </summary>
        /// <param name="inputData">输入的源数据，格式为M*N*C</param>
        /// <param name="channelNum">将第几个通道转换为Halcon图像数据，小于C </param>
        /// <returns>halcon的源数据</returns>
        public static HObject Image2HObject(float[] inputData, int width,int  height)
        {
            try
            {
               
                unsafe
                {
                    fixed (float* ptrdata = inputData)
                    {
                        HObject ho;
                        HOperatorSet.GenImage1(out ho, "real", width, height, new IntPtr(ptrdata));
                        return ho;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 将某个通道数据转为 halcon数据
        /// </summary>
        /// <param name="inputData">输入的源数据，格式为M*N*C</param>
        /// <param name="channelNum">将第几个通道转换为Halcon图像数据，小于C </param>
        /// <returns>halcon的源数据</returns>
        public static HObject Image2HObject(double[] inputData, int width, int height)
        {
            try
            {

                unsafe
                {
                    fixed (double* ptrdata = inputData)
                    {
                        HObject ho;
                        HOperatorSet.GenImage1(out ho, "real", width, height, new IntPtr(ptrdata));
                        return ho;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
