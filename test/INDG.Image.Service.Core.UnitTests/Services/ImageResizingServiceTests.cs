using AutoFixture;
using FluentAssertions;
using INDG.Image.Service.Core.Helpers;
using INDG.Image.Service.Core.Services;
using INDG.Image.Service.Core.Services.Implementation;
using Moq;
using Moq.AutoMock;

namespace INDG.Image.Service.Core.UnitTests.Services
{
    public class ImageResizingServiceTests
    {
        private readonly IImageResizingService service;
        private readonly Mock<IImageHelper> mockImageHelper;
        private readonly IFixture fixture;

        public ImageResizingServiceTests() 
        {
            fixture = new Fixture();
            var mocker = new AutoMocker();
            mockImageHelper = mocker.GetMock<IImageHelper>();
            service = mocker.CreateInstance<ImageResizingService>();
        }

        [Fact]
        public void Given_ImageResizingService_When_ResizeImage_Then_ImageResized()
        {
            //Arrange
            var originalImageBytes = this.fixture.Create<byte[]>();
            var height = this.fixture.Create<int>();
            var resultBytes = this.fixture.Create<byte[]>();

            this.mockImageHelper.Setup(x => x.GetImageBytes(It.IsAny<System.Drawing.Image>())).Returns(resultBytes);

            //Act
            var result = service.ResizeImage(originalImageBytes, height);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(resultBytes);

            this.mockImageHelper.Verify(x => x.GetImageBytes(It.IsAny<System.Drawing.Image>()), Times.Once);
            this.mockImageHelper.Verify(x => x.GetImage(originalImageBytes), Times.Once);
            this.mockImageHelper.Verify(x => x.GetRatio(It.IsAny<System.Drawing.Image>()), Times.Once);
            this.mockImageHelper.Verify(x => x.ResizeImage(It.IsAny<System.Drawing.Image>(), height, It.IsAny<float>()), Times.Once);
            this.mockImageHelper.VerifyNoOtherCalls();
            
        }
    }
}
