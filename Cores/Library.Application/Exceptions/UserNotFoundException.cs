
namespace Library.Application.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email)
            : base($"User with email {email} not found.") { }

        public UserNotFoundException()
            : base("User not found.") { }
    }
}
