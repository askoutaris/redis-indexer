using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Persistence.Write;

namespace RedisIndexer.Factories
{
	public interface IDocumentValuesFactory<TType>
	{
		IDocumentValuesFactory<TType> AddValue<TProperty>(Expression<Func<TType, TProperty>> propertySelector, Func<TProperty, string> valueFactory);
		IDocumentValuesFactory<TType> AddCollection<TProperty>(Expression<Func<TType, IEnumerable<TProperty>>> propertySelector, Func<TProperty, string> valueFactory);
		DocumentValues GetDocumentValues(string documentKey, TType obj);
		public int MappingsCount { get; }
	}
}
