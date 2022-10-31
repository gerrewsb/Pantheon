namespace Pantheon.Abstractions.Bases
{
	/// <summary>
	/// Base for entities with string as ID
	/// </summary>
	public abstract class EntityBase : EntityBase<string>
	{
		public EntityBase()
		{
			ID = Guid.NewGuid().ToString();
		}
	}

	/// <summary>
	/// Base for entities with an equatable ID
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public abstract class EntityBase<TKey>
		where TKey : IEquatable<TKey>
	{
		public TKey ID { get; set; } = default!;
	}
}
