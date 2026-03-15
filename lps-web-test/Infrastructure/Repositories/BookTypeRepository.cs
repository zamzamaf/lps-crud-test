using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using lps_web_test.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Infrastructure.Repositories
{
    public class BookTypeRepository : IBookTypeRepository
    {
        private readonly LpsDbContext _context;

        public BookTypeRepository(LpsDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookType>> GetAllAsync()
        {
            return await _context.BookTypes.ToListAsync();
        }

        public IQueryable<BookType> GetAllAsync(string? search)
        {
            IQueryable<BookType> query = _context.BookTypes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(b =>
                    EF.Functions.Like(b.BookTypeName, pattern));
            }

            return query;
        }

        public async Task<BookType?> GetByIdAsync(int id)
        {
            return await _context.BookTypes.FirstOrDefaultAsync(bt => bt.Id == id);
        }

        public async Task AddAsync(BookType bookType)
        {
            _context.BookTypes.Add(bookType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookType bookType)
        {
            _context.BookTypes.Update(bookType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bookType = await GetByIdAsync(id);
            if (bookType != null)
            {
                _context.BookTypes.Remove(bookType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
