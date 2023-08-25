using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RedisIndexer.Utils
{
	class ExpressionHelper : IExpressionHelper
	{
		public string GetMemberPath<TType>(Expression expressionSelector)
		{
			var path = new StringBuilder($"RedisIndexer.{typeof(TType).Name}");
			Expression? expression = expressionSelector;
			do
			{
				if (expression is LambdaExpression lambda)
				{
					expression = lambda.Body;
				}
				else if (expression is MemberExpression member)
				{
					if (member is null)
						throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

					if (member.Member is not PropertyInfo propInfo)
						throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

					PathAdd(path, propInfo.Name);

					return path.ToString();
				}
				else if (expression is MethodCallExpression methodCall)
				{
					var memberAccess = methodCall.Arguments.OfType<MemberExpression>().SingleOrDefault();
					if (memberAccess is null)
						throw new ArgumentException($"Expression '{expression}' inside of '{expressionSelector}' is not a member access expression");

					if (memberAccess.Member is not PropertyInfo propInfo)
						throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

					PathAdd(path, propInfo.Name);

					expression = methodCall.Arguments.OfType<LambdaExpression>().FirstOrDefault();
					if (expression is null)
						throw new ArgumentException($"Expression '{expression}' inside of '{expressionSelector}' is not a member access lambda function");
				}
			} while (expression is not null);

			return path.ToString();
		}

		private void PathAdd(StringBuilder sb, string value)
		{
			if (sb.Length > 0)
				sb.Append('.');

			sb.Append(value);
		}
	}
}
