using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace RedisIndexer.Serializers
{
	class TypeNameSerializationBinder : DefaultSerializationBinder
	{
		private readonly ILogger<TypeNameSerializationBinder>? _logger;
		private readonly Dictionary<string, Type> _jsonDiscriminatorTypes = new Dictionary<string, Type>();

		public TypeNameSerializationBinder(ILogger<TypeNameSerializationBinder>? logger)
		{
			_logger = logger;
			LoadAllJsonDiscriminatorTypes();
		}

		public override void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
		{
			if (!(serializedType.GetCustomAttribute(typeof(JsonDiscriminatorAttribute)) is JsonDiscriminatorAttribute jsonDiscriminatorAttribute))
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}
			else
			{
				assemblyName = null;
				typeName = jsonDiscriminatorAttribute.Name;
			}
		}

		public override Type BindToType(string? assemblyName, string typeName)
		{
			if (_jsonDiscriminatorTypes.TryGetValue(typeName, out Type? type))
				return type;

			return base.BindToType(assemblyName, typeName);
		}

		private void LoadAllJsonDiscriminatorTypes()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				try
				{
					var types = assembly
						.GetTypes()
						.Where(x => x.GetCustomAttribute(typeof(JsonDiscriminatorAttribute)) != null)
						.Select(type => new
						{
							JsonDiscriminator = (type.GetCustomAttribute(typeof(JsonDiscriminatorAttribute)) as JsonDiscriminatorAttribute)!.Name,
							Type = type
						}).ToArray();

					foreach (var type in types)
					{
						if (_jsonDiscriminatorTypes.ContainsKey(type.JsonDiscriminator))
							throw new ArgumentException($"Dublicate JsonDiscriminator {type.JsonDiscriminator}");

						_jsonDiscriminatorTypes.Add(type.JsonDiscriminator, type.Type);
					}
				}
				catch (ReflectionTypeLoadException ex)
				{
					if (_logger == null)
						throw;

					_logger.LogError(ex, ex.Message);
				}
			}
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class JsonDiscriminatorAttribute : Attribute
	{
		public string Name { get; }

		public JsonDiscriminatorAttribute(string name)
		{
			Name = name;
		}
	}
}
