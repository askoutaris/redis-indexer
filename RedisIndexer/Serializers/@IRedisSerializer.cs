using StackExchange.Redis;

namespace RedisIndexer.Serializers
{
	public interface IRedisSerializer<TType>
	{
		TType Deserialize(RedisValue value);
		RedisValue Serialize(TType obj);
	}
}
