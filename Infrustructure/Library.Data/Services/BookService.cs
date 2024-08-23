using AutoMapper;
using Library.Data.Context;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Domain.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _libraryDbContext;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookService( LibraryDbContext libraryDbContext, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _libraryDbContext = libraryDbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task AddImageToBook(BookDTO BookDTO)
        {
            Book? book = await _unitOfWork.Books.Get(BookDTO.Id);
            if (BookDTO.ImageFile == null)
            {
                return;
            }
            string? solutionPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            if (book!.ImageFileName != null)
            {
                string OldImagePath = Path.Combine(solutionPath, "Library API","Infrustructure", "Library.Data", "FileStorage", "BookImages", book.ImageFileName);
                System.IO.File.Delete(OldImagePath);
            }
            string uploadFolder = Path.Combine(solutionPath, "Library API", "Infrustructure", "Library.Data", "FileStorage", "BookImages");
            var fileName = Guid.NewGuid().ToString() + "_" + BookDTO.ImageFile.FileName;
            string filepath = Path.Combine(uploadFolder, fileName);
            using (var stream = System.IO.File.Create(filepath))
            {
                BookDTO.ImageFile.CopyTo(stream);
            }
            book.ImageFileName = fileName;
            await _libraryDbContext.SaveChangesAsync();
            return;
        }
        public async Task<string> AddBookToUser(Guid bookId, long userId)
        {
            var user = await _libraryDbContext.Users
                                 .Include(u => u.Books)
                                 .FirstOrDefaultAsync(x => x.Id == userId);
            var book = await _unitOfWork.Books.Get(bookId);
            if (book == null)
            {
                return "Book not found";
            }
            if (book.StartRentDateTime != null && book.EndRentDateTime == null)
            {
                return "The book has already been issued to another user";
            }
            book.StartRentDateTime = DateTime.UtcNow;
            book.EndRentDateTime = DateTime.UtcNow.AddDays(60);
            user!.Books.Add(book);

            await _libraryDbContext.SaveChangesAsync();

            return "The book was issued successfully";

        }
        public async Task<List<Book>> BooksPagination(int pageNum, int pageSize)
        {
           var books = await _libraryDbContext.Books         
                                .Skip((pageNum - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            return books;
        }
    }
}
