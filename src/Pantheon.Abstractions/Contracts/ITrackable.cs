namespace Pantheon.Abstractions.Contracts
{
	/// <summary>
	/// To be used by Entities where it needs to be clear when the entity was last updated
	/// </summary>
	public interface ITrackable
	{
		DateTime LastUpdate { get; set; }
	}
}
