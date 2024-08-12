using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Web.Utils
{
    public static class GetImageExtHelper
    {

        /// <summary>
        /// 字节数组转换成图片
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image byte2img(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                Image img = Image.FromStream(ms);
                ms.Close();
                return img;
            }
        }

        /// <summary>
        /// 获取图片后缀
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string GetImageExt(Image image)
        {
            string imageExt = "";
            var RawFormatGuid = image.RawFormat.Guid;
            if (ImageFormat.Png.Guid == RawFormatGuid)
            {
                imageExt = "png";
            }
            if (ImageFormat.Jpeg.Guid == RawFormatGuid)
            {
                imageExt = "jpg";
            }
            if (ImageFormat.Bmp.Guid == RawFormatGuid)
            {
                imageExt = "bmp";
            }
            if (ImageFormat.Gif.Guid == RawFormatGuid)
            {
                imageExt = "gif";
            }
            if (ImageFormat.Icon.Guid == RawFormatGuid)
            {
                imageExt = "icon";
            }
            return imageExt;
        }
    }
}
