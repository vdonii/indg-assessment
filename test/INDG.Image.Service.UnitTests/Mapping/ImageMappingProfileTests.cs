using AutoFixture;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using INDG.Image.Service.ApiModels;
using INDG.Image.Service.Mapping;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace INDG.Image.Service.UnitTests.Mapping
{
    public class ImageMappingProfileTests
    {
        private readonly MapperConfiguration mapperConfiguration;
        private readonly IMapper mapper;
        private readonly IFixture fixture;

        public ImageMappingProfileTests()
        {
            mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<ImageMappingProfile>());
            mapper = mapperConfiguration.CreateMapper();
            fixture = new Fixture();
        }

        [Fact]
        public void Given_MappingProfile_When_CheckingConfiguration_Then_ShouldBeValid()
        {
            //Arrange
            //Act
            //Assert
            this.mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void Given_MappingFrom_AddImageApiRequest_When_MappingTo_AddImageRequest_Then_Mapped()
        {
            //Arrange
            var addImageApiRequest = this.fixture.Build<ApiModels.AddImageRequest>()
                .With(x => x.File, GetFile())
                .Create();
            //Act
            var addImageRequest = mapper.Map<Core.Models.AddImageRequest>(addImageApiRequest);
            //Assert
            addImageRequest.Should().NotBeNull();
            addImageRequest.Should().BeEquivalentTo(addImageApiRequest);
        }

        [Fact]
        public void Given_MappingFrom_AddImageResponse_When_MappingTo_AddImageApiResponse_Then_Mapped()
        {
            //Arrange
            var addImageResponse = this.fixture.Create<Core.Models.AddImageResponse>();
            //Act
            var addImageApiResponse = mapper.Map<ApiModels.AddImageResponse>(addImageResponse);
            //Assert
            addImageApiResponse.Should().NotBeNull();
            addImageApiResponse.Should().BeEquivalentTo(addImageResponse);
        }

        [Fact]
        public void Given_MappingFrom_GetImageResponse_When_MappingTo_GetImageApiResponse_Then_Mapped()
        {
            //Arrange
            var getImageResponse = this.fixture.Create<Core.Models.GetImageResponse>();
            //Act
            var getImageApiResponse = mapper.Map<ApiModels.GetImageResponse>(getImageResponse);
            //Assert
            getImageApiResponse.Should().NotBeNull();
            getImageApiResponse.Should().BeEquivalentTo(getImageResponse);
        }

        [Fact]
        public void Given_MappingFrom_DeleteImageResponse_When_MappingTo_DeleteImageApiResponse_Then_Mapped()
        {
            //Arrange
            var deleteImageResponse = this.fixture.Create<Core.Models.DeleteImageResponse>();
            //Act
            var deleteImageApiResponse = mapper.Map<ApiModels.DeleteImageResponse>(deleteImageResponse);
            //Assert
            deleteImageApiResponse.Should().NotBeNull();
            deleteImageApiResponse.Should().BeEquivalentTo(deleteImageResponse);
        }

        [Fact]
        public void Given_MappingFrom_UpdateImageApiRequest_When_MappingTo_UpdateImageRequest_Then_Mapped()
        {
            //Arrange
            var updateImageApiRequest = this.fixture.Build<ApiModels.UpdateImageRequest>()
                .With(x => x.File, GetFile())
                .Create();
            //Act
            var updateImageRequest = mapper.Map<Core.Models.UpdateImageRequest>(updateImageApiRequest);
            //Assert
            updateImageRequest.Should().NotBeNull();
            updateImageRequest.File.Should().BeEquivalentTo(updateImageApiRequest.File);
        }

        [Fact]
        public void Given_MappingFrom_UpdateImageResponse_When_MappingTo_UpdateImageApiResponse_Then_Mapped()
        {
            //Arrange
            var updateImageResponse = this.fixture.Create<Core.Models.UpdateImageResponse>();
            //Act
            var updateImageApiResponse = mapper.Map<ApiModels.UpdateImageResponse>(updateImageResponse);
            //Assert
            updateImageApiResponse.Should().NotBeNull();
            updateImageApiResponse.Should().BeEquivalentTo(updateImageResponse);
        }

        [Fact]
        public void Given_MappingFrom_ResizeImageApiRequest_When_MappingTo_ResizeImageRequest_Then_Mapped()
        {
            //Arrange
            var resizeImageApiRequest = this.fixture.Create<ApiModels.ResizeImageRequest>();
            //Act
            var resizeImageRequest = mapper.Map<Core.Models.ResizeImageRequest>(resizeImageApiRequest);
            //Assert
            resizeImageRequest.Should().NotBeNull();
            resizeImageRequest.Should().BeEquivalentTo(resizeImageApiRequest);
        }

        [Fact]
        public void Given_MappingFrom_ResizeImageResponse_When_MappingTo_ResizeImageApiResponse_Then_Mapped()
        {
            //Arrange
            var resizeImageResponse = this.fixture.Create<Core.Models.ResizeImageResponse>();
            //Act
            var resizeImageApiResponse = mapper.Map<ApiModels.ResizeImageResponse>(resizeImageResponse);
            //Assert
            resizeImageApiResponse.Should().NotBeNull();
            resizeImageApiResponse.Should().BeEquivalentTo(resizeImageResponse);
        }

        private IFormFile GetFile()
        {
            using var testStream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));
            var file = new FormFile(testStream, 0, 1, "file", "file");

            return file;
        }
    }
}