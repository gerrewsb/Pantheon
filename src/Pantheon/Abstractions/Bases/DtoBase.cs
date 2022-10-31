namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Base for Dto's with a string as ID
	/// </summary>
	public abstract class DtoBase : DtoBase<string>
    {
		public DtoBase()
		{
            ID = Guid.NewGuid().ToString();
		}
    }

	/// <summary>
	/// Base for Dto's with an equatable ID
	/// </summary>
	public abstract class DtoBase<TKey>
        where TKey : IEquatable<TKey>
	{
		public TKey ID { get; set; } = default!;
	}
}
