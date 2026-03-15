using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using lps_web_test.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LpsDbContext _context;

        public BookRepository(LpsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Include(b => b.BookType).ToListAsync();
        }

        public IQueryable<Book> GetAllAsync(string? search)
        {
            IQueryable<Book> query = _context.Books
                                            .Include(b => b.BookType)
                                            .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(b =>
                    EF.Functions.Like(b.BookTitle, pattern) ||
                    EF.Functions.Like(b.Author, pattern));
            }

            return query;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.Include(b => b.BookType).FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var book = await GetByIdAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync(string? search)
        {
            IQueryable<Book> query = _context.Books;
            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(b => EF.Functions.Like(b.BookTitle, pattern) || EF.Functions.Like(b.Author, pattern));
            }
            return await query.CountAsync();
        }

        public async Task<List<BookType>> GetBooksWithTypesAsync()
        {
            return await _context.BookTypes.Include(bt => bt.Books).ToListAsync();
        }
    }
}
