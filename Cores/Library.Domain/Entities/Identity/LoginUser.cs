namespace Library.Domain.Entities
{
    public class LoginUser
    {
        public string Mail { get; set; } = "";
        public string Password { get; set; } = "";
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? MiddleName { get; set; }
    }
}
