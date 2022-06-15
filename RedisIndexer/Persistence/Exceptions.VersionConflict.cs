using System;
using System.Runtime.Serialization;

namespace RedisIndexer.Persistence
{
	[Serializable]
	class VersionConflictException : Exception
	{
		public VersionConflictException() { }
		public VersionConflictException(string message) : base(message) { }
		public VersionConflictException(string message, Exception inner) : base(message, inner) { }
		protected VersionConflictException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}
}
