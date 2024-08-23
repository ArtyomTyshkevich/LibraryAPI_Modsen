using AutoMapper;
using Library.Data.Context;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController : Controller
    {
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IUnitOfWork _unitOfWork;
        public AuthorController(IMapper mapper, LibraryDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _libraryDbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("Get/byId/{id}")]
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
        [Authorize]
        public async Task<IActionResult> GetAuthors()
        {
            var author = await _unitOfWork.Authors.Get();

            if (author == null)
            {
                return NotFound(new { Message = "Authors not found" });
            }
            return Ok(author);
        }
        [HttpGet]
        [Route("GetAuthorsWithPagination")]
        public async Task<IActionResult> GetAuthorsWithPagination(int pageNum, int pageSize)
        {
            var authors = await _libraryDbContext.Authors
                     .Skip((pageNum - 1) * pageSize)
                     .Take(pageSize)
                     .ToListAsync();
            return Ok(authors);
        }
        [HttpPost]
        [Route("Create")]

        public async Task<IActionResult> CreateAuthor(Author author)
        {
            await _unitOfWork.Authors.Create(author);
            return Ok(author);
        }
        [HttpPatch]
        [Route("Update")]
        public async Task<IActionResult> UpdateAuthor(Author author)
        {
            await _unitOfWork.Authors.Update(author);
            return Ok(author);
        }
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            Author? author = await _unitOfWork.Authors.Get(id);
            if (author == null)
            {
                return NotFound(new { Message = "Author not found" });
            }
            await _unitOfWork.Authors.Delete(author);
            return Ok("Successfully deleted");
        }
        [HttpGet]
        [Route("GetBooksByAuthor")]
        public async Task<IActionResult> GetBooksByAuthor(Guid id)
        {
            var authorWithBooks = await _unitOfWork.Authors.Get(id);
            List<Book> books = authorWithBooks!.Books;
            return Ok(books);
        }
    }
}
