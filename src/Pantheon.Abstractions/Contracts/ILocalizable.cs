namespace Pantheon.Abstractions.Contracts
{
	/// <summary>
	/// To be used for Dto's so the proper Description field can be put in the Translation based on the users language
	/// </summary>
	public interface ILocalizable
	{
		string? Translation { get; set; }
	}
}
