using AutoMapper;
using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController : Controller
    {
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IMapper mapper, LibraryDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAuthor")]
        public async Task<IActionResult> GetAuthor(Guid id)
        {
            var author = await _unitOfWork.Authors.Get(id);

            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }
            return Ok(author);
        }
        [HttpGet]
        [Route("GetAuthors")]
        public async Task<IActionResult> GetAuthors()
        {
            var author = await _unitOfWork.Authors.Get();

            if (author == null)
            {
                return NotFound(new { Message = "Authors not found" });
            }
            return Ok(author);
        }
        [HttpPost]
        [Route("CreateAuthor")]
        public async Task<IActionResult> CreateAuthor(Author author)
        {
            await _unitOfWork.Authors.Create(author);
            return Ok(author);
        }
        [HttpPatch]
        [Route("UpdateAuthor")]
        public async Task<IActionResult> UpdateAuthor(Author author)
        {
            await _unitOfWork.Authors.Update(author);
            return Ok(author);
        }
        [HttpDelete]
        [Route("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(Author author)
        {
            await _unitOfWork.Authors.Delete(author);
            return Ok(author);
        }
        [HttpGet]
        [Route("GetBooksByAuthor")]
        public async Task<IActionResult> GetBooksByAuthor(Guid id)
        {
            var authorWithBooks = await _unitOfWork.Authors.Get(id);
            List<Book> books = authorWithBooks.Books;
            return Ok(books);
        }

    }
}
