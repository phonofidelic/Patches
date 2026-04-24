using Patches.Infrastructure.ModulargridApi;
using Xunit;

namespace Patches.Tests;

public class ParseModulesFromJsonTests
{
    private static ModulargridApiClient CreateClient() =>
        new ModulargridApiClient(new HttpClient());

    [Fact]
    public void ValidJson_ReturnsMappedDtos()
    {
        var json = """
            {
                "response": {
                    "success": true,
                    "result": {
                        "Module": [
                            {
                                "name": "Plaits",
                                "description": "Macro oscillator",
                                "te": "16",
                                "Vendor": { "name": "Mutable Instruments" }
                            }
                        ]
                    }
                }
            }
            """;

        var client = CreateClient();
        var dtos = client.ParseModulesFromJson(json);

        Assert.Single(dtos);
        Assert.Equal("Plaits", dtos[0].Name);
        Assert.Equal("Macro oscillator", dtos[0].Description);
        Assert.Equal(16, dtos[0].HorizontalPitch);
        Assert.Equal("Mutable Instruments", dtos[0].VendorName);
    }

    [Fact]
    public void ValidJson_MultipleModules_ReturnsAll()
    {
        var json = """
            {
                "response": {
                    "success": true,
                    "result": {
                        "Module": [
                            { "name": "Rings", "te": "8", "Vendor": { "name": "Mutable Instruments" } },
                            { "name": "Clouds", "te": "18", "Vendor": { "name": "Mutable Instruments" } }
                        ]
                    }
                }
            }
            """;

        var client = CreateClient();
        var dtos = client.ParseModulesFromJson(json);

        Assert.Equal(2, dtos.Count);
    }

    [Fact]
    public void ValidJson_NoVendor_VendorNameIsNull()
    {
        var json = """
            {
                "response": {
                    "success": true,
                    "result": {
                        "Module": [
                            { "name": "SomeDIY", "te": "4" }
                        ]
                    }
                }
            }
            """;

        var client = CreateClient();
        var dtos = client.ParseModulesFromJson(json);

        Assert.Single(dtos);
        Assert.Null(dtos[0].VendorName);
    }

    [Fact]
    public void Json_SuccessFalse_ThrowsInvalidOperationException()
    {
        var json = """
            {
                "response": {
                    "success": false
                }
            }
            """;

        var client = CreateClient();
        Assert.Throws<InvalidOperationException>(() => client.ParseModulesFromJson(json));
    }

    [Fact]
    public void Json_NoModuleArray_ThrowsInvalidOperationException()
    {
        var json = """
            {
                "response": {
                    "success": true,
                    "result": {}
                }
            }
            """;

        var client = CreateClient();
        Assert.Throws<InvalidOperationException>(() => client.ParseModulesFromJson(json));
    }

    [Fact]
    public void MalformedJson_ThrowsException()
    {
        var client = CreateClient();
        Assert.ThrowsAny<Exception>(() => client.ParseModulesFromJson("not valid json {{{"));
    }

    [Fact]
    public void NonNumericTe_DefaultsToZeroHp()
    {
        var json = """
            {
                "response": {
                    "success": true,
                    "result": {
                        "Module": [
                            { "name": "Foo", "te": "n/a" }
                        ]
                    }
                }
            }
            """;

        var client = CreateClient();
        var dtos = client.ParseModulesFromJson(json);

        Assert.Single(dtos);
        Assert.Equal(0, dtos[0].HorizontalPitch);
    }
}
