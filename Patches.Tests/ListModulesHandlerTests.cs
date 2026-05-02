using AutoMapper;
using NSubstitute;
using Patches.Application.Contracts;
using Patches.Application.Handlers;
using Patches.Domain.Entities;
using Patches.Shared.Queries;

namespace Patches.Tests;

public class ListModulesHandlerTests
{
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRepository<Module, Guid> _modules = Substitute.For<IRepository<Module, Guid>>();

    public ListModulesHandlerTests()
    {
        _unitOfWork.Modules.Returns(_modules);
    }

    [Fact]
    public async Task HandleAsync_EmptyRepository_ReturnsEmptyList()
    {
        _modules.GetAll().Returns([]);

        var handler = new ListModulesHandler(_mapper, _unitOfWork);
        var result = await handler.HandleAsync(new ListModulesQuery());

        Assert.Empty(result.Modules);
    }

    [Fact]
    public async Task HandleAsync_TwoModules_ReturnsTwoMappedItems()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var module1 = new Module { Name = "Plaits", HorizontalPitch = 14 };
        var module2 = new Module { Name = "Rings", HorizontalPitch = 8 };

        _modules.GetAll().Returns([module1, module2]);
        _mapper.Map<ModuleListItem>(module1).Returns(new ModuleListItem(id1, "Plaits"));
        _mapper.Map<ModuleListItem>(module2).Returns(new ModuleListItem(id2, "Rings"));

        var handler = new ListModulesHandler(_mapper, _unitOfWork);
        var result = await handler.HandleAsync(new ListModulesQuery());

        Assert.Equal(2, result.Modules.Count);
        Assert.Equal(id1, result.Modules[0].Id);
        Assert.Equal("Plaits", result.Modules[0].Name);
        Assert.Equal(id2, result.Modules[1].Id);
        Assert.Equal("Rings", result.Modules[1].Name);
    }
}
