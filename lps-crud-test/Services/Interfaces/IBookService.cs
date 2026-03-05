using lps_crud_test.Helpers;
using lps_crud_test.Models.LpsDb;

namespace lps_crud_test.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAsync();
        /// <summary>Fetch all or filter by search term in title or author.</summary>
        Task<List<Book>> GetAsync(string? search);
        /// <summary>Return paged list optionally filtered.</summary>
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
