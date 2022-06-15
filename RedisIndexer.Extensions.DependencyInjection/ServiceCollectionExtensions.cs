using System;
using Microsoft.Extensions.DependencyInjection;

namespace RedisIndexer.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddRedisIndexer<TType>(this IServiceCollection services, Action<IndexManagerBuilder<TType>> configure)
		{
			var builder = IndexManagerBuilder<TType>.New();

			configure(builder);

			var manager = builder.Build();

			services.AddSingleton<IIndexManager<TType>>(manager);

			return services;
		}
	}
}
