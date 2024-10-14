using Amazon.S3;
using Amazon.S3.Model;
using EnsureThat;
using INDG.Image.Service.Core.Configuration;
using INDG.Image.Service.Core.Models.Repository.S3;
using Microsoft.Extensions.Options;

namespace INDG.Image.Service.Core.Repositories.Implementation
{
    public class AwsS3Repository : IAwsS3Repository
    {
        private readonly IAmazonS3 amazonS3Client;
        private readonly IOptions<S3Configuration> s3Configuration;

        public AwsS3Repository(IAmazonS3 amazonS3Client, IOptions<S3Configuration> s3Configuration)
        {
            this.amazonS3Client = amazonS3Client;
            this.s3Configuration = s3Configuration;
        }

        public async Task<PutObjectRepositoryResponse> PutObjectAsync(PutObjectRepositoryRequest putObjectRepositoryRequest)
        {
            EnsureArg.IsNotNull(putObjectRepositoryRequest, nameof(putObjectRepositoryRequest));

            using (var requestStream = new MemoryStream(putObjectRepositoryRequest.Data)) 
            {
                var request = new PutObjectRequest
                {
                    BucketName = s3Configuration.Value.BucketName,
                    CannedACL = S3CannedACL.Private,
                    Key = putObjectRepositoryRequest.Key,
                    InputStream = requestStream
                };

                await amazonS3Client.PutObjectAsync(request);

                return new PutObjectRepositoryResponse { Key = putObjectRepositoryRequest.Key, Result = true};
            }
        }

        public async Task<GetObjectRepositoryResponse> GetObjectAsync(GetObjectRepositoryRequest getObjectRepositoryRequest)
        {
            EnsureArg.IsNotNull(getObjectRepositoryRequest, nameof(getObjectRepositoryRequest));

            try
            {
                using var response = await amazonS3Client.GetObjectAsync(new GetObjectRequest { BucketName = s3Configuration.Value.BucketName, Key = getObjectRepositoryRequest.Key });
                using var memoryStream = new MemoryStream();

                await response.ResponseStream.CopyToAsync(memoryStream);

                return new GetObjectRepositoryResponse { Data = memoryStream.ToArray() };
            }
            catch (AmazonS3Exception e) 
            { 
                if (e.StatusCode is System.Net.HttpStatusCode.NotFound) 
                {
                    return new GetObjectRepositoryResponse { Data = [0] };
                }

                throw;
            }
        }

        public async Task<DeleteObjectRepositoryResponse> DeleteObjectAsync(DeleteObjectRepositoryRequest deleteObjectRepositoryRequest)
        {
            EnsureArg.IsNotNull(deleteObjectRepositoryRequest, nameof(deleteObjectRepositoryRequest));

            try
            {
                await amazonS3Client.DeleteAsync(s3Configuration.Value.BucketName, deleteObjectRepositoryRequest.Key, null);

                return new DeleteObjectRepositoryResponse { Result = true };
            }
            catch (AmazonS3Exception e)
            {
                return new DeleteObjectRepositoryResponse { Result = false, ErrorCode = e.ErrorCode };
            }
        }
    }
}
