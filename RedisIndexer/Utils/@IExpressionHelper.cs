using System.Linq.Expressions;

namespace RedisIndexer.Utils
{
	public interface IExpressionHelper
	{
		string GetMemberPath(Expression expression);
	}
}
