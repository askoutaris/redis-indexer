using System;
using System.Globalization;

namespace RedisIndexer.ValueConverters
{
	class Int64ValueConverter : IValueConverter<long>
	{
		public string Convert(long value)
			=> value.ToString(CultureInfo.InvariantCulture);
	}
}
