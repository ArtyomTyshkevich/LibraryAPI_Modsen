using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetBook(Guid id, CancellationToken cancellationToken)
        {
            BookDTO bookDTO = await _bookService.BooksByIdRedis(id, cancellationToken);
            return Ok(bookDTO);
        }

        [HttpGet]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks(CancellationToken cancellationToken)
        {
            var books = await _unitOfWork.Books.Get(cancellationToken);
            List<BookDTO> booksDTO = _mapper.Map<List<BookDTO>>(books);
            return Ok(booksDTO);
        }

        [HttpGet]
        [Route("GetBookWithPagination")]
        public async Task<IActionResult> GetBookWithPagination(int PageNum, int PageSize, CancellationToken cancellationToken)
        {
            var books = await _unitOfWork.Books.GetWithPagination(PageNum, PageSize, cancellationToken);
            List<BookDTO> booksDTO = _mapper.Map<List<BookDTO>>(books);
            return Ok(booksDTO);
        }

        [HttpGet]
        [Route("get/byISBN/{ISBN}")]
        public async Task<IActionResult> GetBookByISBNWithFile(string ISBN, CancellationToken cancellationToken)
        {
            BookDTO bookDTO = await _bookService.BooksByISBNFileSystem(ISBN, cancellationToken);
            return Ok(bookDTO);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateBook(BookDTO bookDTO, CancellationToken cancellationToken)
        {
            Book book = _mapper.Map<Book>(bookDTO);
            await _unitOfWork.Books.Create(book, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateBook(BookDTO bookDTO, CancellationToken cancellationToken)
        {
            Book book = _mapper.Map<Book>(bookDTO);
            await _unitOfWork.Books.Update(book, cancellationToken);
            return Ok();
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteBook(Guid Id, CancellationToken cancellationToken)
        {
            Book? book = await _unitOfWork.Books.Get(Id, cancellationToken);
            await _unitOfWork.Books.Delete(book!, cancellationToken);
            return Ok();
        }

        [HttpPatch]
        [Route("IssueBookToUser")]
        public async Task<IActionResult> IssueBookToUser(Guid bookId, long userId, CancellationToken cancellationToken)
        {
            var book = await _bookService.BookToUser(bookId, userId, cancellationToken);
            return Ok("Book successfully added to user");
        }

        [HttpPatch]
        [Route("ChangeBookImage")]
        public async Task<IActionResult> ChangeBookImage([FromForm] BookDTO bookDTO, CancellationToken cancellationToken)
        {
            await _bookService.AddImageToBook(bookDTO, cancellationToken);
            return Ok();
        }
    }
}