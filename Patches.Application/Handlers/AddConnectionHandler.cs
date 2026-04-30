using System;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Domain.ValueObjects;
using Patches.Shared.Commands;
using Patches.Shared.Dtos;

namespace Patches.Application.Handlers;

public class AddConnectionHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork
) : IHandler<AddConnectionCommand, AddConnectionResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    public async Task<AddConnectionResult> HandleAsync(AddConnectionCommand command, CancellationToken ct = default)
    {
        var inputConnectionPoint = await unitOfWork.ConnectionPoints
            .FindByIdAsync(command.Input.Id, trackChanges: true, ct)
                ?? throw new Exception($"Could not find input ConnectionPoint with Id '{command.Input.Id}'");

        var outputConnectionPoint = await unitOfWork.ConnectionPoints
            .FindByIdAsync(command.Output.Id, trackChanges: true, ct)
                ?? throw new Exception($"Could not find output ConnectionPoint with Id '{command.Output.Id}'");

        var patch = await unitOfWork.Patches
            .FindByIdAsync(command.PatchId, trackChanges: true, ct);

        if (patch == null)
        {
            patch = new Patch
            {
                Name = "New Patch",
            };
            unitOfWork.Patches.Add(patch);
        }

        var connection = new Connection
        {
            PatchId = patch.Id,
            InputId = inputConnectionPoint.Id,
            OutputId = outputConnectionPoint.Id,
        };
        
        unitOfWork.Connections.Add(connection);
        // patch.Connections.Add(connection);

        await unitOfWork.SaveChangesAsync();

        return new AddConnectionResult
        {
            Connection = mapper.Map<ConnectionDto>(connection)
        };
    }
}
