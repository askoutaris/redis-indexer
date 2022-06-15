using System.Threading.Tasks;
using RedisIndexer.Factories;
using RedisIndexer.Persistence;
using RedisIndexer.Serializers;

namespace RedisIndexer.Persistence.Write
{
	class IndexWriter<TType> : IIndexWriter<TType>
	{
		private readonly IRedisContext _redisContext;
		private readonly IRedisEntityAdapter<DocumentValues> _documentValues;
		private readonly IRedisEntityAdapter<DocumentSource> _documentSources;
		private readonly IDocumentValuesFactory<TType> _documentValuesFactory;
		private readonly IRedisSerializer<TType> _documentSerializer;

		public IndexWriter(
			IRedisContext redisContext,
			IRedisEntityAdapter<DocumentValues> documentValues,
			IRedisEntityAdapter<DocumentSource> documentSources,
			IDocumentValuesFactory<TType> documentValuesFactory,
			IRedisSerializer<TType> documentSerializer)
		{
			_redisContext = redisContext;
			_documentValues = documentValues;
			_documentSources = documentSources;
			_documentValuesFactory = documentValuesFactory;
			_documentSerializer = documentSerializer;
		}

		public async Task Write(string documentKey, TType obj)
		{
			await TryRemoveOldDocumentValues(documentKey);

			IndexNewDocumentValues(documentKey, obj);

			UpdateDocumentSource(documentKey, obj);

			await _redisContext.CommitTransactional();
		}

		public async Task Remove(string documentKey)
		{
			await TryRemoveOldDocumentValues(documentKey);

			_documentSources.Remove(documentKey);

			await _redisContext.CommitTransactional();
		}

		private void IndexNewDocumentValues(string documentKey, TType obj)
		{
			var documentValues = _documentValuesFactory.GetDocumentValues(documentKey, obj);

			AddOrUpdate(documentValues);
		}

		private void UpdateDocumentSource(string documentKey, TType obj)
		{
			var source = _documentSerializer.Serialize(obj);
			var document = new DocumentSource(documentKey, source.ToString());
			_documentSources.Set(documentKey, document, null);
		}

		private async Task TryRemoveOldDocumentValues(string documentKey)
		{
			var oldDocumentValues = await _documentValues.TryGet(documentKey);

			if (oldDocumentValues is not null)
				Remove(oldDocumentValues);
		}

		private void AddOrUpdate(DocumentValues document)
		{
			_documentValues.Set(document.Key, document, null);

			foreach (var value in document.Values)
				value.WriteToRedis(_redisContext);
		}

		private void Remove(DocumentValues document)
		{
			_documentValues.Remove(document.Key);

			foreach (var value in document.Values)
				value.RemoveFromRedis(_redisContext);
		}
	}
}
