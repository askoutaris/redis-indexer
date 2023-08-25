using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Entities;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Utils;
using RedisIndexer.ValueConverters;

namespace RedisIndexer.Factories
{

	class KeywordIndexedValueFactory<TType, TProperty> : IIndexedValueFactory<TType>
	{
		private readonly string _setKey;
		private readonly Func<TType, TProperty> _propertySelector;
		private readonly IValueFactory _valueFactory;
		private readonly IValueConverter<TProperty> _converter;

		public KeywordIndexedValueFactory(
			IExpressionHelper expressionHelper,
			Expression<Func<TType, TProperty>> propertySelector,
			IValueFactory valueFactory,
			IValueConverter<TProperty> converter)
		{
			_setKey = expressionHelper.GetMemberPath<TType>(propertySelector);
			_propertySelector = propertySelector.Compile();
			_valueFactory = valueFactory;
			_converter = converter;
		}

		public IEnumerable<IndexedValue> GetValues(string documentKey, TType obj)
		{
			var propertyValue = _propertySelector(obj);
			var value = _converter.Convert(propertyValue);
			var indexedValue = _valueFactory.CreateDocumentValue(value, documentKey);

			yield return new IndexedValue.SortedSet(setKey: _setKey, value: indexedValue);
		}
	}
}
