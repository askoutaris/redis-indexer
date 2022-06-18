using System;
using System.Collections.Generic;
using System.Text;

namespace RedisIndexer.Utils
{
	public interface IStringTokenizer
	{
		IEnumerable<string> Tokenize(string value);
	}
}
