using System.Collections.Generic;

namespace RedisIndexer.Utils
{
	class StringTokenizer : IStringTokenizer
	{
		private int _minLength;

		public StringTokenizer(int minLength)
		{
			_minLength = minLength;
		}

		public IEnumerable<string> Tokenize(string value)
		{
			for (int startIndex = 0; startIndex < value.Length; startIndex++)
			{
				var takeLength = _minLength;
				while (startIndex + takeLength <= value.Length)
				{
					yield return value.Substring(startIndex, takeLength);
					takeLength++;
				}
			}
		}
	}
}
