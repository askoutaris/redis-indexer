namespace RedisIndexer.Entities
{
	public class DocumentSource
	{
		public string Key { get; }
		public string Source { get; }

		public DocumentSource(string key, string source)
		{
			Key = key;
			Source = source;
		}
	}
}
