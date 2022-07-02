using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedisIndexer.Entities;
using RedisIndexer.Factories;
using RedisIndexer.Persistence;
using RedisIndexer.Persistence.Read;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Serializers;
using RedisIndexer.Utils;
using StackExchange.Redis;

namespace RedisIndexer
{
	public class IndexManager<TType> : IIndexManager<TType>
	{
		private readonly IValueFactory _valueFactory;
		private readonly IDocumentValuesFactory<TType> _documentValuesFactory;
		private readonly IRedisSerializer<DocumentValues> _documentValuesSerializer;
		private readonly IRedisSerializer<DocumentSource> _documentSourceSerializer;
		private readonly IRedisSerializer<TType> _documentSerializer;
		private readonly IExpressionHelper _expressionHelper;
		private readonly ILoggerFactory? _loggerFactory;

		public IndexManager(
			IValueFactory valueFactory,
			IDocumentValuesFactory<TType> documentValuesFactory,
			IRedisSerializer<DocumentValues> documentValuesSerializer,
			IRedisSerializer<DocumentSource> documentSourceSerializer,
			IRedisSerializer<TType> documentSerializer,
			IExpressionHelper expressionHelper,
			ILoggerFactory? loggerFactory)
		{
			_valueFactory = valueFactory;
			_documentValuesFactory = documentValuesFactory;
			_documentValuesSerializer = documentValuesSerializer;
			_documentSourceSerializer = documentSourceSerializer;
			_documentSerializer = documentSerializer;
			_expressionHelper = expressionHelper;
			_loggerFactory = loggerFactory;
		}

		public async Task Index(IDatabase database, string documentKey, TType obj)
		{
			var redisContext = new RedisContext(database);

			await Index(redisContext, documentKey, obj);
		}

		public async Task ConcurrentIndex(IDatabase database, string documentKey, string concurrencyToken, TType obj)
		{
			var redisContext = new RedisContext(database);

			await ConcurrentIndex(redisContext, documentKey, concurrencyToken, obj);
		}

		public async Task Index(IRedisContext redisContext, string documentKey, TType obj)
		{
			var indexWriter = GetIndexWriter(redisContext);

			await indexWriter.Write(documentKey, obj);
		}

		public async Task ConcurrentIndex(IRedisContext redisContext, string documentKey, string concurrencyToken, TType obj)
		{
			var indexWriter = GetIndexWriter(redisContext);

			await indexWriter.ConcurrentWrite(documentKey, concurrencyToken, obj);
		}

		public async Task Remove(IDatabase database, string documentKey)
		{
			var redisContext = new RedisContext(database);

			await Remove(redisContext, documentKey);
		}

		public async Task Remove(IRedisContext redisContext, string documentKey)
		{
			var indexWriter = GetIndexWriter(redisContext);

			await indexWriter.Remove(documentKey);
		}

		public Task<string[]> SearchKeys(IDatabase database, Action<IQueryBuilder<TType>> filters)
		{
			var queryBuilder = new QueryBuilder<TType>(_expressionHelper, _valueFactory);

			filters(queryBuilder);

			var queryables = queryBuilder.ToQueryables();

			return SearchKeys(database, queryables);
		}

		public async Task<string[]> SearchKeys(IDatabase database, params IIndexQueryable[] queryables)
		{
			var allKeys = new List<string>();
			foreach (var queryable in queryables)
			{
				var keys = await queryable.FetchKeys(database);
				allKeys.AddRange(keys);
			}

			return allKeys
				.Distinct()
				.ToArray();
		}

		private IIndexWriter<TType> GetIndexWriter(IRedisContext redisContext)
		{
			var documentValuesAdapter = new RedisEntityAdapter<DocumentValues>(redisContext, _documentValuesSerializer);
			var documentSourcesAdapter = new RedisEntityAdapter<DocumentSource>(redisContext, _documentSourceSerializer);

			return new IndexWriter<TType>(
				redisContext: redisContext,
				documentValues: documentValuesAdapter,
				documentSources: documentSourcesAdapter,
				documentValuesFactory: _documentValuesFactory,
				documentSerializer: _documentSerializer);
		}
	}
}
