using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Exceptions;
using Library.Application.Interfaces;
using Library.Data.UseCases.Commands.BooksCommands;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Library.Data.UseCases.Queries.BooksQueries.Handlers
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDTO>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetBookByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<BookDTO> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await _unitOfWork.Books.Get(request.BookId, cancellationToken);
            if (book == null)
            {
                throw new BookNotFoundException(request.BookId);
            }

            var bookDTO = _mapper.Map<BookDTO>(book);
            var command = new CreateFormFileFromBookDTOCommand { BookDTO = bookDTO };
            IFormFile? formFile = await _mediator.Send(command, cancellationToken);
            bookDTO.ImageFile = formFile;

            return bookDTO;
        }
    }
}