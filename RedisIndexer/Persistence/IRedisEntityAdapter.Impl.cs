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

		public void Remove(RedisKey key)
		{
			_context.Add(db => db.KeyDeleteAsync(
				key: GetEntityKey(key.ToString())
			));
		}

		private string GetEntityKey(string key)
			=> $"{typeof(TEntity).FullName}-{key}";
	}
}
