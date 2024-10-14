using Microsoft.AspNetCore.Http;

namespace INDG.Image.Service.Core.Models
{
    public class AddImageRequest
    {
        public IFormFile File { get; set; }
    }
}
