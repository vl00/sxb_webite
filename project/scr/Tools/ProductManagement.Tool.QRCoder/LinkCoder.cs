using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ProductManagement.Tool.QRCoder
{
    public class LinkCoder
    {
        /// <summary>
        /// 构建一个链接二维码
        /// </summary>
        /// <param name="link"></param>
        /// <param name="pixelSize"></param>
        /// <param name="icon">设置中心图标</param>
        /// <returns></returns>
        public Bitmap Create(string link, int pixelSize = 20, Bitmap icon = null)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.M);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(pixelSize, Color.Black, Color.White, icon);
            return qrCodeImage;
        }
    }
}
