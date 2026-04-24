using AutoMapper;
using NSubstitute;
using Patches.Application.Contracts;
using Patches.Application.Handlers;
using Patches.Domain.Entities;
using Patches.Shared.Commands;
using Xunit;

namespace Patches.Tests;

public class AddModuleHandlerTests
{
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IRepository<Module> _modules = Substitute.For<IRepository<Module>>();

    public AddModuleHandlerTests()
    {
        _unitOfWork.Modules.Returns(_modules);
    }

    [Fact]
    public async Task HandleAsync_MapsAndSaves()
    {
        var command = new AddModuleCommand("Plaits", 14, null, null);
        var module = new Module { Name = "Plaits", HorizontalPitch = 14 };
        var expectedResult = new AddModuleResult { Name = "Plaits", HorizontalPitch = 14 };

        _mapper.Map<Module>(command).Returns(module);
        _mapper.Map<AddModuleResult>(module).Returns(expectedResult);

        var handler = new AddModuleHandler(_mapper, _unitOfWork);
        var result = await handler.HandleAsync(command);

        _mapper.Received(1).Map<Module>(command);
        _modules.Received(1).Add(module);
        await _unitOfWork.Received(1).SaveChangesAsync();
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task HandleAsync_ReturnsResultWithMappedValues()
    {
        var command = new AddModuleCommand("Rings", 8, "Resonator", "Mutable Instruments");
        var module = new Module { Name = "Rings", HorizontalPitch = 8 };
        var expectedResult = new AddModuleResult { Id = Guid.NewGuid(), Name = "Rings", HorizontalPitch = 8 };

        _mapper.Map<Module>(command).Returns(module);
        _mapper.Map<AddModuleResult>(module).Returns(expectedResult);

        var handler = new AddModuleHandler(_mapper, _unitOfWork);
        var result = await handler.HandleAsync(command);

        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.Name, result.Name);
        Assert.Equal(expectedResult.HorizontalPitch, result.HorizontalPitch);
    }
}
