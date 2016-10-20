using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace CJRProfileBlog.Models
{
    public static class ImageUploadValidator
    {
        public static object ImageFormatException { get; private set; }

        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }
            if (file.ContentLength> 2*1024*1024 || file.ContentLength < 1024)
            {
                return false;
            }
            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) || ImageFormat.Png.Equals(img.RawFormat) || ImageFormat.Gif.Equals(img.RawFormat) || ImageFormat.Bmp.Equals(img.RawFormat);
                }
            }
            catch
            {
                return false;
            }
        }
    }
}