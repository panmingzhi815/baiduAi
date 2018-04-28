using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace baiduAI
{
    public class Images
    {

        public static Bitmap ZoomImage(Bitmap SourceImage, int width, int height)
        {
            if (width == 0)
            {
                width = SourceImage.Width;
            }
            if (height == 0)
            {
                height = SourceImage.Height;
            }
            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            int IntX = 0;
            int IntY = 0;
            try
            {
                IntX = (bitmap.Width - SourceImage.Width) / 2;
                IntY = (bitmap.Height - SourceImage.Height) / 2;

                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Red);
                PointF drawPoint = new PointF(20.0F, 20.0F);
                g.DrawImage(SourceImage, IntX, IntY, SourceImage.Width, SourceImage.Height);
                g.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), drawFont, drawBrush, drawPoint);

                SourceImage.Dispose();
                drawFont.Dispose();
                drawBrush.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine("拉伸图片异常:" + ex.Message);
            }

            return bitmap;
        }


        public static Bitmap ZoomP(Bitmap a, float s, float v)
        {
            Bitmap bmp = new Bitmap((int)(a.Width * s), (int)(a.Height * v), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.ScaleTransform(s, v);
            g.DrawImage(a, 0, 0, a.Width, a.Height);
            return bmp;
        }

        public static byte[] bitmap2byte(Bitmap bitmap) {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] bytes = ms.ToArray();
                bitmap.Dispose();
                return bytes;
            }
        }

        public static Bitmap byte2bitmap(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return (Bitmap)Bitmap.FromStream(ms);
            }
        }
    }
}
