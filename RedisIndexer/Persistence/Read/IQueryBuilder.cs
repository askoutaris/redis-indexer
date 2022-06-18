using System;
using System.Linq.Expressions;

namespace RedisIndexer.Persistence.Read
{
	public interface IQueryBuilder<TType>
	{
		QueryBuilder<TType> ByValueRange<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string? valueFrom, string? valueTo);
		QueryBuilder<TType> ByValue<TProperty>(Expression<Func<TType, TProperty>> propertySelector, string value);
		IIndexQueryable[] ToQueryables();
	}
}
