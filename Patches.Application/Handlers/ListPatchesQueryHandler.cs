using System;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Shared.Dtos;
using Patches.Shared.Queries;

namespace Patches.Application.Handlers;

public class ListPatchesQueryHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork
) : IHandler<ListPatchesQuery, ListPatchesQueryResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    public async Task<ListPatchesQueryResult> HandleAsync(ListPatchesQuery request, CancellationToken ct = default)
    {
        var patches = unitOfWork.Patches.GetAll();
        return new ListPatchesQueryResult
        {
            Patches = [.. patches.Select(mapper.Map<PatchListItemDto>)]
        };
    }
}
