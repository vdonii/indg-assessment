using INDG.Image.Service.Core.Models;

namespace INDG.Image.Service.Core.Mapping.Custom
{
    public interface IImageServiceResponsesMapper
    {
        AddImageResponse MapAddImageResponse(string id, bool result);
        GetImageResponse MapGetImageResponse(byte[] imageData);
        DeleteImageResponse MapDeleteImageResponse(bool result, string errorCode);
        UpdateImageResponse MapUpdateImageResponse(string id, bool result);
        ResizeImageResponse MapResizeImageResponse(string id, bool result);
    }
}
