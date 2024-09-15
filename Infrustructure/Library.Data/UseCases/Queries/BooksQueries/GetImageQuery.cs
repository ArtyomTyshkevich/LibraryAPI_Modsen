using MediatR;

namespace Library.Data.UseCases.Queries.BooksQueries
{
    public class GetImageQuery : IRequest<byte[]>
    {
        public string ImageKey { get; set; }
    }
}