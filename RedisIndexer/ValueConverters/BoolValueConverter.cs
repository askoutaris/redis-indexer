namespace RedisIndexer.ValueConverters
{
	class BoolValueConverter : IValueConverter<bool>
	{
		public string Convert(bool value)
			=> value.ToString();
	}
}
