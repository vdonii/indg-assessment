namespace INDG.Image.Service.Core.Models.Repository.S3
{
    public class PutObjectRepositoryRequest
    {
        public string Key { get; set; }
        public byte[] Data { get; set; }
    }
}
