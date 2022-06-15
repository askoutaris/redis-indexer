using System.Collections.Generic;
using RedisIndexer.Persistence.Write;

namespace RedisIndexer.Factories
{
	interface IIndexedValueFactory<TType>
	{
		IEnumerable<IndexedValue> GetValues(string documentKey, TType obj);
	}
}
