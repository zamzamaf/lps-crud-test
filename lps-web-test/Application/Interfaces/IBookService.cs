using lps_web_test.Domain.Entities;
using lps_web_test.Helpers;

namespace lps_web_test.Application.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAsync();
        Task<List<Book>> GetAsync(string? search);
        Task<PaginatedList<Book>> GetPagedAsync(string? search, int pageIndex, int pageSize);
        Task<Book?> GetByIdAsync(int id);
        Task CreateAsync(Book book);
        Task UpdateAsync(Book book);
        Task EditAsync(Book book);
        Task<bool> BookExists(int id);
        Task DeleteAsync(int id);
        Task<List<BookType>> GetBooksWithTypesAsync();
    }
}
