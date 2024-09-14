
namespace Library.Application.Exceptions
{
    public class RefreshTokenInvalidException : Exception
    {
        public RefreshTokenInvalidException()
            : base("The provided refresh token is invalid.") { }

        public RefreshTokenInvalidException(string message)
            : base(message) { }
    }
}
