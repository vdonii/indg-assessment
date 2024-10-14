using AutoMapper;
using HybridModelBinding;
using INDG.Image.Service.ApiModels;
using INDG.Image.Service.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace INDG.Image.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> logger;
        private readonly IMapper mapper;
        private readonly IImageService imagesService;

        public ImagesController(ILogger<ImagesController> logger, IMapper mapper, IImageService imagesService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.imagesService = imagesService;
        }

        [HttpPut]
        public async Task<ActionResult> UploadImageAsync([FromForm] AddImageRequest addImageApiRequest)
        {
            var serviceRequest = this.mapper.Map<Core.Models.AddImageRequest>(addImageApiRequest);

            var response = await this.imagesService.UploadImageAsync(serviceRequest);

            var apiResponse = this.mapper.Map<AddImageResponse>(response);

            return new ObjectResult(apiResponse)
            {
                StatusCode = (int?)(apiResponse.Result ? HttpStatusCode.Created : HttpStatusCode.InternalServerError)
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetImageAsync([FromRoute] string id)
        {
            var response = await this.imagesService.GetImageAsync(id);

            var apiResponse = this.mapper.Map<GetImageResponse>(response);

            return File(apiResponse.Data, "image/gif");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteImageAsync([FromRoute] string id)
        {
            var response = await this.imagesService.DeleteImageAsync(id);

            var apiResponse = this.mapper.Map<DeleteImageResponse>(response);

            return new ObjectResult(apiResponse)
            {
                StatusCode = (int?)(apiResponse.Result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError)
            };
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> UpdateImageAsync([FromRoute] string id, [FromForm] UpdateImageRequest updateImageApiRequest)
        {
            var serviceRequest = this.mapper.Map<Core.Models.UpdateImageRequest>(updateImageApiRequest);
            serviceRequest.Id = id;

            var response = await this.imagesService.UpdateImageAsync(serviceRequest);

            var apiResponse = this.mapper.Map<UpdateImageResponse>(response);

            return new ObjectResult(apiResponse)
            {
                StatusCode = (int?)(apiResponse.Result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError)
            };
        }

        [HttpPost("{id}/resize")]
        public async Task<ActionResult> ResizeImageAsync([FromHybrid] ResizeImageRequest resizeImageApiRequest)
        {
            var serviceRequest = this.mapper.Map<Core.Models.ResizeImageRequest>(resizeImageApiRequest);

            var response = await this.imagesService.ResizeImageAsync(serviceRequest);

            var apiResponse = this.mapper.Map<ResizeImageResponse>(response);

            return new ObjectResult(apiResponse)
            {
                StatusCode = (int?)(apiResponse.Result ? HttpStatusCode.OK : HttpStatusCode.InternalServerError)
            };
        }
    }
}
