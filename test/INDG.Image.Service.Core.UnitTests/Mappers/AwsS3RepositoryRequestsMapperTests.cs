using AutoFixture.Xunit2;
using FluentAssertions;
using INDG.Image.Service.Core.Mapping.Custom.Implementation;

namespace INDG.Image.Service.Core.UnitTests.Mappers
{
    public class AwsS3RepositoryRequestsMapperTests
    {
        private readonly AwsS3RepositoryRequestsMapper mapper;

        public AwsS3RepositoryRequestsMapperTests()
        {
            mapper = new AwsS3RepositoryRequestsMapper();
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_PutObjectRepositoryRequest_Then_FieldsAreMapped(string key, byte[] data)
        {
            //Arrange
            //Act
            var result = mapper.MapPutObjectRepositoryRequest(key, data);

            //Assert
            result.Should().NotBeNull();
            result.Key.Should().Be(key);
            result.Data.Should().BeEquivalentTo(data);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_GetObjectRepositoryRequest_Then_FieldsAreMapped(string key)
        {
            //Arrange
            //Act
            var result = mapper.MapGetObjectRepositoryRequest(key);

            //Assert
            result.Should().NotBeNull();
            result.Key.Should().Be(key);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_DeleteObjectRepositoryRequest_Then_FieldsAreMapped(string key)
        {
            //Arrange
            //Act
            var result = mapper.MapDeleteObjectRepositoryRequest(key);

            //Assert
            result.Should().NotBeNull();
            result.Key.Should().Be(key);
        }
    }
}