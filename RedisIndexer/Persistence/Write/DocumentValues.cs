using System.Collections.Generic;

namespace RedisIndexer.Persistence.Write
{
	public class DocumentValues
	{
		public string Key { get; }
		public IReadOnlyCollection<IndexedValue> Values { get; }

		public DocumentValues(string key, IReadOnlyCollection<IndexedValue> values)
		{
			Key = key;
			Values = values;
		}
	}
}
