using System.Globalization;

namespace RedisIndexer.ValueConverters
{
	class Int32ValueConverter : IValueConverter<int>
	{
		public string Convert(int value)
			=> value.ToString(CultureInfo.InvariantCulture);
	}
}
