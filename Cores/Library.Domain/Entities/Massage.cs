

namespace Library.Domain.Entities
{
    public class Massage
    {
        public Guid Id { get; set; }
        public string Desription { get; set; } = "";
        public DateTime DepartureTime { get; set; }
    }
}
