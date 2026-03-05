using lps_crud_test.Models.LpsDb;
using lps_crud_test.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using lps_crud_test.Helpers;

namespace lps_crud_test.Services
{
    public class BookServices : IBookService
    {
        private readonly LpsDbContext _context;
        private readonly IConfiguration _configuration;

        public BookServices(LpsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<Book>> GetAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<List<Book>> GetAsync(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return await GetAsync();

            var pattern = $"%{search}%";
            return await _context.Books
                                 .Include(b => b.BookType)
                                 .Where(b => EF.Functions.Like(b.BookTitle, pattern) || EF.Functions.Like(b.Author, pattern))
                                 .ToListAsync();
        }

        public async Task<PaginatedList<Book>> GetPagedAsync(string? search, int pageIndex, int pageSize)
        {
            IQueryable<Book> query = _context.Books.Include(b => b.BookType);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(b => EF.Functions.Like(b.BookTitle, pattern) || EF.Functions.Like(b.Author, pattern));
            }
            return await PaginatedList<Book>.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.BookType).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task CreateAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook != null)
            {
                existingBook.BookTitle = book.BookTitle;
                existingBook.Author = book.Author;
                existingBook.BookTypeId = book.BookTypeId;
                existingBook.ReleaseDate = book.ReleaseDate;
                existingBook.NumberOfPages = book.NumberOfPages;
                existingBook.IsActive = book.IsActive;
                existingBook.UpdatedAt = DateTime.Now;
                existingBook.UpdatedBy = book.UpdatedBy;
                
                _context.Books.Update(existingBook);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(e => e.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<BookType>> GetBooksWithTypesAsync()
        {
            return await _context.BookTypes.Include(bt => bt.Books).ToListAsync();
        }
    }
}
