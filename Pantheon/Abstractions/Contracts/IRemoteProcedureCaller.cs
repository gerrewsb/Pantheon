namespace Pantheon.Abstractions.Contracts
{
	public interface IRemoteProcedureCaller<TRequest, TResult>
	{
		Task<TResult?> Get(TRequest request);
	}
}
