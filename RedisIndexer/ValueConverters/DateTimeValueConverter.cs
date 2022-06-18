using System;

namespace RedisIndexer.ValueConverters
{
	class DateTimeValueConverter : IValueConverter<DateTime>
	{
		public string Convert(DateTime value)
			=> value.ToString("o");
	}
}
