using System;
using OSGeo.GDAL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RGeos.Terrain
{
    public class DemHelper
    {
        private int width;
        private int height;
        private Band band;

        public void Start()
        {
            Gdal.AllRegister();
        }

        public void Read(string path)
        {
            Dataset ds = Gdal.Open(path, Access.GA_ReadOnly);
            band = ds.GetRasterBand(1);
            width = ds.RasterXSize;
            height = ds.RasterYSize;
        }
        /// <summary>
        /// 从影像中重采样提取Rows*Cols大小的影像
        /// </summary>
        /// <param name="Cols">列数</param>
        /// <param name="Rows">行数</param>
        /// <returns></returns>
        public RasterBandData ReadDate(int Cols, int Rows)
        {
            //获取dem信息
            double min, max, no;
            int hasvalue;
            band.GetMaximum(out max, out hasvalue);
            band.GetMinimum(out min, out hasvalue);
            band.GetNoDataValue(out no, out hasvalue);
            double[] data = new double[Cols * Rows];
            band.ReadRaster(0, 0, width, height, data, Cols, Rows, 0, 0);
            RasterBandData dataBand = new RasterBandData();
            dataBand.data = data;
            dataBand.Columns = Cols;
            dataBand.Rows = Rows;
            dataBand.NoDataValue = no;
            dataBand.MaxValue = max;
            dataBand.MinValue = min;
            return dataBand;
        }
        /// <summary>
        /// 创建8位灰度图
        /// </summary>
        /// <param name="Cols">列数</param>
        /// <param name="Rows">行数</param>
        /// <returns></returns>
        public Bitmap MakeGrayScale(int Cols, int Rows)
        {
            //新建8位灰度图
            Bitmap bitmap = new Bitmap(Cols, Rows, PixelFormat.Format8bppIndexed);
            BitmapData bmpdata = bitmap.LockBits(new Rectangle(0, 0, Cols, Rows),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //获取dem信息
            double min, max, no;
            int hasvalue;
            band.GetMaximum(out max, out hasvalue);
            band.GetMinimum(out min, out hasvalue);
            band.GetNoDataValue(out no, out hasvalue);
            double[] data = new double[Cols * Rows];
            band.ReadRaster(0, 0, width, height, data, Cols, Rows, 0, 0);

            //计算图像参数
            int offset = bmpdata.Stride - bmpdata.Width; //计算每行未用空间字节数
            IntPtr ptr = bmpdata.Scan0; //获取首地址
            int scanBytes = bmpdata.Stride * bmpdata.Height; //图像字节数= 扫描字节数 * 高度
            byte[] grayValues = new byte[scanBytes]; //为图像数据分配内存

            //为图像数据赋值
            int posSrc = 0, posScan = 0;
            for (int i = 0; i < Rows; i++) //row
            {
                for (int j = 0; j < Cols; j++) //col
                {
                    double item = data[posSrc++];
                    if (item == no)
                    {
                        grayValues[posScan++] = 0; //黑色
                    }
                    else
                    {
                        grayValues[posScan++] = (byte)((item - min) * 256 / (max - min));
                    }
                }
                //跳过图像数据每行未用空间的字节， length = stride - width * bytePerPixel
                posScan += offset;
            }

            Marshal.Copy(grayValues, 0, ptr, scanBytes);
            bitmap.UnlockBits(bmpdata);

            //修改生成位图的索引表，从伪彩修改为灰度
            ColorPalette palette;
            //获取一个Format8bppIndex格式图像的Palette对象
            using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                palette = bmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                palette.Entries[i] = Color.FromArgb(i, i, i);
            }
            //修改生成位图的索引表
            bitmap.Palette = palette;

            return bitmap;
        }
    }
}
