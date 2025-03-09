using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Helpers
{
    public static class Base64PicHelper
    {
        public static string? Base64_Pic(string base64)
        {
            if (base64.IndexOf(",") >= 0)
            {
                base64 = base64.Substring(base64.IndexOf(",") + 1);
            }
            byte[] bytes;
            string tempFile = CommonHelper.TempDir;
            try
            {
                bytes = Convert.FromBase64String(base64);
                tempFile = Path.Combine(tempFile, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png");
                File.WriteAllBytes(tempFile, bytes);
                return tempFile;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        public static string Pic_Base64(string filepath)
        {
            string res = "";
            using (FileStream fs = File.OpenRead(filepath))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                res = Convert.ToBase64String(bytes);
                res = "data:image/png;base64," + res;
                //如果文件超过1M，则将内容保存到桌面临时文件
                if (fs.Length > 1024 * 1024)
                {
                    string desktop = CommonHelper.desktopPath;
                    string tempFile = desktop + "\\temp.txt";
                    File.WriteAllText(tempFile, res);
                    res = $"文件大小超过1M，请到桌面查看临时文件，文件路径{tempFile}";
                }
            }
            return res;
        }
    }
}
