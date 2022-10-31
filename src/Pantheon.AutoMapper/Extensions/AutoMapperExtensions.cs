using AutoMapper;
using Pantheon.Enumerations;

namespace Pantheon.AutoMapper.Extensions
{
	public static class AutoMapperExtensions
	{

		public static IMappingOperationOptions SetOption(this IMappingOperationOptions options, MapperOptions optionKey, object optionValue)
		{
			options.Items[optionKey.ToString()] = optionValue;
			return options;
		}

		public static T GetOption<T>(this ResolutionContext context, MapperOptions optionKey)
		{
			if (context.Options.Items.TryGetValue(optionKey.ToString(), out var result))
			{
				return (T)result;
			}

			throw new InvalidOperationException($"Option with key '{optionKey}' not found in MapperContext.");
		}
	}
}
