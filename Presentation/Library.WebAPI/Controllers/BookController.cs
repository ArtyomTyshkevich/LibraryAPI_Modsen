using AutoMapper;
using Library.Data.Context;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
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
        public BookController(IMapper mapper, LibraryDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("getBook/byId/{id}")]
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
        public async Task<IActionResult> GetBooks(Guid id)
        {
            var books = await _unitOfWork.Books.Get();
            return Ok(books);
        }
        [HttpGet]
        [Route("getBook/byISBN/{ISBN}")]
        public async Task<IActionResult> GetBooksByISBN(string ISBN)
        {
            var book =  await _dbContext.Books
                                           .Include(b=>b.Author)
                                           .FirstOrDefaultAsync(a => a.ISBN == ISBN);;
            if (book == null)
            {
                return NotFound(new { Message = "Book not found" });
            }
            return Ok(book);
        }
        [HttpPost]
        [Route("CreateBook")]
        public async Task<IActionResult> CreateBook(Book book)
        {
            await _unitOfWork.Books.Create(book);
            return Ok(book);
        }
        [HttpPatch]
        [Route("UpdateBook")]
        public async Task<IActionResult> UpdateBook(Book book)
        {
            await _unitOfWork.Books.Update(book);
            return Ok(book);
        }
        [HttpPatch]
        [Route("DeleteBook")]
        public async Task<IActionResult> DeleteBook(Book book)
        {
            await _unitOfWork.Books.Delete(book);
            return Ok(book);
        }
        [HttpPatch]
        [Route("IssueBookToUser")]
        public async Task<IActionResult> IssueBookToUser(Guid bookId, long userId)
        {
           var user = await _dbContext.Users
                                .Include(u => u.Books)
                                .FirstOrDefaultAsync(x => x.Id == userId);
            var book = await _unitOfWork.Books.Get(bookId);
            if (book == null)
            {
                return NotFound("Книга не найдена");
            }
            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                return BadRequest("Книга уже выдана другому пользователю");
            }
            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user.Books.Add(book);

            await _dbContext.SaveChangesAsync();

            return Ok("Книга успешно выдана");
        }
    }
}
