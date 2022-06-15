using System;
using System.Threading.Tasks;

namespace RedisIndexer.Persistence.Read
{
	public interface IIndexReader<TType>
	{
		Task<string[]> SearchKeys(Action<IQueryBuilder<TType>> filters);
		Task<string[]> SearchKeys(params IIndexQueryable[] queryables);
	}
}
