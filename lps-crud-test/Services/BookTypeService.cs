using lps_crud_test.Models.LpsDb;
using lps_crud_test.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace lps_crud_test.Services
{
    public class BookTypeService : IBookTypeService
    {
        private readonly LpsDbContext _context;
        private readonly IConfiguration _configuration;

        public BookTypeService(LpsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<BookType>> GetAsync()
        {
            return await _context.BookTypes.ToListAsync();
        }

        public async Task<List<BookType>> GetAsync(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return await GetAsync();

            // EF Core can't translate StringComparison overload; use case-insensitive LIKE instead
            var pattern = $"%{search}%";
            return await _context.BookTypes
                                 .Where(bt => EF.Functions.Like(bt.BookTypeName, pattern))
                                 .ToListAsync();
        }

        public async Task<BookType?> GetByIdAsync(int id)
        {
            return await _context.BookTypes.FindAsync(id);
        }

        public async Task CreateAsync(BookType bookType)
        {
            _context.BookTypes.Add(bookType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookType bookType)
        {
            _context.BookTypes.Update(bookType);
            await _context.SaveChangesAsync();
        }

        public async Task EditAsync(BookType bookType)
        {
            var existingBookType = await _context.BookTypes.FindAsync(bookType.Id);
            if (existingBookType != null)
            {
                existingBookType.BookTypeName = bookType.BookTypeName;
                existingBookType.IsActive = bookType.IsActive;
                existingBookType.UpdatedAt = DateTime.Now;
                existingBookType.UpdatedBy = bookType.UpdatedBy;
                
                _context.BookTypes.Update(existingBookType);
                await _context.SaveChangesAsync();
            }
        }   

        public async Task<bool> BookTypeExists(int id)
        {
            return await _context.BookTypes.AnyAsync(e => e.Id == id);
        } 

        public async Task DeleteAsync(int id)
        {
            var bookType = await _context.BookTypes.FindAsync(id);
            if (bookType != null)
            {
                _context.BookTypes.Remove(bookType);
                await _context.SaveChangesAsync();
            }
        }
    }
}
