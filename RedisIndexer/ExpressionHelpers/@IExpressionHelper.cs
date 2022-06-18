using System.Linq.Expressions;

namespace RedisIndexer.ExpressionHelpers
{
	public interface IExpressionHelper
	{
		string GetMemberPath(Expression expression);
	}
}
