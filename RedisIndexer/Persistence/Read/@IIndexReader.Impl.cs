using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RedisIndexer.Factories;
using RedisIndexer.ExpressionHelpers;
using StackExchange.Redis;

namespace RedisIndexer.Persistence.Read
{
	class IndexReader<TType> : IIndexReader<TType>
	{
		private readonly IDatabase _db;
		private readonly IExpressionHelper _expressionHelper;
		private readonly IValueFactory _valueFactory;

		public IndexReader(IDatabase db, IExpressionHelper expressionHelper, IValueFactory valueFactory)
		{
			_db = db;
			_expressionHelper = expressionHelper;
			_valueFactory = valueFactory;
		}

		public Task<string[]> SearchKeys(Action<IQueryBuilder<TType>> filters)
		{
			var queryBuilder = new QueryBuilder<TType>(_expressionHelper, _valueFactory);

			filters(queryBuilder);

			var queryables = queryBuilder.ToQueryables();

			return SearchKeys(queryables);
		}

		public async Task<string[]> SearchKeys(params IIndexQueryable[] queryables)
		{
			var allKeys = new List<string>();
			foreach (var queryable in queryables)
			{
				var keys = await queryable.FetchKeys(_db);
				allKeys.AddRange(keys);
			}

			return allKeys
				.Distinct()
				.ToArray();
		}
	}
}
