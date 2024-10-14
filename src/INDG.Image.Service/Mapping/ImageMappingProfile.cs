using AutoMapper;
namespace INDG.Image.Service.Mapping
{
    public class ImageMappingProfile : Profile
    {
        public ImageMappingProfile() 
        {
            CreateMap<ApiModels.AddImageRequest, Core.Models.AddImageRequest>();
            CreateMap<Core.Models.AddImageResponse, ApiModels.AddImageResponse>();

            CreateMap<Core.Models.GetImageResponse, ApiModels.GetImageResponse>();

            CreateMap<Core.Models.DeleteImageResponse, ApiModels.DeleteImageResponse>();

            CreateMap<ApiModels.UpdateImageRequest, Core.Models.UpdateImageRequest>()
                .ForMember(m => m.Id, opt => opt.Ignore());
            CreateMap<Core.Models.UpdateImageResponse, ApiModels.UpdateImageResponse>();

            CreateMap<ApiModels.ResizeImageRequest, Core.Models.ResizeImageRequest>();
            CreateMap<Core.Models.ResizeImageResponse, ApiModels.ResizeImageResponse>();
        }
    }
}
