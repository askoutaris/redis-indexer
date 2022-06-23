using System.Collections.Generic;

namespace RedisIndexer.Utils
{
	public interface IStringTokenizer
	{
		IEnumerable<string> Tokenize(string value);
	}
}
