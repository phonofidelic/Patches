using System;
using Patches.Application.Commands;
using Patches.Application.Contracts;

namespace Patches.Application.Handlers;

public class InitializePatchMatrixHandler : IHandler<InitializePatchMatrixCommand, InitializePatchMatrixResult>
{
    public async Task<InitializePatchMatrixResult> HandleAsync(InitializePatchMatrixCommand request)
    {
        return new();
    }
}
