using Patches.Application.Contracts;
using Patches.Shared.Commands;

namespace Patches.Application.Handlers;

public class DeleteConnectionHandler(IUnitOfWork unitOfWork)
    : IHandler<DeleteConnectionCommand, DeleteConnectionResult>
{
    public async Task<DeleteConnectionResult> HandleAsync(
        DeleteConnectionCommand command, CancellationToken ct = default)
    {
        var existing = unitOfWork.Connections
            .FindByCondition(
                c => c.PatchId == command.PatchId &&
                     c.InputId == command.InputId &&
                     c.OutputId == command.OutputId,
                trackChanges: true)
            .FirstOrDefault();

        if (existing == null)
            return new DeleteConnectionResult { Success = false };

        unitOfWork.Connections.Remove(existing);
        await unitOfWork.SaveChangesAsync();
        return new DeleteConnectionResult { Success = true };
    }
}
