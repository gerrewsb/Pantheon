namespace Pantheon.Mediator.Models
{
	public class AuthorizationResult
	{
        public bool IsAuthorized { get; }
        public string? FailureMessage { get; set; }

        private AuthorizationResult(bool isAuthorized, string? failureMessage = null)
        {
            IsAuthorized = isAuthorized;
            FailureMessage = failureMessage;
        }

        public static AuthorizationResult Fail()
            => new(false);

        public static AuthorizationResult Fail(string failureMessage)
            => new(false, failureMessage);

        public static AuthorizationResult Succeed()
            => new(true);
    }
}
