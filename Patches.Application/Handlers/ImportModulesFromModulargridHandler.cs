using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Shared.Commands;

namespace Patches.Application.Handlers;

public class ImportModulesFromModulargridHandler(
    IModulargridApiClient apiClient,
    IUnitOfWork unitOfWork) : IHandler<ImportModulesFromModulargridCommand, ImportModulesFromModulargridResult>
{
    public async Task<ImportModulesFromModulargridResult> HandleAsync(ImportModulesFromModulargridCommand command)
    {
        var dtos = await apiClient.GetModulesAsync(command.EndpointUrl);

        var existingModules = unitOfWork.Modules.GetAll()
            .Select(m => (m.Name, m.Vendor?.Name))
            .ToHashSet();

        var vendorsByName = unitOfWork.Vendors.GetAll()
            .ToDictionary(v => v.Name, v => v);

        int imported = 0, skipped = 0;

        foreach (var dto in dtos)
        {
            if (existingModules.Contains((dto.Name, dto.VendorName)))
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

        return new ImportModulesFromModulargridResult
        {
            ImportedCount = imported,
            SkippedCount = skipped
        };
    }
}
