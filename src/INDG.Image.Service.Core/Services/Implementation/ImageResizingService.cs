using INDG.Image.Service.Core.Helpers;

namespace INDG.Image.Service.Core.Services.Implementation
{
    public class ImageResizingService : IImageResizingService
    {
        private readonly IImageHelper imageHelper;

        public ImageResizingService(IImageHelper imageHelper) 
        {
            this.imageHelper = imageHelper;
        }

        public byte[] ResizeImage(byte[] originalImageBytes, int desiredHeight)
        {
            var image = this.imageHelper.GetImage(originalImageBytes);

            var ratio = this.imageHelper.GetRatio(image);
            var resizedImage = this.imageHelper.ResizeImage(image, desiredHeight, ratio);

            return this.imageHelper.GetImageBytes(resizedImage);
        }
    }
}
