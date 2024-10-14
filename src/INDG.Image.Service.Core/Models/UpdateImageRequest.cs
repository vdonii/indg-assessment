using Microsoft.AspNetCore.Http;

namespace INDG.Image.Service.Core.Models
{
    public class UpdateImageRequest
    {
        public string Id { get; set; }
        public IFormFile File { get; set; }
    }
}
