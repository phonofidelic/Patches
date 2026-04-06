using Patches.Application.Commands;
using Patches.Application.Contracts;

namespace Patches.Application.Handlers;

public class AddModuleHandler : IHandler<AddModuleCommand, AddModuleResult>
{
    public async Task<AddModuleResult> HandleAsync(AddModuleCommand command)
    {
        return new AddModuleResult(
            id: Guid.NewGuid(),
            name: command.Name,
            hp: command.HorizontalPitch,
            u: command.VerticalUnits,
            connectionPoints: []
        );
    }
}
