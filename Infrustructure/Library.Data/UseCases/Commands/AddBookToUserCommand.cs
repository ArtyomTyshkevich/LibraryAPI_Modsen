using MediatR;

namespace Library.Data.UseCases.Commands
{
    public class AddBookToUserCommand : IRequest<string>
    {
        public Guid BookId { get; set; }
        public long UserId { get; set; }
    }
}
