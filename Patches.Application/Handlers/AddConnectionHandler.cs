using System;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Domain.ValueObjects;
using Patches.Shared.Commands;

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
        var inputConnectionPoint = await unitOfWork.ConnectionPoints.FindByIdAsync(command.Input.Id, ct);
        if (inputConnectionPoint == null)
        {
            var module = await unitOfWork.Modules.FindByIdAsync(command.Input.ModuleId, ct)
                ?? throw new Exception($"Could not find Module with ID '{command.Input.ModuleId}' when trying to create new ConnectionPoint '{command.Input.Name}'");
            inputConnectionPoint = new ConnectionPoint
            {
                Name = command.Input.Name,
                Module = module,
                Type = new ConnectionPointType(command.Input.Type)
            };

            unitOfWork.ConnectionPoints.Add(inputConnectionPoint);
        }

        var outputConnectionPoint = await unitOfWork.ConnectionPoints.FindByIdAsync(command.Output.Id, ct);
        if (outputConnectionPoint == null)
        {
            var module = await unitOfWork.Modules.FindByIdAsync(command.Output.ModuleId, ct)
                ?? throw new Exception($"Could not find Module with ID '{command.Input.ModuleId}' when trying to create new ConnectionPoint '{command.Output.Name}'");
            outputConnectionPoint = new ConnectionPoint
            {
                Name = command.Output.Name,
                Module = module,
                Type = new ConnectionPointType(command.Output.Type)
            };

            unitOfWork.ConnectionPoints.Add(outputConnectionPoint);
        }


        var patch = await unitOfWork.Patches.FindByIdAsync(command.PatchId, ct);

        if (patch == null)
        {
            patch = new Patch
            {
                Name = "Test Patch",
            };
            unitOfWork.Patches.Add(patch);
        }

        var connection = new Connection
        {
            // InputId = inputConnectionPoint.Id,
            // OutputId = inputConnectionPoint.Id,
            Input = inputConnectionPoint,
            Output = outputConnectionPoint,
        };
        
        unitOfWork.Connections.Add(connection);
        patch.Connections.Add(connection);

        await unitOfWork.SaveChangesAsync();

        return new AddConnectionResult();
    }
}
