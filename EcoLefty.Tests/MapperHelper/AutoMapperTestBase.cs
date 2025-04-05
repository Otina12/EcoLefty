using AutoMapper;

namespace EcoLefty.Tests.MapperHelper;

public abstract class AutoMapperTestBase<TSource, TDestination> where TSource : new()
{
    protected readonly IMapper _mapper = AutoMapperConfig.Initialize();

    [Fact]
    public void MapWithoutThrowing()
    {
        _mapper.Map<TDestination>(new TSource());
    }
}