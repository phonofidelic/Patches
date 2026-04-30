using System;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Shared.Queries;

namespace Patches.Application.Handlers;

public class ListModulesHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork) : IHandler<ListModulesQuery, ListModulesQueryResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork repository = unitOfWork;
    public async Task<ListModulesQueryResult> HandleAsync(
        ListModulesQuery request,
        CancellationToken ct = default)
    {
        var modules = repository.Modules.GetAll()
            .Select(mapper.Map<ModuleListItem>)
            .ToList();
        
        return new ListModulesQueryResult
        {
            Modules = modules
        };
    }
}
