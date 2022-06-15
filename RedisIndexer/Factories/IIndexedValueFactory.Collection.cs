using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Utils;

namespace RedisIndexer.Factories
{
	class CollectionIndexedValueFactory<TType, TProperty> : IIndexedValueFactory<TType>
	{
		private readonly string _setKey;
		private readonly IValueFactory _valueFactory;
		private readonly Func<TType, IEnumerable<TProperty>> _propertySelector;
		private readonly Func<TProperty, string> _valueSelector;

		public CollectionIndexedValueFactory(
			IExpressionHelper expressionHelper,
			IValueFactory valueFactory,
			Expression<Func<TType, IEnumerable<TProperty>>> propertySelector,
			Func<TProperty, string> valueSelector)
		{
			_setKey = expressionHelper.GetMemberPath(propertySelector);
			_valueFactory = valueFactory;
			_propertySelector = propertySelector.Compile();
			_valueSelector = valueSelector;
		}

		public IEnumerable<IndexedValue> GetValues(string documentKey, TType obj)
		{
			var properties = _propertySelector(obj);
			foreach (var property in properties)
			{
				var value = _valueSelector(property);
				var indexedValue = _valueFactory.CreateDocumentValue(value, documentKey);
				yield return new IndexedValue.SortedSet(setKey: _setKey, value: indexedValue);
			}
		}
	}
}
