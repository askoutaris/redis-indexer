using System.Collections.Generic;
using RedisIndexer.Persistence.Write;

namespace RedisIndexer.Entities
{
	public class DocumentValues
	{
		public string Key { get; }
		public string? ConcurrencyToken { get; }
		public IReadOnlyCollection<IndexedValue> Values { get; }

		public DocumentValues(string key, string? concurrencyToken, IReadOnlyCollection<IndexedValue> values)
		{
			Key = key;
			ConcurrencyToken = concurrencyToken;
			Values = values;
		}
	}
}
