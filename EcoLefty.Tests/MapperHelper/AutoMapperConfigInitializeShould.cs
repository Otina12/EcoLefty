using AutoMapper;

namespace EcoLefty.Tests.MapperHelper;

public class AutoMapperConfigInitializeShould
{
    [Fact]
    public void CreateValidMappingConfiguration()
    {
        IMapper mapper = AutoMapperConfig.Initialize();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}