using System.Threading.Tasks;
using System.Xml.Linq;
using RedisIndexer.Entities;
using RedisIndexer.Factories;
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
		}

		public async Task ConcurrentWrite(string documentKey, string concurrencyToken, TType obj)
		{
			var expectedConcurrencyToken = await TryRemoveOldDocumentValues(documentKey);

			ConcurrentIndexNewDocumentValues(documentKey, expectedConcurrencyToken, concurrencyToken, obj);

			UpdateDocumentSource(documentKey, obj);
		}

		public async Task Remove(string documentKey)
		{
			await TryRemoveOldDocumentValues(documentKey);

			_documentSources.Remove(documentKey);
		}

		private void IndexNewDocumentValues(string documentKey, TType obj)
		{
			var documentValues = _documentValuesFactory.GetDocumentValues(documentKey, null, obj);

			_documentValues.Set(documentValues.Key, documentValues, null);

			foreach (var value in documentValues.Values)
				value.WriteToRedis(_redisContext);
		}

		private void ConcurrentIndexNewDocumentValues(string documentKey, string? expectedConcurrencyToken, string newConcurrencyToken, TType obj)
		{
			var documentValues = _documentValuesFactory.GetDocumentValues(documentKey, newConcurrencyToken, obj);

			_documentValues.ConcurrentSet(documentValues.Key, documentValues, expectedConcurrencyToken, newConcurrencyToken, null);

			foreach (var value in documentValues.Values)
				value.WriteToRedis(_redisContext);
		}

		private void UpdateDocumentSource(string documentKey, TType obj)
		{
			var source = _documentSerializer.Serialize(obj);
			var document = new DocumentSource(documentKey, source.ToString());
			_documentSources.Set(documentKey, document, null);
		}

		private async Task<ConcurrencyToken?> TryRemoveOldDocumentValues(string documentKey)
		{
			var oldDocumentValues = await _documentValues.TryGet(documentKey);

			if (oldDocumentValues is not null)
			{
				Remove(oldDocumentValues);
				return oldDocumentValues.ConcurrencyToken;
			}

			return null;
		}

		private void Remove(DocumentValues document)
		{
			_documentValues.Remove(document.Key);

			foreach (var value in document.Values)
				value.RemoveFromRedis(_redisContext);
		}
	}
}
