using System.Globalization;

namespace RedisIndexer.ValueConverters
{
	class Int16ValueConverter : IValueConverter<short>
	{
		public string Convert(short value)
			=> value.ToString(CultureInfo.InvariantCulture);
	}
}
