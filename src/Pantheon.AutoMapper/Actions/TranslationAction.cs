using AutoMapper;
using Pantheon.Abstractions.Contracts;
using Pantheon.AutoMapper.Extensions;
using Pantheon.Enumerations;
using Pantheon.Extensions;

namespace Pantheon.AutoMapper.Actions
{
	/// <summary>
	/// Add a translation action as an AfterMap to a <see cref="Profile"/>
	/// </summary>
	public class TranslationAction : IMappingAction<ITranslatable, ILocalizable>
	{
		public void Process(ITranslatable source, ILocalizable destination, ResolutionContext context)
		{
			destination.Translation = source.GetDescription(context.GetOption<LanguageISO>(MapperOptions.Language));
		}
	}
}
