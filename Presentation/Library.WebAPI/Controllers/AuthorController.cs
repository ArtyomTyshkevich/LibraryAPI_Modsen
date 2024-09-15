using AutoMapper;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Library.Application.DTOs;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("Get/byId/{id}")]
        public async Task<IActionResult> GetAuthor(Guid id, CancellationToken cancellationToken)
        {
            var author = await _unitOfWork.Authors.Get(id, cancellationToken);
            AuthorDTO authorDTO = _mapper.Map<AuthorDTO>(author);
            return Ok(authorDTO);
        }

        [HttpGet]
        [Route("GetAuthors")]
        public async Task<IActionResult> GetAuthors(CancellationToken cancellationToken)
        {
            var authors = await _unitOfWork.Authors.Get(cancellationToken);
            List<AuthorDTO> authorsDTO = _mapper.Map<List<AuthorDTO>>(authors);
            return Ok(authorsDTO);
        }

        [HttpGet]
        [Route("GetAuthorsWithPagination")]
        public async Task<IActionResult> GetAuthorsWithPagination(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            var authors = await _unitOfWork.Authors.GetWithPagination(pageNum, pageSize, cancellationToken);
            List<AuthorDTO> authorsDTO = _mapper.Map<List<AuthorDTO>>(authors);
            return Ok(authorsDTO);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAuthor(AuthorDTO authorDTO, CancellationToken cancellationToken)
        {
            Author author = _mapper.Map<Author>(authorDTO);
            await _unitOfWork.Authors.Create(author, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateAuthor(AuthorDTO authorDTO, CancellationToken cancellationToken)
        {
            Author author = _mapper.Map<Author>(authorDTO);
            await _unitOfWork.Authors.Update(author, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
        {
            Author? author = await _unitOfWork.Authors.Get(id, cancellationToken);
            await _unitOfWork.Authors.Delete(author!, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("GetBooksByAuthor")]
        public async Task<IActionResult> GetBooksByAuthor(Guid id, CancellationToken cancellationToken)
        {
            var authorWithBooks = await _unitOfWork.Authors.Get(id, cancellationToken);
            List<Book> books = authorWithBooks!.Books;
            List<BookDTO> booksDTO = _mapper.Map<List<BookDTO>>(books);
            return Ok(books);
        }
    }
}