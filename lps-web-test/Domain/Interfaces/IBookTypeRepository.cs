using lps_web_test.Domain.Entities;

namespace lps_web_test.Domain.Interface
{
    public interface IBookTypeRepository
    {
        Task<List<BookType>> GetAllAsync();
        IQueryable<BookType> GetAllAsync(string? search);
        Task<BookType?> GetByIdAsync(int id);
        Task AddAsync(BookType bookType);
        Task UpdateAsync(BookType bookType);
        Task DeleteAsync(int id);
    }
}
