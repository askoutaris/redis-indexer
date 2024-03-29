﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Factories;
using RedisIndexer.Utils;

namespace RedisIndexer.Persistence.Read
{
	public class QueryBuilder<TType> : IQueryBuilder<TType>
	{
		private readonly IExpressionHelper _expressionHelper;
		private readonly IValueFactory _valueFactory;
		private readonly List<IIndexQueryable> _queryables;

		public QueryBuilder(IExpressionHelper expressionHelper, IValueFactory valueFactory)
		{
			_expressionHelper = expressionHelper;
			_valueFactory = valueFactory;
			_queryables = new List<IIndexQueryable>();
		}

		public QueryBuilder<TType> ByValue<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string value)
		{
			var setKey = _expressionHelper.GetMemberPath<TType>(propertySelector);
			var queryableValueFrom = value != null ? _valueFactory.CreateQueryableValue(value) : null;
			var queryableValueTo = value != null ? _valueFactory.CreateQueryableValue(value) + "\xff" : null;
			_queryables.Add(new IndexQueryable.SortedSetRange(setKey, queryableValueFrom, queryableValueTo));
			return this;
		}

		public QueryBuilder<TType> ByValueRange<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string? valueFrom, string? valueTo)
		{
			var setKey = _expressionHelper.GetMemberPath<TType>(propertySelector);
			var queryableValueFrom = valueFrom != null ? _valueFactory.CreateQueryableValue(valueFrom) : null;
			var queryableValueTo = valueTo != null ? _valueFactory.CreateQueryableValue(valueTo) + "\xff" : null;
			_queryables.Add(new IndexQueryable.SortedSetRange(setKey, queryableValueFrom, queryableValueTo));
			return this;
		}

		public IIndexQueryable[] ToQueryables()
		{
			return _queryables.ToArray();
		}
	}
}
