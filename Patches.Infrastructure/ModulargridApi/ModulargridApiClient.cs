using System.Net.Http.Json;
using System.Text.Json;
using Patches.Application.Contracts;

namespace Patches.Infrastructure.ModulargridApi;

public class ModulargridApiClient(HttpClient httpClient) : IModulargridApiClient
{
    public async Task<IReadOnlyList<ModulargridModuleDto>> GetModulesAsync(string endpointUrl)
    {
        var apiResponse = await httpClient.GetFromJsonAsync<ModulargridApiResponse>(endpointUrl);
        return MapToDto(apiResponse);
    }

    public IReadOnlyList<ModulargridModuleDto> ParseModulesFromJson(string json)
    {
        var apiResponse = JsonSerializer.Deserialize<ModulargridApiResponse>(json);
        return MapToDto(apiResponse);
    }

    private static IReadOnlyList<ModulargridModuleDto> MapToDto(ModulargridApiResponse? apiResponse)
    {
        if (apiResponse?.Response is not { Success: true } wrapper)
            throw new InvalidOperationException("Modulargrid JSON does not contain a successful response.");

        if (wrapper.Result?.Module is not { } modules)
            throw new InvalidOperationException("Modulargrid JSON contained no Module array.");

        return modules
            .Select(m => new ModulargridModuleDto(
                m.Name ?? string.Empty,
                m.Description ?? string.Empty,
                int.TryParse(m.Te, out var hp) ? hp : 0,
                m.Vendor?.Name))
            .ToList();
    }
}
