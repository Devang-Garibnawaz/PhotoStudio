using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace PhotoStudio.Models
{
    public class ImageProcessing
    {
        public static bool InsertImages(string path)
        {
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(path);
                Image img = Image.FromStream(new MemoryStream(bytes));
                img = FixedSize(img, 630, 420);
                img.Save(path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool InsertImages(string path, bool ApplyWatermark)
        {
            bool result = false;
            try
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(path);
                Image img = Image.FromStream(new MemoryStream(bytes));
                img = FixedSize(img, 630, 420);
                img.Save(path);
                if (ApplyWatermark)
                {
                    result = AddWatermarkToImage(img, path);
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public static bool AddWatermarkToImage(Image ImagePhoto, string path)
        {
            try
            {
                using (Graphics g = Graphics.FromImage(ImagePhoto))
                {

                    // For Transparent Watermark Text 
                    int opacity = 128; // range from 0 to 255

                    //SolidBrush brush = new SolidBrush(Color.Red);
                    SolidBrush brush = new SolidBrush(Color.FromArgb(opacity, Color.White));
                    Font font = new Font("Arial", 18);
                    g.DrawString("InstaAlbum", font, brush, new PointF(0, 0));
                    ImagePhoto.Save(path);
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}