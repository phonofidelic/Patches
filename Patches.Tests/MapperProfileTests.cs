using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Patches.Infrastructure.Data;
using Xunit;

namespace Patches.Tests;

public class MapperProfileTests
{
    [Fact]
    public void MapperProfile_ConfigurationIsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>(), NullLoggerFactory.Instance);
        config.AssertConfigurationIsValid();
    }
}
