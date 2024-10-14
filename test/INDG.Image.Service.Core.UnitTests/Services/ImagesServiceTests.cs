using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using INDG.Image.Service.Core.Helpers;
using INDG.Image.Service.Core.Mapping.Custom;
using INDG.Image.Service.Core.Models;
using INDG.Image.Service.Core.Models.Repository.S3;
using INDG.Image.Service.Core.Repositories;
using INDG.Image.Service.Core.Services;
using INDG.Image.Service.Core.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.AutoMock;
using System.Text;

namespace INDG.Image.Service.Core.UnitTests.Services
{
    public class ImagesServiceTests
    {
        private readonly IImageService service;
        private readonly Mock<IImageResizingService> mockImageResizingService;
        private readonly Mock<IAwsS3Repository> mockAwsS3Repository;
        private readonly Mock<IImageHelper> mockImageHelper;
        private readonly Mock<IAwsS3RepositoryRequestsMapper> mockAwsS3RepositoryRequestsMapper;
        private readonly Mock<IImageServiceResponsesMapper> mockImageServiceResponsesMapper;
        private readonly IFixture fixture;

        public ImagesServiceTests() 
        {
            fixture = new Fixture();
            var mocker = new AutoMocker();
            mockImageResizingService = mocker.GetMock<IImageResizingService>();
            mockAwsS3Repository = mocker.GetMock<IAwsS3Repository>();
            mockImageHelper = mocker.GetMock<IImageHelper>();
            mockAwsS3RepositoryRequestsMapper = mocker.GetMock<IAwsS3RepositoryRequestsMapper>();
            mockImageServiceResponsesMapper = mocker.GetMock<IImageServiceResponsesMapper>();
            service = mocker.CreateInstance<ImageService>();
        }

        [Fact]
        public async Task Given_ImageService_When_UploadImageAsync_Then_ImageUploaded()
        {
            //Arrange
            using var testStream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));
            var file = new FormFile(testStream, 0, 1, "file", "file");
            var addImageRequest = this.fixture.Build<AddImageRequest>()
                .With(x => x.File, file)
                .Create();
            var imageBytes = this.fixture.Create<byte[]>();
            var putObjectRepositoryRequest = this.fixture.Create<PutObjectRepositoryRequest>();
            var putObjectRepositoryResponse = this.fixture.Create<PutObjectRepositoryResponse>();
            var addImageResponse = this.fixture.Create<AddImageResponse>();

            this.mockImageHelper.Setup(x => x.GetImageBytes(addImageRequest.File)).Returns(imageBytes);
            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapPutObjectRepositoryRequest(It.IsAny<string>(), imageBytes)).Returns(putObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.PutObjectAsync(putObjectRepositoryRequest)).ReturnsAsync(putObjectRepositoryResponse);
            this.mockImageServiceResponsesMapper.Setup(x => x.MapAddImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result)).Returns(addImageResponse);

            //Act
            var result = await service.UploadImageAsync(addImageRequest);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(addImageResponse);

            this.mockImageHelper.Verify(x => x.GetImageBytes(addImageRequest.File), Times.Once);
            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapPutObjectRepositoryRequest(It.IsAny<string>(), imageBytes), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.PutObjectAsync(putObjectRepositoryRequest), Times.Once);
            this.mockImageServiceResponsesMapper.Verify(x => x.MapAddImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result), Times.Once);

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void Given_ImageService_When_UploadImageAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await service.UploadImageAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Theory, AutoData]
        public async Task Given_ImageService_When_GetImageAsync_Then_ImageRetrieved(string id)
        {
            //Arrange
            var getObjectRepositoryRequest = this.fixture.Create<GetObjectRepositoryRequest>();
            var getObjectRepositoryResponse = this.fixture.Create<GetObjectRepositoryResponse>();
            var getImageResponse = this.fixture.Create<GetImageResponse>();

            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapGetObjectRepositoryRequest(id)).Returns(getObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.GetObjectAsync(getObjectRepositoryRequest)).ReturnsAsync(getObjectRepositoryResponse);
            this.mockImageServiceResponsesMapper.Setup(x => x.MapGetImageResponse(getObjectRepositoryResponse.Data)).Returns(getImageResponse);

            //Act
            var result = await service.GetImageAsync(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(getImageResponse);

            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapGetObjectRepositoryRequest(id), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.GetObjectAsync(getObjectRepositoryRequest), Times.Once);
            this.mockImageServiceResponsesMapper.Verify(x => x.MapGetImageResponse(getObjectRepositoryResponse.Data), Times.Once);

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Given_ImageService_When_GetImageAsync_And_IdIsMissing_Then_ArgumentExceptionIsThrown(string id)
        {
            //Arrange
            //Act
            var func = async () => await service.GetImageAsync(id);

            //Assert
            func.Should().ThrowAsync<ArgumentException>();

            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Theory, AutoData]
        public async Task Given_ImageService_When_DeleteImageAsync_Then_ImageDeleted(string id)
        {
            //Arrange
            var deleteObjectRepositoryRequest = this.fixture.Create<DeleteObjectRepositoryRequest>();
            var deleteObjectRepositoryResponse = this.fixture.Create<DeleteObjectRepositoryResponse>();
            var deleteImageResponse = this.fixture.Create<DeleteImageResponse>();

            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapDeleteObjectRepositoryRequest(id)).Returns(deleteObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.DeleteObjectAsync(deleteObjectRepositoryRequest)).ReturnsAsync(deleteObjectRepositoryResponse);
            this.mockImageServiceResponsesMapper.Setup(x => x.MapDeleteImageResponse(deleteObjectRepositoryResponse.Result, deleteObjectRepositoryResponse.ErrorCode)).Returns(deleteImageResponse);

            //Act
            var result = await service.DeleteImageAsync(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(deleteImageResponse);

            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapDeleteObjectRepositoryRequest(id), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.DeleteObjectAsync(deleteObjectRepositoryRequest), Times.Once);
            this.mockImageServiceResponsesMapper.Verify(x => x.MapDeleteImageResponse(deleteObjectRepositoryResponse.Result, deleteObjectRepositoryResponse.ErrorCode), Times.Once);

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Given_ImageService_When_DeleteImageAsync_And_IdIsMissing_Then_ArgumentExceptionIsThrown(string id)
        {
            //Arrange
            //Act
            var func = async () => await service.DeleteImageAsync(id);

            //Assert
            func.Should().ThrowAsync<ArgumentException>();

            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_ImageService_When_UpdateImageAsync_Then_ImageUpdated()
        {
            //Arrange
            using var testStream = new MemoryStream(Encoding.UTF8.GetBytes("stream"));
            var file = new FormFile(testStream, 0, 1, "file", "file");
            var updateImageRequest = this.fixture.Build<UpdateImageRequest>()
                .With(x => x.File, file)
                .Create();
            var imageBytes = this.fixture.Create<byte[]>();
            var putObjectRepositoryRequest = this.fixture.Create<PutObjectRepositoryRequest>();
            var putObjectRepositoryResponse = this.fixture.Create<PutObjectRepositoryResponse>();
            var updateImageResponse = this.fixture.Create<UpdateImageResponse>();

            this.mockImageHelper.Setup(x => x.GetImageBytes(updateImageRequest.File)).Returns(imageBytes);
            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapPutObjectRepositoryRequest(updateImageRequest.Id, imageBytes)).Returns(putObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.PutObjectAsync(putObjectRepositoryRequest)).ReturnsAsync(putObjectRepositoryResponse);
            this.mockImageServiceResponsesMapper.Setup(x => x.MapUpdateImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result)).Returns(updateImageResponse);

            //Act
            var result = await service.UpdateImageAsync(updateImageRequest);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(updateImageResponse);

            this.mockImageHelper.Verify(x => x.GetImageBytes(updateImageRequest.File), Times.Once);
            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapPutObjectRepositoryRequest(updateImageRequest.Id, imageBytes), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.PutObjectAsync(putObjectRepositoryRequest), Times.Once);
            this.mockImageServiceResponsesMapper.Verify(x => x.MapUpdateImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result), Times.Once);

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void Given_ImageService_When_UpdateImageAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await service.UpdateImageAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Given_ImageService_When_ResizeImageAsync_Then_ImageResized()
        {
            //Arrange
            var resizeImageRequest = this.fixture.Create<ResizeImageRequest>();
            var imageBytes = this.fixture.Create<byte[]>();
            var putObjectRepositoryRequest = this.fixture.Create<PutObjectRepositoryRequest>();
            var putObjectRepositoryResponse = this.fixture.Create<PutObjectRepositoryResponse>();
            var resizeImageResponse = this.fixture.Create<ResizeImageResponse>();

            var getObjectRepositoryRequest = this.fixture.Create<GetObjectRepositoryRequest>();
            var getObjectRepositoryResponse = this.fixture.Create<GetObjectRepositoryResponse>();

            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapGetObjectRepositoryRequest(resizeImageRequest.Id)).Returns(getObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.GetObjectAsync(getObjectRepositoryRequest)).ReturnsAsync(getObjectRepositoryResponse);

            this.mockImageResizingService.Setup(x => x.ResizeImage(getObjectRepositoryResponse.Data, resizeImageRequest.Height)).Returns(imageBytes);
            this.mockAwsS3RepositoryRequestsMapper.Setup(x => x.MapPutObjectRepositoryRequest(It.IsAny<string>(), imageBytes)).Returns(putObjectRepositoryRequest);
            this.mockAwsS3Repository.Setup(x => x.PutObjectAsync(putObjectRepositoryRequest)).ReturnsAsync(putObjectRepositoryResponse);
            this.mockImageServiceResponsesMapper.Setup(x => x.MapResizeImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result)).Returns(resizeImageResponse);

            //Act
            var result = await service.ResizeImageAsync(resizeImageRequest);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resizeImageResponse);

            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapGetObjectRepositoryRequest(resizeImageRequest.Id), Times.Once);
            this.mockAwsS3RepositoryRequestsMapper.Verify(x => x.MapPutObjectRepositoryRequest(It.IsAny<string>(), imageBytes), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.PutObjectAsync(putObjectRepositoryRequest), Times.Once);
            this.mockAwsS3Repository.Verify(x => x.GetObjectAsync(getObjectRepositoryRequest), Times.Once);
            this.mockImageServiceResponsesMapper.Verify(x => x.MapResizeImageResponse(putObjectRepositoryResponse.Key, putObjectRepositoryResponse.Result), Times.Once);
            this.mockImageResizingService.Verify(x => x.ResizeImage(getObjectRepositoryResponse.Data, resizeImageRequest.Height), Times.Once);

            this.mockImageResizingService.VerifyNoOtherCalls();
            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void Given_ImageService_When_ResizeImageAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await service.ResizeImageAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockImageResizingService.VerifyNoOtherCalls();
            this.mockImageHelper.VerifyNoOtherCalls();
            this.mockAwsS3RepositoryRequestsMapper.VerifyNoOtherCalls();
            this.mockAwsS3Repository.VerifyNoOtherCalls();
            this.mockImageServiceResponsesMapper.VerifyNoOtherCalls();
        }
    }
}
