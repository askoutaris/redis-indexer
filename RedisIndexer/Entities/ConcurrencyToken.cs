namespace RedisIndexer.Entities
{
	struct ConcurrencyToken
	{
		public string Value { get; }

		public ConcurrencyToken(string value)
		{
			Value = value;
		}

		public static implicit operator string?(ConcurrencyToken key) => key.Value;
		public static implicit operator ConcurrencyToken(string? value) => value is not null ? new ConcurrencyToken(value) : null;
	}
}
