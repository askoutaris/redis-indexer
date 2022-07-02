using RedisIndexer.Persistence;
using RedisIndexer.Serializers;

namespace RedisIndexer.Entities
{
	abstract partial class IndexedValue
	{
		[JsonDiscriminator("IndexedValue.SortedSet")]
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
