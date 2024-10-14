using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture;
using FluentAssertions;
using INDG.Image.Service.Core.Configuration;
using INDG.Image.Service.Core.Models.Repository.S3;
using INDG.Image.Service.Core.Repositories;
using INDG.Image.Service.Core.Repositories.Implementation;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;

namespace INDG.Image.Service.Core.UnitTests.Repositories
{
    public class AwsS3RepositoryTests
    {
        private readonly IAwsS3Repository repository;
        private readonly Mock<IAmazonS3> mockAmazonS3;
        private readonly Mock<IOptions<S3Configuration>> mockS3Configuration;
        private readonly IFixture fixture;

        public AwsS3RepositoryTests() 
        {
            fixture = new Fixture();
            var mocker = new AutoMocker();
            mockAmazonS3 = mocker.GetMock<IAmazonS3>();
            mockS3Configuration = mocker.GetMock<IOptions<S3Configuration>>();
            repository = mocker.CreateInstance<AwsS3Repository>();
        }

        [Fact]
        public async Task Given_AwsS3Repository_When_GetObjectAsync_Then_ReturnData()
        {
            //Arrange
            var getObjectRepositoryRequest = this.fixture.Create<GetObjectRepositoryRequest>();
            var responseData = new MemoryStream(new byte[1]);
            var getObjectResponse = this.fixture.Build<GetObjectResponse>()
                .With(x => x.ResponseStream, responseData)
                .Create();
            var configuration = this.fixture.Create<S3Configuration>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);
            this.mockAmazonS3.Setup(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>())).ReturnsAsync(getObjectResponse);

            //Act
            var result = await this.repository.GetObjectAsync(getObjectRepositoryRequest);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeEmpty();

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Given_AwsS3Repository_When_GetObjectAsync_And_AmazonS3NotFoundException_Then_EmptyData()
        {
            //Arrange
            var getObjectRepositoryRequest = this.fixture.Create<GetObjectRepositoryRequest>();
            var configuration = this.fixture.Create<S3Configuration>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);
            this.mockAmazonS3.Setup(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>())).ThrowsAsync(new AmazonS3Exception(new Exception()) { StatusCode = System.Net.HttpStatusCode.NotFound});

            //Act
            var result = await this.repository.GetObjectAsync(getObjectRepositoryRequest);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo([0]);

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Given_AwsS3Repository_When_GetObjectAsync_And_AmazonS3Exception_Then_ExceptionIsThrown()
        {
            //Arrange
            var getObjectRepositoryRequest = this.fixture.Create<GetObjectRepositoryRequest>();
            var configuration = this.fixture.Create<S3Configuration>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);
            this.mockAmazonS3.Setup(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>())).ThrowsAsync(new AmazonS3Exception(new Exception()) { StatusCode = System.Net.HttpStatusCode.NotFound });

            //Act
            var func = async () => await this.repository.GetObjectAsync(getObjectRepositoryRequest);

            //Assert
            func.Should().ThrowAsync<AmazonS3Exception>();

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.GetObjectAsync(It.Is<GetObjectRequest>(m => m.Key.Equals(getObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Given_AwsS3Repository_When_GetObjectAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await this.repository.GetObjectAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockS3Configuration.Verify(x => x.Value, Times.Never);
            this.mockAmazonS3.Verify(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Given_AwsS3Repository_When_PutObjectAsync_Then_ObjectPut()
        {
            //Arrange
            var putObjectRepositoryRequest = this.fixture.Create<PutObjectRepositoryRequest>();
            var configuration = this.fixture.Create<S3Configuration>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);

            //Act
            var result = await this.repository.PutObjectAsync(putObjectRepositoryRequest);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeTrue();

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(m => m.Key.Equals(putObjectRepositoryRequest.Key)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Given_AwsS3Repository_When_PutObjectAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await this.repository.PutObjectAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockS3Configuration.Verify(x => x.Value, Times.Never);
            this.mockAmazonS3.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Given_AwsS3Repository_When_DeleteObjectAsync_Then_ObjectDeleted()
        {
            //Arrange
            var deleteObjectRepositoryRequest = this.fixture.Create<DeleteObjectRepositoryRequest>();
            var configuration = this.fixture.Create<S3Configuration>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);

            //Act
            var result = await this.repository.DeleteObjectAsync(deleteObjectRepositoryRequest);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeTrue();
            result.ErrorCode.Should().BeNullOrEmpty();

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.DeleteAsync(configuration.BucketName, deleteObjectRepositoryRequest.Key, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Given_AwsS3Repository_When_DeleteObjectAsync_And_AmazonS3Exception_Then_FalseResult()
        {
            //Arrange
            var deleteObjectRepositoryRequest = this.fixture.Create<DeleteObjectRepositoryRequest>();
            var configuration = this.fixture.Create<S3Configuration>();
            var amazonS3Exception = this.fixture.Create<AmazonS3Exception>();

            this.mockS3Configuration.Setup(x => x.Value).Returns(configuration);
            this.mockAmazonS3.Setup(x => x.DeleteAsync(configuration.BucketName, deleteObjectRepositoryRequest.Key, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>())).ThrowsAsync(amazonS3Exception);

            //Act
            var result = await this.repository.DeleteObjectAsync(deleteObjectRepositoryRequest);

            //Assert
            result.Should().NotBeNull();
            result.Result.Should().BeFalse();
            result.ErrorCode.Should().Be(amazonS3Exception.ErrorCode);

            this.mockS3Configuration.Verify(x => x.Value, Times.Once);
            this.mockAmazonS3.Verify(x => x.DeleteAsync(configuration.BucketName, deleteObjectRepositoryRequest.Key, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Given_AwsS3Repository_When_DeleteObjectAsync_And_RequestIsNull_Then_ArgumentExceptionIsThrown()
        {
            //Arrange
            //Act
            var func = async () => await this.repository.DeleteObjectAsync(null);

            //Assert
            func.Should().ThrowAsync<ArgumentNullException>();

            this.mockS3Configuration.Verify(x => x.Value, Times.Never);
            this.mockAmazonS3.VerifyNoOtherCalls();
        }


    }
}
