using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.Networking.Interfaces;

public interface IServiceCaller
{
    ClientResult<TResult> Call<TChannel, TArgument, TResult>(
        string address,
        Func<TChannel, TArgument, TResult> operation,
        TArgument argument)
        where TChannel : class;
}
