using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Shared.Commands;

namespace Patches.Application.Handlers;

public class ImportModulesFromJsonHandler(
    IModulargridApiClient apiClient,
    IUnitOfWork unitOfWork) : IHandler<ImportModulesFromJsonCommand, ImportModulesFromJsonResult>
{
    public async Task<ImportModulesFromJsonResult> HandleAsync(
        ImportModulesFromJsonCommand command,
        CancellationToken ct = default)
    {
        var dtos = apiClient.ParseModulesFromJson(command.Json);

        var existingModules = unitOfWork.Modules.GetAll()
            .Select(m => (m.Name.ToLowerInvariant(), m.Vendor?.Name?.ToLowerInvariant()))
            .ToHashSet();

        var vendorsByName = unitOfWork.Vendors.GetAll()
            .ToDictionary(v => v.Name, v => v, StringComparer.OrdinalIgnoreCase);

        int imported = 0, skipped = 0;

        foreach (var dto in dtos)
        {
            if (existingModules.Contains((dto.Name.ToLowerInvariant(), dto.VendorName?.ToLowerInvariant())))
            {
                skipped++;
                continue;
            }

            Vendor? vendor = null;
            if (dto.VendorName is not null)
            {
                if (!vendorsByName.TryGetValue(dto.VendorName, out vendor))
                {
                    vendor = new Vendor { Name = dto.VendorName };
                    unitOfWork.Vendors.Add(vendor);
                    vendorsByName[dto.VendorName] = vendor;
                }
            }

            unitOfWork.Modules.Add(new Module
            {
                Name = dto.Name,
                Description = dto.Description,
                HorizontalPitch = dto.HorizontalPitch,
                VerticalUnits = 3,
                Vendor = vendor
            });

            imported++;
        }

        await unitOfWork.SaveChangesAsync();

        return new ImportModulesFromJsonResult
        {
            ImportedCount = imported,
            SkippedCount = skipped
        };
    }
}
