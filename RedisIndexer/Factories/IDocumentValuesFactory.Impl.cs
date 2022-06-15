using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Utils;

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

		public IDocumentValuesFactory<TType> AddValue<TProperty>(Expression<Func<TType, TProperty>> propertySelector, Func<TProperty, string> valueFactory)
		{
			_valueFactories.Add(new MemberIndexedValueFactory<TType, TProperty>(_expressionHelper, _valueFactory, propertySelector, valueFactory));
			return this;
		}

		public IDocumentValuesFactory<TType> AddCollection<TProperty>(Expression<Func<TType, IEnumerable<TProperty>>> propertySelector, Func<TProperty, string> valueFactory)
		{
			_valueFactories.Add(new CollectionIndexedValueFactory<TType, TProperty>(_expressionHelper, _valueFactory, propertySelector, valueFactory));
			return this;
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
	}
}
