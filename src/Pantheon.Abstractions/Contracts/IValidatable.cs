namespace Pantheon.Abstractions.Contracts
{
	/// <summary>
	/// Used by entities to check if they are currently valid/active
	/// </summary>
	public interface IValidatable
	{
		DateTime? ValidFrom { get; set; }
		DateTime? ValidUntil { get; set; }
	}
}
