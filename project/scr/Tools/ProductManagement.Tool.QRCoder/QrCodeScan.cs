using System;
using System.Collections.Generic;
using System.Drawing;
using ZXing;
using ZXing.Common;
namespace ProductManagement.Tool.QRCoder
{
    public class QrCodeScan
    {
        public static string Scan(Bitmap bitmap)
        {
            DecodingOptions decodeOption = new DecodingOptions
            {
                PossibleFormats = new List<BarcodeFormat>() { BarcodeFormat.QR_CODE }
            };
            BarcodeReader br = new BarcodeReader
            {
                Options = decodeOption
            };
            Result result = br.Decode(bitmap);

            return result?.Text;
        }
    }
}
