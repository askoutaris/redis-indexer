using System;
using System.Threading.Tasks;
using RedisIndexer.Persistence;
using RedisIndexer.Persistence.Read;
using StackExchange.Redis;

namespace RedisIndexer
{
	public interface IIndexManager<TType>
	{
		Task ConcurrentIndex(IDatabase database, string documentKey, string concurrencyToken, TType obj);
		Task ConcurrentIndex(IRedisContext redisContext, string documentKey, string concurrencyToken, TType obj);
		Task Index(IDatabase database, string documentKey, TType obj);
		Task Index(IRedisContext redisContext, string documentKey, TType obj);
		Task Remove(IDatabase database, string documentKey);
		Task Remove(IRedisContext redisContext, string documentKey);
		Task<string[]> SearchKeys(IDatabase database, Action<IQueryBuilder<TType>> filters);
		Task<string[]> SearchKeys(IDatabase database, params IIndexQueryable[] queryables);
	}
}
