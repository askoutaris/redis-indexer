using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RedisIndexer.Entities;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Utils;
using RedisIndexer.ValueConverters;

namespace RedisIndexer.Factories
{

	class TokenizedIndexedValueFactory<TType> : IIndexedValueFactory<TType>
	{
		private readonly string _setKey;
		private readonly Func<TType, string> _propertySelector;
		private readonly IValueFactory _valueFactory;
		private readonly IValueConverter<string> _converter;
		private readonly IStringTokenizer _tokenizer;

		public TokenizedIndexedValueFactory(
			IExpressionHelper expressionHelper,
			Expression<Func<TType, string>> propertySelector,
			IValueFactory valueFactory,
			IValueConverter<string> converter,
			IStringTokenizer tokenizer)
		{
			_setKey = expressionHelper.GetMemberPath(propertySelector);
			_propertySelector = propertySelector.Compile();
			_valueFactory = valueFactory;
			_converter = converter;
			_tokenizer = tokenizer;
		}

		public IEnumerable<IndexedValue> GetValues(string documentKey, TType obj)
		{
			var propertyValue = _propertySelector(obj);
			var value = _converter.Convert(propertyValue);

			foreach (var token in _tokenizer.Tokenize(value))
			{
				var indexedValue = _valueFactory.CreateDocumentValue(token, documentKey);
				yield return new IndexedValue.SortedSet(setKey: _setKey, value: indexedValue);
			}
		}
	}
}
