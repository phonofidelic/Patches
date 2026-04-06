using AutoMapper;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Shared.Commands;

namespace Patches.Application.Handlers;

public class AddModuleHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork
) : IHandler<AddModuleCommand, AddModuleResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork repository = unitOfWork;
    public async Task<AddModuleResult> HandleAsync(AddModuleCommand command)
    {
        var module = mapper.Map<Module>(command);

        repository.Modules.Add(module);
        await repository.SaveChangesAsync();

        return mapper.Map<AddModuleResult>(module);
    }
}
