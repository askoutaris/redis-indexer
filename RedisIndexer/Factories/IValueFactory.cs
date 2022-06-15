namespace RedisIndexer.Factories
{
	public interface IValueFactory
	{
		string CreateDocumentValue(string value, string documentKey);
		string CreateQueryableValue(string value);
	}
}
