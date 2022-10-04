using AutoMapper;
using Microsoft.AspNetCore.Http;
using Pantheon.Abstractions.Contracts;
using Pantheon.Extensions;

namespace Pantheon.AutoMapper.Actions
{
	/// <summary>
	/// Add a translation action as an AfterMap to a <see cref="Profile"/>
	/// </summary>
	public class TranslationAction : IMappingAction<ITranslatable, ILocalizable>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TranslationAction(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public void Process(ITranslatable source, ILocalizable destination, ResolutionContext context)
		{
			destination.Translation = source.GetTranslation(_httpContextAccessor.GetMemberLanguageISO());
		}
	}
}
