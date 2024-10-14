namespace INDG.Image.Service.Core.Services
{
    public interface IImageResizingService
    {
        byte[] ResizeImage(byte[] originalImage, int desiredHeight);
    }
}
