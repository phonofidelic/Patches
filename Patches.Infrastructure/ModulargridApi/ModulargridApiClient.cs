using System.Net.Http.Json;
using Patches.Application.Contracts;

namespace Patches.Infrastructure.ModulargridApi;

public class ModulargridApiClient(HttpClient httpClient) : IModulargridApiClient
{
    public async Task<IReadOnlyList<ModulargridModuleDto>> GetModulesAsync(string endpointUrl)
    {
        var apiResponse = await httpClient.GetFromJsonAsync<ModulargridApiResponse>(endpointUrl);

        if (apiResponse?.Response is not { Success: true } wrapper)
            throw new InvalidOperationException("Modulargrid API returned an unsuccessful response.");

        if (wrapper.Result?.Module is not { } modules)
            throw new InvalidOperationException("Modulargrid API response contained no Module array.");

        return modules
            .Select(m => new ModulargridModuleDto(
                m.Name ?? string.Empty,
                m.Description ?? string.Empty,
                int.TryParse(m.Te, out var hp) ? hp : 0,
                m.Vendor?.Name))
            .ToList();
    }
}
