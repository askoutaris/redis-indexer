using System.Linq;
using StackExchange.Redis;

namespace RedisIndexer
{
	static class IndexedValueExtensions
	{
		public static string GetDocumentKey(this RedisValue value)
			=> value.ToString().Split('-').Last();
	}
}
