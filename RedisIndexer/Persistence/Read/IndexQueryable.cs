using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIndexer.Persistence.Read
{
	public interface IIndexQueryable
	{
		Task<string[]> FetchKeys(IDatabase db);
	}

	abstract partial class IndexQueryable : IIndexQueryable
	{
		public abstract Task<string[]> FetchKeys(IDatabase db);
	}
}
