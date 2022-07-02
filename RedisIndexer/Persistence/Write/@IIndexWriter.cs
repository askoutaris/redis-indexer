using System.Threading.Tasks;

namespace RedisIndexer.Persistence.Write
{
	public interface IIndexWriter<TType>
	{
		Task Write(string documentKey, TType obj);
		Task Remove(string documentKey);
		Task ConcurrentWrite(string documentKey, string concurrencyToken, TType obj);
	}
}
