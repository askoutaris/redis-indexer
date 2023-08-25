using System.Linq.Expressions;

namespace RedisIndexer.Utils
{
	public interface IExpressionHelper
	{
		string GetMemberPath<TType>(Expression expression);
	}
}
