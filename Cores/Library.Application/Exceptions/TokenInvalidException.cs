
namespace Library.Application.Exceptions
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException()
            : base("The provided token is invalid.") { }

        public TokenInvalidException(string message)
            : base(message) { }
    }
}
