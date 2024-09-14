using MediatR;

namespace Library.Data.UseCases.Queries
{
    public class GetImageQuery : IRequest<byte[]>
    {
        public string ImageKey { get; set; }
    }
}