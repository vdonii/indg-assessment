using AutoFixture.Xunit2;
using FluentAssertions;
using INDG.Image.Service.Core.Mapping.Custom.Implementation;

namespace INDG.Image.Service.Core.UnitTests.Mappers
{
    public class ImageServiceResponsesMapperTests
    {
        private ImageServiceResponsesMapper mapper;

        public ImageServiceResponsesMapperTests() 
        {
            mapper = new ImageServiceResponsesMapper();
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_GetImageResponse_Then_FieldsAreMapped(byte[] data)
        {
            //Arrange
            //Act
            var result = mapper.MapGetImageResponse(data);

            //Assert
            result.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(data);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_AddImageResponse_Then_FieldsAreMapped(string id, bool result)
        {
            //Arrange
            //Act
            var mappingResult = mapper.MapAddImageResponse(id, result);

            //Assert
            mappingResult.Should().NotBeNull();
            mappingResult.Id.Should().Be(id);
            mappingResult.Result.Should().Be(result);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_UpdateImageResponse_Then_FieldsAreMapped(string id, bool result)
        {
            //Arrange
            //Act
            var mappingResult = mapper.MapUpdateImageResponse(id, result);

            //Assert
            mappingResult.Should().NotBeNull();
            mappingResult.Id.Should().Be(id);
            mappingResult.Result.Should().Be(result);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_DeleteImageResponse_Then_FieldsAreMapped(string errorCode, bool result)
        {
            //Arrange
            //Act
            var mappingResult = mapper.MapDeleteImageResponse(result, errorCode);

            //Assert
            mappingResult.Should().NotBeNull();
            mappingResult.ErrorCode.Should().Be(errorCode);
            mappingResult.Result.Should().Be(result);
        }

        [Theory]
        [AutoData]
        public void Given_MappingFrom_Data_When_MappingTo_ResizeImageResponse_Then_FieldsAreMapped(string id, bool result)
        {
            //Arrange
            //Act
            var mappingResult = mapper.MapResizeImageResponse(id, result);

            //Assert
            mappingResult.Should().NotBeNull();
            mappingResult.Id.Should().Be(id);
            mappingResult.Result.Should().Be(result);
        }
    }
}
