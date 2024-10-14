using INDG.Image.Service.Core.Models.Repository.S3;

namespace INDG.Image.Service.Core.Repositories
{
    public interface IAwsS3Repository
    {
        Task<PutObjectRepositoryResponse> PutObjectAsync(PutObjectRepositoryRequest putObjectRepositoryRequest);
        Task<GetObjectRepositoryResponse> GetObjectAsync(GetObjectRepositoryRequest getObjectRepositoryRequest);
        Task<DeleteObjectRepositoryResponse> DeleteObjectAsync(DeleteObjectRepositoryRequest deleteObjectRepositoryRequest);
    }
}
