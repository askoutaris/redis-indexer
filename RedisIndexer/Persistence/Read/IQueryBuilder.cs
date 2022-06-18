using System;
using System.Linq.Expressions;

namespace RedisIndexer.Persistence.Read
{
	public interface IQueryBuilder<TType>
	{
		QueryBuilder<TType> AddKeywordRange<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string? valueFrom, string? valueTo);
		QueryBuilder<TType> AddKeyword<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string value);
		IIndexQueryable[] ToQueryables();
	}
}
