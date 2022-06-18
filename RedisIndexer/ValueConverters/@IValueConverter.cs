namespace RedisIndexer.ValueConverters
{
	public interface IValueConverter<T>
	{
		string Convert(T value);
	}
}
