using lps_crud_test.Models.LpsDb;

namespace lps_crud_test.Services.Interfaces
{
    public interface IBookTypeService
    {
        Task<List<BookType>> GetAsync();
        /// <summary>
        /// Return all book types or filter by a search term (case-insensitive, contained in the name).
        /// </summary>
        Task<List<BookType>> GetAsync(string? search);
        Task<BookType?> GetByIdAsync(int id);
        Task CreateAsync(BookType bookType);
        Task UpdateAsync(BookType bookType);
        Task EditAsync(BookType bookType);
        Task<bool> BookTypeExists(int id);
        Task DeleteAsync(int id);
    }
}
