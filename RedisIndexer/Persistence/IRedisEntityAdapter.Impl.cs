using System;
using System.Threading.Tasks;
using RedisIndexer.Serializers;
using StackExchange.Redis;

namespace RedisIndexer.Persistence
{
	class RedisEntityAdapter<TEntity> : IRedisEntityAdapter<TEntity> where TEntity : class
	{
		protected readonly IRedisContext _context;
		protected readonly IRedisSerializer<TEntity> _serializer;

		public RedisEntityAdapter(IRedisContext context, IRedisSerializer<TEntity> serializer)
		{
			_context = context;
			_serializer = serializer;
		}

		public async Task<bool> KeyExists(RedisKey key)
		{
			return await _context.Database.KeyExistsAsync(GetEntityKey(key.ToString()));
		}

		public async Task<TEntity?> TryGet(RedisKey key)
		{
			var value = await _context.Database.StringGetAsync(key: GetEntityKey(key.ToString()));

			if (value.IsNull)
				return null;

			var entity = _serializer.Deserialize(value);

			return entity;
		}

		public void Set(string key, TEntity entity, TimeSpan? expiry)
		{
			_context.Add(db => db.StringSetAsync(
				key: GetEntityKey(key),
				value: _serializer.Serialize(entity),
				expiry: expiry
			));
		}

		public void ConcurrentSet(string key, TEntity entity, string? expectedConcurrencyToken, string newConcurrencyToken, TimeSpan? expiry)
		{
			var versionKey = GetVersionKey(key);

			_context.Add(db => db.StringSetAsync(versionKey, newConcurrencyToken, expiry));

			Condition condition;
			if (expectedConcurrencyToken is null)
				condition = Condition.KeyNotExists(versionKey);
			else
				condition = Condition.StringEqual(versionKey, expectedConcurrencyToken);

			_context.AddCondition(condition);

			Set(key, entity, expiry);
		}

		public void Remove(RedisKey key)
		{
			_context.Add(db => db.KeyDeleteAsync(
				key: GetEntityKey(key.ToString())
			));

			_context.Add(db => db.KeyDeleteAsync(
				key: GetVersionKey(key.ToString())
			));
		}

		private string GetEntityKey(string key)
			=> $"{typeof(TEntity).FullName}-{key}";

		private string GetVersionKey(string key)
			=> $"{typeof(TEntity).FullName}Version-{key}";
	}
}
