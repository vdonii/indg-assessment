using EnsureThat;
using INDG.Image.Service.Core.Helpers;
using INDG.Image.Service.Core.Mapping.Custom;
using INDG.Image.Service.Core.Models;
using INDG.Image.Service.Core.Repositories;

namespace INDG.Image.Service.Core.Services.Implementation
{
    public class ImageService : IImageService
    {
        private IImageResizingService imageResizingService;
        private IAwsS3Repository awsS3Repository;
        private IImageHelper imageHelper;
        private IAwsS3RepositoryRequestsMapper awsS3RepositoryMapper;
        private IImageServiceResponsesMapper imageServiceResponsesMapper;

        public ImageService(IImageResizingService imageResizingService, 
            IAwsS3Repository awsS3Repository,
            IImageHelper imageHelper,
            IAwsS3RepositoryRequestsMapper awsS3RepositoryMapper,
            IImageServiceResponsesMapper imageServiceResponsesMapper)
        {
            this.imageResizingService = imageResizingService;
            this.awsS3Repository = awsS3Repository;
            this.imageHelper = imageHelper;
            this.awsS3RepositoryMapper = awsS3RepositoryMapper;
            this.imageServiceResponsesMapper = imageServiceResponsesMapper;
        }

        public async Task<AddImageResponse> UploadImageAsync(AddImageRequest addImageRequest)
        {
            EnsureArg.IsNotNull(addImageRequest, nameof(addImageRequest));

            var imageBytes = this.imageHelper.GetImageBytes(addImageRequest.File);

            var putRepositoryRequest = this.awsS3RepositoryMapper.MapPutObjectRepositoryRequest(Guid.NewGuid().ToString(), imageBytes);
            var result = await this.awsS3Repository.PutObjectAsync(putRepositoryRequest);

            return imageServiceResponsesMapper.MapAddImageResponse(result.Key, result.Result);
        }

        public async Task<GetImageResponse> GetImageAsync(string id)
        {
            EnsureArg.IsNotNullOrWhiteSpace(id, nameof(id));

            var getRepositoryRequest = this.awsS3RepositoryMapper.MapGetObjectRepositoryRequest(id);
            var result = await this.awsS3Repository.GetObjectAsync(getRepositoryRequest);

            return imageServiceResponsesMapper.MapGetImageResponse(result.Data);
        }

        public async Task<DeleteImageResponse> DeleteImageAsync(string id)
        {
            EnsureArg.IsNotNullOrWhiteSpace(id, nameof(id));

            var deleteRepositoryRequest = this.awsS3RepositoryMapper.MapDeleteObjectRepositoryRequest(id);
            var result = await this.awsS3Repository.DeleteObjectAsync(deleteRepositoryRequest);

            return imageServiceResponsesMapper.MapDeleteImageResponse(result.Result, result.ErrorCode);
        }

        public async Task<UpdateImageResponse> UpdateImageAsync(UpdateImageRequest updateImageRequest)
        {
            EnsureArg.IsNotNull(updateImageRequest, nameof(updateImageRequest));

            var imageBytes = this.imageHelper.GetImageBytes(updateImageRequest.File);

            var putRepositoryRequest = this.awsS3RepositoryMapper.MapPutObjectRepositoryRequest(updateImageRequest.Id, imageBytes);
            var result = await this.awsS3Repository.PutObjectAsync(putRepositoryRequest);

            return imageServiceResponsesMapper.MapUpdateImageResponse(result.Key, result.Result);
        }

        public async Task<ResizeImageResponse> ResizeImageAsync(ResizeImageRequest resizeImageRequest)
        {
            EnsureArg.IsNotNull(resizeImageRequest, nameof(resizeImageRequest));

            var getRepositoryRequest = this.awsS3RepositoryMapper.MapGetObjectRepositoryRequest(resizeImageRequest.Id);
            var result = await this.awsS3Repository.GetObjectAsync(getRepositoryRequest);

            var resizedImageBytes = this.imageResizingService.ResizeImage(result.Data, resizeImageRequest.Height);

            var putRepositoryRequest = this.awsS3RepositoryMapper.MapPutObjectRepositoryRequest(Guid.NewGuid().ToString(), resizedImageBytes);
            var putResult = await this.awsS3Repository.PutObjectAsync(putRepositoryRequest);

            return imageServiceResponsesMapper.MapResizeImageResponse(putResult.Key, putResult.Result);
        }

    }
}
