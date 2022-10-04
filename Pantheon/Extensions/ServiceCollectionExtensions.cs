using Microsoft.Extensions.DependencyInjection;

namespace Pantheon.Extensions
{
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// <para>Extension to be used to add all implementations of a certain interface within certain assemblies.</para>
		/// <para>The added implementation will be injected with the implemented class name instead of the interface name</para>
		/// </summary>
		/// <param name="services"></param>
		/// <param name="lifetime"></param>
		/// <param name="interfaceType"></param>
		/// <param name="markers"></param>
		public static IServiceCollection AddServices(this IServiceCollection services, ServiceLifetime lifetime, Type interfaceType, params Type[] markers)
		{
			return services.Scan(scan => scan
				.FromAssembliesOf(markers)
				.AddClasses(classes => classes.AssignableTo(interfaceType))
				.AsSelf()
				.WithLifetime(lifetime));
		}
	}
}
