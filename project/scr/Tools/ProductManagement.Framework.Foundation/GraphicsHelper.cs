using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Framework.Foundation
{
    public static class GraphicsHelper
    {
        public static async Task<MemoryStream> ComposeImages(Image baseImage, Image qrcodeImage, int x, int y, int width, int height)
        {
            if (baseImage == null || qrcodeImage == null)
            {
                throw new ArgumentNullException();
            }

            //指定位置画二维码
            using (Graphics graphics = Graphics.FromImage(baseImage))
            {
                //二维码图片
                graphics.DrawImage(qrcodeImage, x, y, width, height);
            }

            var ms = new MemoryStream();
            baseImage.Save(ms, ImageFormat.Png);

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static MemoryStream CreateWangKeTongInviterHaiBao(
            Image baseImage
            , string nickName
            , Image headImage
            , Image qrCode)
        {
            try
            {
                //指定位置画二维码
                using (Graphics graphics = Graphics.FromImage(baseImage))
                {

                    var  headImageRectangle = new Rectangle(85, 1255, 53, 53);
                    var qrCodeImageRectangle = new Rectangle(501, 1144, 165, 165);

                    Image newHeadImage = CutEllipse(headImage, new Rectangle(0, 0, headImage.Width, headImage.Height), new Size(55, 55));
                    //画头像
                    graphics.DrawImage(newHeadImage, headImageRectangle);
                    //画昵称
                    Font font = new Font("宋体", 18);
                    //#191919
                    SolidBrush solidBrush = new SolidBrush(Color.FromArgb(25, 25, 25));
                    //if (nickName == null)
                    //{
                    //    nickName = "";
                    //}
                    //if (nickName.Length > 5)
                    //{
                    //    nickName = $"{nickName[0]}***{nickName[nickName.Length - 1]}";
                    //}
                    //graphics.DrawString("邀请您一同参加", font, solidBrush, new PointF(160f, 1241.4f));
                    //二维码图片
                    graphics.DrawImage(qrCode, qrCodeImageRectangle);

                }

                var ms = new MemoryStream();
                baseImage.Save(ms, ImageFormat.Png);

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch
            {
                return null;
            }
            finally
            {

                baseImage.Dispose();
                headImage.Dispose();
                qrCode.Dispose();
            }

        }

        public static MemoryStream CreateWangKeTongGuWenHaiBao(
       Image baseImage
       ,Image headImage
       ,Rectangle headImageRectangle
       ,Image qrCode
       ,Rectangle qrCodeRectangle)
        {
            try
            {
                //指定位置画二维码
                using (Graphics graphics = Graphics.FromImage(baseImage))
                {
                    //string timerange = $"有效期：{startTime.ToString("yyyy-MM-dd HH:mm:ss")}至{endTime.ToString("yyyy-MM-dd HH:mm:ss")}";
                    ////画昵称
                    //Font font = new Font("宋体", 12);
                    ////画有效期
                    //graphics.DrawString(timerange, font, Brushes.Black, new PointF(60f, 150f));

                    //二维码大小尺寸：180 * 180
                    //二维码位置：x = 523 y = 1502
                    //头像大小尺寸：48 * 48
                    //头像位置：x = 73 y = 1738
                    if (headImage != null)
                    {
                        //画头像
                        graphics.DrawImage(headImage, headImageRectangle);
                    }
                    //二维码图片
                    graphics.DrawImage(qrCode, qrCodeRectangle);
                }
                var ms = new MemoryStream();
                baseImage.Save(ms, ImageFormat.Png); 

                ms.Seek(0, SeekOrigin.Begin);
                return ms;
            }
            catch
            {
                return null;
            }
            finally
            {

                baseImage.Dispose();
                qrCode.Dispose();
            }

        }


        public static Image CutEllipse(Image img, Rectangle rec, Size size)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (TextureBrush br = new TextureBrush(img, System.Drawing.Drawing2D.WrapMode.Clamp, rec))
                {
                    br.ScaleTransform(bitmap.Width / (float)rec.Width, bitmap.Height / (float)rec.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillEllipse(br, new Rectangle(Point.Empty, size));
                }
            }
            return bitmap;
        }



        /// <summary>
        /// 从Url中获取图片
        /// </summary>
        /// <param name="image"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Image> ImageFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var dataStream = await client.GetStreamAsync(url);
                return Image.FromStream(dataStream);
            }
        }


        public static string GetMD5(this Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                return MD5Helper.GetSimpleMD5(ms);
            }


        }




    }
}
