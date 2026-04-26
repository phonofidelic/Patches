using Patches.Application.Contracts;
using Patches.Shared.Commands;

namespace Patches.Application.Handlers;

public class InitializePatchMatrixHandler : IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>
{
    public async Task<InitializePatchMatrixResult> HandleAsync(
        InitializePatchMatrixCommand request,
        CancellationToken ct = default)
    {
        return new();
    }
}
