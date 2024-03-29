﻿using RedisIndexer.Persistence;

namespace RedisIndexer.Entities
{
	public abstract partial class IndexedValue
	{
		public string Value { get; }

		protected IndexedValue(string value)
		{
			Value = value;
		}

		public abstract void WriteToRedis(IRedisContext redisContext);
		public abstract void RemoveFromRedis(IRedisContext redisContext);
	}
}
