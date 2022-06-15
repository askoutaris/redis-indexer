using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Utils;

namespace RedisIndexer.Factories
{

	class MemberIndexedValueFactory<TType, TProperty> : IIndexedValueFactory<TType>
	{
		private readonly string _setKey;
		private readonly Func<TType, TProperty> _propertySelector;
		private readonly IValueFactory _valueFactory;
		private readonly Func<TProperty, string> _valueSelector;

		public MemberIndexedValueFactory(
			IExpressionHelper expressionHelper,
			IValueFactory valueFactory,
			Expression<Func<TType, TProperty>> propertySelector,
			Func<TProperty, string> valueSelector)
		{
			_setKey = expressionHelper.GetMemberPath(propertySelector);
			_propertySelector = propertySelector.Compile();
			_valueFactory = valueFactory;
			_valueSelector = valueSelector;
		}

		public IEnumerable<IndexedValue> GetValues(string documentKey, TType obj)
		{
			var property = _propertySelector(obj);
			var value = _valueSelector(property);
			var indexedValue = _valueFactory.CreateDocumentValue(value, documentKey);

			yield return new IndexedValue.SortedSet(setKey: _setKey, value: indexedValue);
		}
	}
}
