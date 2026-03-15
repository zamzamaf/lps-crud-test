using lps_web_test.Domain.Entities;

namespace lps_web_test.Domain.Interface
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync();
        IQueryable<Book> GetAllAsync(string? search);
        Task<Book?> GetByIdAsync(int id);
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<int> GetCountAsync(string? search);
        Task<List<BookType>> GetBooksWithTypesAsync();
    }
}
