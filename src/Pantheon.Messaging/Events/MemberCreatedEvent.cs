using Pantheon.Messaging.Abstractions.Bases;

namespace Pantheon.Messaging.Events
{
	public record MemberCreatedEvent : EventBase
	{
		public int ID { get; init; }
		public string FirstName { get; init; } = default!;
		public string LastName { get; init; } = default!;
		public DateTime? ValidFrom { get; init; }
		public DateTime? ValidUntil { get; init; }
	}
}
