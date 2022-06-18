using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using RedisIndexer.ExpressionHelpers;
using RedisIndexer.Persistence.Write;
using RedisIndexer.ValueConverters;

namespace RedisIndexer.Factories
{
	class DocumentValuesFactory<TType> : IDocumentValuesFactory<TType>
	{
		private readonly List<IIndexedValueFactory<TType>> _valueFactories;
		private readonly IExpressionHelper _expressionHelper;
		private readonly IValueFactory _valueFactory;

		public int MappingsCount => _valueFactories.Count;

		public DocumentValuesFactory(IExpressionHelper expressionHelper, IValueFactory valueFactory)
		{
			_valueFactories = new List<IIndexedValueFactory<TType>>();
			_expressionHelper = expressionHelper;
			_valueFactory = valueFactory;
		}

		public DocumentValues GetDocumentValues(string documentKey, TType obj)
		{
			var indexedValues = _valueFactories
				.SelectMany(factory => factory.GetValues(documentKey, obj))
				.ToArray();

			return new DocumentValues(
				key: documentKey,
				values: indexedValues);
		}

		public IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<string>>> propertySelector) => AddKeywordCollection(propertySelector, new StringValueConverter());
		public IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<short>>> propertySelector) => AddKeywordCollection(propertySelector, new Int16ValueConverter());
		public IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<int>>> propertySelector) => AddKeywordCollection(propertySelector, new Int32ValueConverter());
		public IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<long>>> propertySelector) => AddKeywordCollection(propertySelector, new Int64ValueConverter());
		public IDocumentValuesFactory<TType> AddKeywordCollection(Expression<Func<TType, IEnumerable<DateTime>>> propertySelector) => AddKeywordCollection(propertySelector, new DateTimeValueConverter());
		public IDocumentValuesFactory<TType> AddKeywordCollection<TProperty>(Expression<Func<TType, IEnumerable<TProperty>>> propertySelector, IValueConverter<TProperty> converter)
		{
			_valueFactories.Add(new KeywordCollectionIndexedValueFactory<TType, TProperty>(_expressionHelper, _valueFactory, propertySelector, converter));
			return this;
		}

		public IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, string>> propertySelector) => AddKeyword(propertySelector, new StringValueConverter());
		public IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, short>> propertySelector) => AddKeyword(propertySelector, new Int16ValueConverter());
		public IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, int>> propertySelector) => AddKeyword(propertySelector, new Int32ValueConverter());
		public IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, long>> propertySelector) => AddKeyword(propertySelector, new Int64ValueConverter());
		public IDocumentValuesFactory<TType> AddKeyword(Expression<Func<TType, DateTime>> propertySelector)=>AddKeyword(propertySelector, new DateTimeValueConverter());
		public IDocumentValuesFactory<TType> AddKeyword<TProperty>(Expression<Func<TType, TProperty>> propertySelector, IValueConverter<TProperty> converter)
		{
			_valueFactories.Add(new KeywordIndexedValueFactory<TType, TProperty>(_expressionHelper, propertySelector, _valueFactory, converter));
			return this;
		}
	}
}
