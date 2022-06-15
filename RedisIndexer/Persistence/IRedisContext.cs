using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIndexer.Persistence
{
	public interface IRedisContext
	{
		IDatabase Database { get; }
		void Add(Func<IDatabaseAsync, Task> action);
		void AddCondition(Condition condition);
		Task CommitTransactional();
	}
}
