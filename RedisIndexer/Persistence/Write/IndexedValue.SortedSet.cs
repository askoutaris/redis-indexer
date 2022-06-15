using RedisIndexer.Persistence;

namespace RedisIndexer.Persistence.Write
{
	abstract partial class IndexedValue
	{
		public class SortedSet : IndexedValue
		{
			public string SetKey { get; }

			public SortedSet(string setKey, string value) : base(value)
			{
				SetKey = setKey;
			}

			public override void WriteToRedis(IRedisContext redisContext)
			{
				redisContext.Add(db => db.SortedSetAddAsync(SetKey, Value, 0));
			}

			public override void RemoveFromRedis(IRedisContext redisContext)
			{
				redisContext.Add(db => db.SortedSetRemoveAsync(SetKey, Value, 0));
			}
		}
	}
}
