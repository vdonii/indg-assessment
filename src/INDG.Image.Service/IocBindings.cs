using Amazon.S3;
using Amazon.SecurityToken;
using Amazon.SimpleSystemsManagement;
using INDG.Image.Service.Core.Configuration;
using INDG.Image.Service.Core.Helpers;
using INDG.Image.Service.Core.Helpers.Implementation;
using INDG.Image.Service.Core.Mapping.Custom;
using INDG.Image.Service.Core.Mapping.Custom.Implementation;
using INDG.Image.Service.Core.Repositories;
using INDG.Image.Service.Core.Repositories.Implementation;
using INDG.Image.Service.Core.Services;
using INDG.Image.Service.Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace INDG.Image.Service
{
    public class IocBindings
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddScoped<IImageService, ImageService>();
            services.TryAddScoped<IImageResizingService, ImageResizingService>();

            services.TryAddScoped<IAwsS3Repository, AwsS3Repository>();

            services.TryAddScoped<IAwsS3RepositoryRequestsMapper, AwsS3RepositoryRequestsMapper>();
            services.TryAddScoped<IImageServiceResponsesMapper, ImageServiceResponsesMapper>();

            services.TryAddScoped<IImageHelper, ImageHelper>();

            services.AddAWSService<IAmazonS3>();
            services.AddAWSService<IAmazonSecurityTokenService>();
            services.AddAWSService<IAmazonSimpleSystemsManagement>();

            services.Configure<S3Configuration>(configuration.GetSection("s3"));
        }
    }
}
