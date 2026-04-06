namespace Patches.Application.Contracts;

public interface IHandler<TRequest, TResult>
{
    Task<TResult> HandleAsync(TRequest request);
}
