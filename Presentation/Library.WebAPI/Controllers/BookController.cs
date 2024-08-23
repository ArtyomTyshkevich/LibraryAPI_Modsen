using AutoMapper;
using Library.Data.Context;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookController : Controller
    {
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookService _bookService;
        public BookController(IMapper mapper, LibraryDbContext dbContext, IUnitOfWork unitOfWork, IBookService bookService)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
            _bookService = bookService;
        }

        [HttpGet]
        [Route("get/byId/{id}")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var book = await _unitOfWork.Books.Get(id);

            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }
            return Ok(book);
        }

        [HttpGet]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _unitOfWork.Books.Get();
            return Ok(books);
        }
        [HttpGet]
        [Route("GetBookWithPagination")]
        public async Task<IActionResult> GetBookWithPagination(int PageNum, int PageSize)
        {
            var books = await _bookService.BooksPagination(PageNum, PageSize);
            return Ok(books);
        }

        [HttpGet]
        [Route("get/byISBN/{ISBN}")]
        public async Task<IActionResult> GetBooksByISBN(string ISBN)
        {
            var book = await _dbContext.Books
                                           .Include(b => b.Author)
                                           .FirstOrDefaultAsync(a => a.ISBN == ISBN); ;
            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }
            return Ok(book);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateBook(Book book)
        {
            await _unitOfWork.Books.Create(book);
            return Ok(book);
        }

        [HttpPatch]
        [Route("Update")]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            await _unitOfWork.Books.Update(book);
            return Ok(book);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteBook(Guid Id)
        {
            Book? book = await _unitOfWork.Books.Get(Id);
            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }
            await _unitOfWork.Books.Delete(book);
            return Ok("Successfully deleted");
        }

        [HttpPatch]
        [Route("IssueBookToUser")]
        public async Task<IActionResult> IssueBookToUser(Guid bookId, string userId)
        {
            var user = await _dbContext.Users
                                 .Include(u => u.Books)
                                 .FirstOrDefaultAsync(x => x.Id == userId);
            var book = await _unitOfWork.Books.Get(bookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }
            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                return BadRequest("The book has already been issued to another user");
            }
            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user!.Books.Add(book);

            await _dbContext.SaveChangesAsync();

            return Ok("The book was issued successfully");
        }
        [HttpPatch]
        [Route("ChangeBookImage")]
        [Authorize(Policy = "ModerAndHigher")]
        public async Task<IActionResult> ChangeBookImage([FromForm] BookDTO bookDTO)
        {
            await _bookService.AddImageToBook(bookDTO);
            return Ok();
        }
    }
}
