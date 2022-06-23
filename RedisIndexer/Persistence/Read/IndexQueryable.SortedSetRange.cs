using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisIndexer.Persistence.Read
{
	abstract partial class IndexQueryable : IIndexQueryable
	{
		public class SortedSetRange : IndexQueryable
		{
			private readonly string _setKey;
			private readonly string? _valueFrom;
			private readonly string? _valueTo;

			public SortedSetRange(string setKey, string? valueFrom, string? valueTo)
			{
				_setKey = setKey;
				_valueFrom = valueFrom;
				_valueTo = valueTo;
			}

			public override async Task<string[]> FetchKeys(IDatabase db)
			{
				var redisValues = await db.SortedSetRangeByValueAsync(_setKey, _valueFrom ?? default, _valueTo ?? default);

				var keys = redisValues
					.Select(x => x.GetDocumentKey())
					.Distinct()
					.ToArray();

				return keys;
			}
		}
	}
}
