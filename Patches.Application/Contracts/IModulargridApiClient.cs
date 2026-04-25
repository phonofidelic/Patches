namespace Patches.Application.Contracts;

public record ModulargridModuleDto(
    string Name,
    string Description,
    int HorizontalPitch,
    string? VendorName);

public interface IModulargridApiClient
{
    IReadOnlyList<ModulargridModuleDto> ParseModulesFromJson(string json);
}
