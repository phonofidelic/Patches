using NSubstitute;
using Patches.Application.Contracts;
using Patches.Application.Handlers;
using Patches.Domain.Entities;
using Patches.Shared.Commands;
using Xunit;

namespace Patches.Tests;

public class DeleteConnectionHandlerTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IConnectionRepository _connections = Substitute.For<IConnectionRepository>();

    public DeleteConnectionHandlerTests()
    {
        _unitOfWork.Connections.Returns(_connections);
    }

    [Fact]
    public async Task HandleAsync_ConnectionExists_DeletesAndReturnsSuccess()
    {
        var connection = new Connection { PatchId = 1, InputId = 2, OutputId = 3 };
        _connections
            .FindByCondition(Arg.Any<System.Linq.Expressions.Expression<Func<Connection, bool>>>(), trackChanges: true)
            .Returns(new[] { connection }.AsQueryable());

        var handler = new DeleteConnectionHandler(_unitOfWork);
        var result = await handler.HandleAsync(new DeleteConnectionCommand(1, 2, 3));

        _connections.Received(1).Remove(connection);
        await _unitOfWork.Received(1).SaveChangesAsync();
        Assert.True(result.Success);
    }

    [Fact]
    public async Task HandleAsync_ConnectionNotExists_ReturnsNotSuccess()
    {
        _connections
            .FindByCondition(Arg.Any<System.Linq.Expressions.Expression<Func<Connection, bool>>>(), trackChanges: true)
            .Returns(Array.Empty<Connection>().AsQueryable());

        var handler = new DeleteConnectionHandler(_unitOfWork);
        var result = await handler.HandleAsync(new DeleteConnectionCommand(99, 1, 1));

        _connections.DidNotReceive().Remove(Arg.Any<Connection>());
        Assert.False(result.Success);
    }
}
