namespace RedisIndexer.ValueConverters
{
	class StringValueConverter : IValueConverter<string>
	{
		public string Convert(string value)
			=> value;
	}
}
