using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace INDG.Image.Service.Core.Helpers.Implementation
{
    public class ImageHelper : IImageHelper
    {
        public System.Drawing.Image GetImage(byte[] imageBytes)
        {
            using var ms = new MemoryStream(imageBytes);

            return System.Drawing.Image.FromStream(ms);
        }

        public byte[] GetImageBytes(System.Drawing.Image image)
        {
            ImageConverter converter = new();

            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        public byte[] GetImageBytes(IFormFile formFile)
        {
            using var stream = formFile.OpenReadStream();
            using var binaryReader = new BinaryReader(stream);

            return binaryReader.ReadBytes((int)stream.Length);
        }

        public float GetRatio(System.Drawing.Image image)
        {
            return image.Width / image.Height;
        }

        public System.Drawing.Image ResizeImage(System.Drawing.Image originalImage, int height, float ratio)
        {
            return new Bitmap(originalImage, new Size((int)(height * ratio), height));
        }
    }
}
