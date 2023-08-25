using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Entities;
using RedisIndexer.ValueConverters;

namespace RedisIndexer.Factories
{
	public interface IDocumentValuesFactory<TType>
	{
		public int MappingsCount { get; }

		DocumentValues GetDocumentValues(string documentKey, string? concurrencyToken, TType obj);

		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<bool>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<string>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<short>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<int>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<long>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<DateTime>>> propertySelector);
		IDocumentValuesFactory<TType> AddKeywordCollection<TProperty>(Expression<Func<TType, IEnumerable<TProperty>>> propertySelector, IValueConverter<TProperty> converter);

		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, bool>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, string>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, short>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, int>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, long>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, DateTime>> propertySelector);
		IDocumentValuesFactory<TType> AddKeyword<TProperty>(Expression<Func<TType, TProperty>> propertySelector, IValueConverter<TProperty> converter);

		IDocumentValuesFactory<TType> AddTokenized(Expression<Func<TType, string>> propertySelector, int minLength);
	}
}
