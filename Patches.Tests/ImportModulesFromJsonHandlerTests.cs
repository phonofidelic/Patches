using NSubstitute;
using Patches.Application.Contracts;
using Patches.Application.Handlers;
using Patches.Domain.Entities;
using Patches.Shared.Commands;
using Xunit;

namespace Patches.Tests;

public class ImportModulesFromJsonHandlerTests
{
    private readonly IModulargridApiClient _apiClient = Substitute.For<IModulargridApiClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRepository<Module, Guid> _modules = Substitute.For<IRepository<Module, Guid>>();
    private readonly IRepository<Vendor, Guid> _vendors = Substitute.For<IRepository<Vendor, Guid>>();

    public ImportModulesFromJsonHandlerTests()
    {
        _unitOfWork.Modules.Returns(_modules);
        _unitOfWork.Vendors.Returns(_vendors);
    }

    [Fact]
    public async Task SkipsModule_WhenExistingModuleNameDiffersOnlyByCase()
    {
        var existingVendor = new Vendor { Name = "Mutable Instruments" };
        var existingModule = new Module { Name = "Plaits", Vendor = existingVendor };
        _modules.GetAll().Returns([existingModule]);
        _vendors.GetAll().Returns([existingVendor]);
        _apiClient.ParseModulesFromJson(Arg.Any<string>())
            .Returns([new ModulargridModuleDto("plaits", "", 14, "Mutable Instruments")]);

        var handler = new ImportModulesFromJsonHandler(_apiClient, _unitOfWork);
        var result = await handler.HandleAsync(new ImportModulesFromJsonCommand { Json = "{}" });

        Assert.Equal(0, result.ImportedCount);
        Assert.Equal(1, result.SkippedCount);
    }

    [Fact]
    public async Task SkipsModule_WhenExistingVendorNameDiffersOnlyByCase()
    {
        var existingVendor = new Vendor { Name = "Moog" };
        var existingModule = new Module { Name = "Grandmother", Vendor = existingVendor };
        _modules.GetAll().Returns([existingModule]);
        _vendors.GetAll().Returns([existingVendor]);
        _apiClient.ParseModulesFromJson(Arg.Any<string>())
            .Returns([new ModulargridModuleDto("Grandmother", "", 14, "moog")]);

        var handler = new ImportModulesFromJsonHandler(_apiClient, _unitOfWork);
        var result = await handler.HandleAsync(new ImportModulesFromJsonCommand { Json = "{}" });

        Assert.Equal(0, result.ImportedCount);
        Assert.Equal(1, result.SkippedCount);
    }

    [Fact]
    public async Task ReusesVendor_WhenVendorNameDiffersOnlyByCase()
    {
        var existingVendor = new Vendor { Name = "Mutable Instruments" };
        _modules.GetAll().Returns([]);
        _vendors.GetAll().Returns([existingVendor]);
        _apiClient.ParseModulesFromJson(Arg.Any<string>())
            .Returns([new ModulargridModuleDto("Rings", "", 8, "mutable instruments")]);

        var handler = new ImportModulesFromJsonHandler(_apiClient, _unitOfWork);
        var result = await handler.HandleAsync(new ImportModulesFromJsonCommand { Json = "{}" });

        Assert.Equal(1, result.ImportedCount);
        Assert.Equal(0, result.SkippedCount);
        _vendors.DidNotReceive().Add(Arg.Any<Vendor>());
        _modules.Received(1).Add(Arg.Is<Module>(m => m.Vendor == existingVendor));
    }
}
