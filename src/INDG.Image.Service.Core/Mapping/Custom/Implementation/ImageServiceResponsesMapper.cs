using INDG.Image.Service.Core.Models;

namespace INDG.Image.Service.Core.Mapping.Custom.Implementation
{
    public class ImageServiceResponsesMapper : IImageServiceResponsesMapper
    {
        public AddImageResponse MapAddImageResponse(string id, bool result)
        {
            return new AddImageResponse
            {
                Id = id,
                Result = result
            };
        }

        public DeleteImageResponse MapDeleteImageResponse(bool result, string errorCode)
        {
            return new DeleteImageResponse
            {
                Result = result,
                ErrorCode = errorCode
            };
        }

        public GetImageResponse MapGetImageResponse(byte[] imageData)
        {
            return new GetImageResponse
            {
                Data = imageData
            };
        }

        public ResizeImageResponse MapResizeImageResponse(string id, bool result)
        {
            return new ResizeImageResponse
            {
                Id = id,
                Result = result
            };
        }

        public UpdateImageResponse MapUpdateImageResponse(string id, bool result)
        {
            return new UpdateImageResponse
            {
                Id = id,
                Result = result
            };
        }
    }
}
