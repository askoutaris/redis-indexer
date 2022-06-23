using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIndexer.Persistence
{
	public class RedisContext : IRedisContext
	{
		private readonly List<Func<IDatabaseAsync, Task>> _actions;
		private readonly List<Condition> _conditions;
		protected readonly IDatabase _db;

		public IDatabase Database => _db;

		public RedisContext(IDatabase db)
		{
			_db = db;
			_actions = new List<Func<IDatabaseAsync, Task>>();
			_conditions = new List<Condition>();
		}

		public void Add(Func<IDatabaseAsync, Task> action)
		{
			_actions.Add(action);
		}

		public void AddCondition(Condition condition)
			=> _conditions.Add(condition);

		public void SetKey(RedisKey key, RedisValue value, TimeSpan? expiry)
		{
			_actions.Add(db => db.StringSetAsync(key, value, expiry));
		}

		public async Task CommitTransactional()
		{
			var transaction = _db.CreateTransaction();

			foreach (var condition in _conditions)
				transaction.AddCondition(condition);

			// DO NOT await commands in redis transaction
			// Commands executed inside a transaction do not return results until after you execute the transaction. This is simply a feature of how transactions work in Redis. At the moment you are awaiting something that hasn't even been sent yet (transactions are buffered locally until executed) - but even if it had been sent: results simply aren't available until the transaction completes.
			// https://stackoverflow.com/questions/25976231/stackexchange-redis-transaction-methods-freezes
			foreach (var action in _actions)
				_ = action(transaction);

			var committed = await transaction.ExecuteAsync();

			if (!committed)
				throw new VersionConflictException($"Redis transaction failed");

			_actions.Clear();
			_conditions.Clear();
		}
	}
}
