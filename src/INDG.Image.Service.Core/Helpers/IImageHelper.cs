using Microsoft.AspNetCore.Http;

namespace INDG.Image.Service.Core.Helpers
{
    public interface IImageHelper
    {
        System.Drawing.Image GetImage(byte[] imageData);
        float GetRatio(System.Drawing.Image image);
        System.Drawing.Image ResizeImage(System.Drawing.Image originalImage, int height, float ratio);
        byte[] GetImageBytes(System.Drawing.Image image);
        byte[] GetImageBytes(IFormFile formFile);
    }
}
