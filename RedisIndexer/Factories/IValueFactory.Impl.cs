using System;

namespace RedisIndexer.Factories
{
	class ValueFactory : IValueFactory
	{
		private readonly int _maxLength;

		public ValueFactory(int maxLength = 100)
		{
			_maxLength = maxLength;
		}

		public string CreateDocumentValue(string value, string documentKey)
		{
			if (value.Length > _maxLength)
				throw new Exception($"Max length for set key is {_maxLength}. The following key exceeds this limit: {value}");

			var fullKey = value.PadRight(_maxLength, '#');

			return $"{fullKey}-{documentKey}";
		}

		public string CreateQueryableValue(string value)
		{
			if (value.Length > _maxLength)
				throw new Exception($"Max length for set key is {_maxLength}. The following key exceeds this limit: {value}");

			var queryableValue = value.PadRight(_maxLength, '#');

			return queryableValue;
		}
	}
}
