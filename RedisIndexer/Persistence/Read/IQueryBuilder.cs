using System;
using System.Linq.Expressions;

namespace RedisIndexer.Persistence.Read
{
	public interface IQueryBuilder<TType>
	{
		QueryBuilder<TType> AddRange<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string? valueFrom, string? valueTo);
		QueryBuilder<TType> AddExact<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string value);
		IIndexQueryable[] ToQueryables();
	}
}
