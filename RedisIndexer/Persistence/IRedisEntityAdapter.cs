using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIndexer.Persistence
{
	interface IRedisEntityAdapter<TEntity> where TEntity : class
	{
		void ConcurrentSet(string key, TEntity entity, string? expectedConcurrencyToken, string newConcurrencyToken, TimeSpan? expiry);
		Task<bool> KeyExists(RedisKey key);
		void Remove(RedisKey key);
		void Set(string key, TEntity entity, TimeSpan? expiry);
		Task<TEntity?> TryGet(RedisKey key);
	}
}
