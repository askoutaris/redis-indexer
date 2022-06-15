# redis-indexer
Indexer functionality to index and search documents in Redis by many different fields (like ElasticSearch, but with much less functionality supported at the moment)

### RedisIndexer Usage
```csharp
class Program
	{
		static async Task Main(string[] args)
		{
			var multiplexer = ConnectionMultiplexer.Connect("redis.dev-sb-docker.local,allowAdmin=true,defaultDatabase=0");

			// setup our DI
			var serviceProvider = new ServiceCollection()
				.AddTransient<IDatabase>(_ => multiplexer.GetDatabase())
				.AddRedisIndexer<Person>(builder => builder
					.ConfigureMappings(mappings => mappings
						.AddValue(x => x.Id, id => id.ToString())
						.AddValue(x => x.Name, name => name)
						.AddValue(x => x.DateOfBirth, dateOfBirth => dateOfBirth.ToString("o"))
						.AddCollection(x => x.Addresses.Select(a => a.Country), country => country)
						.AddCollection(x => x.Addresses.Select(a => a.City), city => city)))
				.BuildServiceProvider();

			IIndexManager<Person> indexManager = serviceProvider.GetRequiredService<IIndexManager<Person>>();
			IDatabase db = serviceProvider.GetRequiredService<IDatabase>();

			foreach (var person in GetPeople())
				await indexManager.Index(db, person.Id.ToString(), person);

			var peopleIds = await indexManager.SearchKeys(db, filters => filters.AddExact(x => x.Name, "Bill"));
			Console.WriteLine($"Search by Name: 'Bill' PeopleIds: {string.Join(',', peopleIds)}");

			peopleIds = await indexManager.SearchKeys(db, filters => filters.AddRange(x => x.Name, "Bill", null));
			Console.WriteLine($"Search by c Name: 'Bill' PeopleIds: {string.Join(',', peopleIds)}");

			peopleIds = await indexManager.SearchKeys(db, filters => filters.AddExact(x => x.Addresses.Select(a => a.Country), "Germany"));
			Console.WriteLine($"Search by Country: 'Germany' PeopleIds: {string.Join(',', peopleIds)}");

			peopleIds = await indexManager.SearchKeys(db, filters => filters.AddExact(x => x.Addresses.Select(a => a.Country), "Greece"));
			Console.WriteLine($"Search by Country: 'Greece' PeopleIds: {string.Join(',', peopleIds)}");

			peopleIds = await indexManager.SearchKeys(db, filters => filters.AddRange(x => x.DateOfBirth, new DateTime(2001, 1, 1).ToString("o"), null));
			Console.WriteLine($"Search by minimum DateOfBirth: '2001-01-01' PeopleIds: {string.Join(',', peopleIds)}");

			Console.ReadLine();
		}

		private static IEnumerable<Person> GetPeople()
		{
			yield return new Person
			{
				Id = 1,
				Name = "Bill",
				DateOfBirth = new DateTime(2000, 1, 1),
				Addresses = new[] {
					new Address{ Country = "Germany", City = "Berlin" },
					new Address{ Country = "US", City = "New York" }
				}
			};

			yield return new Person
			{
				Id = 2,
				Name = "Bill",
				DateOfBirth = new DateTime(2005, 1, 1),
				Addresses = new[] {
					new Address{ Country = "Germany", City = "Berlin" },
				}
			};

			yield return new Person
			{
				Id = 3,
				Name = "John",
				DateOfBirth = new DateTime(2010, 1, 1),
				Addresses = new[] {
					new Address{ Country = "Greece", City = "Athens" },
				}
			};
		}
	}

	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public Address[] Addresses { get; set; }
	}

	public class Address
	{
		public string Country { get; set; }
		public string City { get; set; }
	}
```
