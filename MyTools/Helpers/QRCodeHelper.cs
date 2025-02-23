using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace MyTools.Helpers
{
    public class QRCodeHelper
    {
        /// <summary>
        /// 解析二维码
        /// </summary>
        public static string DeQrCode(string path)
        {
            var barcodeReader = new BarcodeReader();
            barcodeReader.Options.TryHarder = true; // 可选：增加尝试解码难以读取的图像
            Result res;
            using (Bitmap bitmap = new Bitmap(path))
            {
                res = barcodeReader.Decode(bitmap);
            }
            return res?.Text;
        }
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="qrCodeContent">内容</param>
        public static string CreateZXingQrCode(string qrCodeContent, int Width, int Height, int Margin)
        {
            string path = Path.Combine(CommonHelper.CurrectDir, CommonHelper.TempDir, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png ");
            //设置规格
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                Width = Width,//设置二维码的宽度
                Height = Height,//设置二维码的高度
                Margin = Margin,//设置二维码的边距
                CharacterSet = "UTF-8",//设置内容编码
                DisableECI = true,
            };
            //生成二维码
            BarcodeWriter writer = new BarcodeWriter()
            {
                Format = ZXing.BarcodeFormat.QR_CODE,//生成二维码
                //Format = ZXing.BarcodeFormat.CODE_128,//生成条码
                Options = options,//设置格式
            };
            using (Bitmap bitmap = writer.Write(qrCodeContent))
            {
                bitmap.Save(path);
            }
            return path;
        }
    }
}
