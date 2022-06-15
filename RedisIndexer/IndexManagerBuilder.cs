﻿using System;
using RedisIndexer.Factories;
using RedisIndexer.Persistence;
using RedisIndexer.Persistence.Write;
using RedisIndexer.Serializers;
using RedisIndexer.Utils;
using Microsoft.Extensions.Logging;

namespace RedisIndexer
{
	public class IndexManagerBuilder<TType>
	{
		private IValueFactory _valueFactory;
		private IRedisSerializer<DocumentValues> _documentValuesSerializer;
		private IRedisSerializer<DocumentSource> _documentSourceSerializer;
		private IRedisSerializer<TType> _documentSerializer;
		private IExpressionHelper _expressionHelper;
		private Action<IDocumentValuesFactory<TType>>? _configureMappings;
		private ILoggerFactory? _loggerFactory;

		public static IndexManagerBuilder<TType> New()
			=> new IndexManagerBuilder<TType>();

		public IndexManagerBuilder()
		{
			_expressionHelper = new ExpressionHelper();
			_valueFactory = new ValueFactory();
			_documentValuesSerializer = new RedisNewtonsoftSerializer<DocumentValues>();
			_documentSourceSerializer = new RedisNewtonsoftSerializer<DocumentSource>();
			_documentSerializer = new RedisNewtonsoftSerializer<TType>();
		}

		public IndexManagerBuilder<TType> WithValueFactory(IValueFactory valueFactory)
		{
			_valueFactory = valueFactory;
			return this;
		}

		public IndexManagerBuilder<TType> WithDocumentValuesSerializer(IRedisSerializer<DocumentValues> serializer)
		{
			_documentValuesSerializer = serializer;
			return this;
		}

		public IndexManagerBuilder<TType> WithDocumentSourceSerializer(IRedisSerializer<DocumentSource> serializer)
		{
			_documentSourceSerializer = serializer;
			return this;
		}

		public IndexManagerBuilder<TType> WithDocumentSerializer(IRedisSerializer<TType> serializer)
		{
			_documentSerializer = serializer;
			return this;
		}

		public IndexManagerBuilder<TType> WithExpressionHelper(IExpressionHelper expressionHelper)
		{
			_expressionHelper = expressionHelper;
			return this;
		}

		public IndexManagerBuilder<TType> WithLoggerFactory(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
			return this;
		}

		public IndexManagerBuilder<TType> ConfigureMappings(Action<IDocumentValuesFactory<TType>>? configureMappings)
		{
			_configureMappings = configureMappings;
			return this;
		}

		public IIndexManager<TType> Build()
		{
			var documentValuesFactory = GetDocumentValuesFactory();

			return new IndexManager<TType>(
				valueFactory: _valueFactory,
				documentValuesFactory: documentValuesFactory,
				documentValuesSerializer: _documentValuesSerializer,
				documentSourceSerializer: _documentSourceSerializer,
				documentSerializer: _documentSerializer,
				expressionHelper: _expressionHelper,
				loggerFactory: _loggerFactory);
		}

		private DocumentValuesFactory<TType> GetDocumentValuesFactory()
		{
			if (_configureMappings is null)
				throw new Exception($"{typeof(IndexManagerBuilder<TType>)} has no configured mappings. Consider use .ConfigureMappings() first!");

			var documentValueFactory = new DocumentValuesFactory<TType>(_expressionHelper, _valueFactory);

			_configureMappings(documentValueFactory);

			if (documentValueFactory.MappingsCount == 0)
				throw new Exception($"{typeof(IndexManagerBuilder<TType>)} has zero configured mappings. Consider use .ConfigureMappings() correct!");

			return documentValueFactory;
		}
	}
}