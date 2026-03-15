using lps_web_test.Domain.Entities;

namespace lps_web_test.Application.Interfaces
{
    public interface IBookTypeService
    {
        Task<List<BookType>> GetAsync();
        Task<List<BookType>> GetAsync(string? search);
        Task<BookType?> GetByIdAsync(int id);
        Task CreateAsync(BookType bookType);
        Task UpdateAsync(BookType bookType);
        Task EditAsync(BookType bookType);
        Task<bool> BookTypeExists(int id);
        Task DeleteAsync(int id);
    }
}
