using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisIndexer.Serializers
{
	class RedisNewtonsoftSerializer<TType> : IRedisSerializer<TType>
	{
		private readonly JsonSerializerSettings? _settings;

		public RedisNewtonsoftSerializer()
		{

		}

		public RedisNewtonsoftSerializer(Action<JsonSerializerSettings> configureSettings)
		{
			_settings = new JsonSerializerSettings();
			configureSettings(_settings);
		}

		public RedisNewtonsoftSerializer(JsonSerializerSettings settings)
		{
			_settings = settings;
		}

		public TType Deserialize(RedisValue value)
		{
			var obj = JsonConvert.DeserializeObject<TType>(value.ToString(), _settings) ?? throw new Exception($"Invalid json {value}");
			return obj;
		}

		public RedisValue Serialize(TType obj)
		{
			var json = JsonConvert.SerializeObject(obj, _settings);
			return json;
		}
	}
}
