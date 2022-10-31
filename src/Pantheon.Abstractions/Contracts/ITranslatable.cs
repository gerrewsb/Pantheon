namespace Pantheon.Abstractions.Contracts
{
    /// <summary>
    /// To be used for Entities and Dto's that have a description
    /// </summary>
	public interface ITranslatable
	{
        string? DescNL { get; set; }
        string? DescDE { get; set; }
        string? DescFR { get; set; }
        string? DescEN { get; set; }
    }
}
