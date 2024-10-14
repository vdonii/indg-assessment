using INDG.Image.Service.Core.Models.Repository.S3;

namespace INDG.Image.Service.Core.Mapping.Custom
{
    public interface IAwsS3RepositoryRequestsMapper
    {
        PutObjectRepositoryRequest MapPutObjectRepositoryRequest(string key, byte[] data);
        GetObjectRepositoryRequest MapGetObjectRepositoryRequest(string key);
        DeleteObjectRepositoryRequest MapDeleteObjectRepositoryRequest(string key);
    }
}
