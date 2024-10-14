using INDG.Image.Service.Core.Models.Repository.S3;

namespace INDG.Image.Service.Core.Mapping.Custom.Implementation
{
    public class AwsS3RepositoryRequestsMapper : IAwsS3RepositoryRequestsMapper
    {
        public PutObjectRepositoryRequest MapPutObjectRepositoryRequest(string key, byte[] data)
        {
            return new PutObjectRepositoryRequest
            {
                Key = key,
                Data = data
            };
        }

        public GetObjectRepositoryRequest MapGetObjectRepositoryRequest(string key)
        {
            return new GetObjectRepositoryRequest
            {
                Key = key
            };
        }

        public DeleteObjectRepositoryRequest MapDeleteObjectRepositoryRequest(string key)
        {
            return new DeleteObjectRepositoryRequest
            {
                Key = key
            };
        }
    }
}
