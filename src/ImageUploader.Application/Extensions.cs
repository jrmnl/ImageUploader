using System;
using System.Drawing;
using System.IO;

namespace ImageUploader.Application
{
    public static class Extensions
    {
        public static Image ToImage(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Image.FromStream(stream);
            }
        }

        public static Image ToImageFromBase64(this string base64)
        {
            return Convert.FromBase64String(base64).ToImage();
        }
    }
}
