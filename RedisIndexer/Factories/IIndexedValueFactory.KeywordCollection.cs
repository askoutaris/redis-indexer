using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Persistence.Write;
using RedisIndexer.ExpressionHelpers;
using RedisIndexer.ValueConverters;

namespace RedisIndexer.Factories
{
	class KeywordCollectionIndexedValueFactory<TType, TProperty> : IIndexedValueFactory<TType>
	{
		private readonly string _setKey;
		private readonly IValueFactory _valueFactory;
		private readonly Func<TType, IEnumerable<TProperty>> _propertySelector;
		private readonly IValueConverter<TProperty> _converter;

		public KeywordCollectionIndexedValueFactory(
			IExpressionHelper expressionHelper,
			IValueFactory valueFactory,
			Expression<Func<TType, IEnumerable<TProperty>>> propertySelector,
			IValueConverter<TProperty> converter)
		{
			_setKey = expressionHelper.GetMemberPath(propertySelector);
			_valueFactory = valueFactory;
			_propertySelector = propertySelector.Compile();
			_converter = converter;
		}

		public IEnumerable<IndexedValue> GetValues(string documentKey, TType obj)
		{
			var properties = _propertySelector(obj);
			foreach (var property in properties)
			{
				var value = _converter.Convert(property);
				var indexedValue = _valueFactory.CreateDocumentValue(value, documentKey);
				yield return new IndexedValue.SortedSet(setKey: _setKey, value: indexedValue);
			}
		}
	}
}
