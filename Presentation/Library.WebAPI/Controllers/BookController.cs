using AutoMapper;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.WebAPI.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookService _bookService;

        public BookController(IMapper mapper, IUnitOfWork unitOfWork, IBookService bookService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookService = bookService;
            
        }

        [HttpGet]
        [Route("get/byId/{id}")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            BookDTO bookDTO = await _bookService.BooksByIdRedis(id);
            return Ok(bookDTO);
        }

        [HttpGet]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _unitOfWork.Books.Get();
            List<BookDTO> booksDTO = _mapper.Map<List<BookDTO>>(books);
            return Ok(booksDTO);
        }
        [HttpGet]
        [Route("GetBookWithPagination")]
        public async Task<IActionResult> GetBookWithPagination(int PageNum, int PageSize)
        {
            var books = await _bookService.BooksPagination(PageNum, PageSize);
            List<BookDTO> booksDTO = _mapper.Map<List<BookDTO>>(books);
            return Ok(booksDTO);
        }

        [HttpGet]
        [Route("get/byISBN/{ISBN}")]
        public async Task<IActionResult> GetBookByISBNWithFile(string ISBN)
        {
            BookDTO bookDTO = await _bookService.BooksByISBNFileSystem(ISBN);
            return Ok(bookDTO);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateBook(Book book)
        {
            await _unitOfWork.Books.Create(book);
            return Ok(book);
        }

        [HttpPut]
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
            await _unitOfWork.Books.Delete(book!);
            return Ok("Successfully deleted");
        }

        [HttpPatch]
        [Route("IssueBookToUser")]
        public async Task<IActionResult> IssueBookToUser(Guid bookId, long userId)
        {
            var book =await  _bookService.BookToUser(bookId, userId);
            if (book == null)
            {
                return BadRequest("The book has already been issued to another user");
            }
            return Ok(book);

        }
        [HttpPatch]
        [Route("ChangeBookImage")]
        public async Task<IActionResult> ChangeBookImage([FromForm] BookDTO bookDTO)
        {
            await _bookService.AddImageToBook(bookDTO);
            return Ok();
        }      
    }
}
