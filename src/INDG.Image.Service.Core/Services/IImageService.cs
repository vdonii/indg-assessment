using INDG.Image.Service.Core.Models;

namespace INDG.Image.Service.Core.Services
{
    public interface IImageService
    {
        Task<AddImageResponse> UploadImageAsync(AddImageRequest addImageRequest);
        Task<GetImageResponse> GetImageAsync(string id);
        Task<DeleteImageResponse> DeleteImageAsync(string id);
        Task<UpdateImageResponse> UpdateImageAsync(UpdateImageRequest updateImageRequest);
        Task<ResizeImageResponse> ResizeImageAsync(ResizeImageRequest resizeImageRequest);
    }
}
