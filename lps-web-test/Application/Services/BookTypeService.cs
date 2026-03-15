using lps_web_test.Application.Interfaces;
using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Application.Services
{
    public class BookTypeService : IBookTypeService
    {
        private readonly IBookTypeRepository _bookTypeRepository;

        public BookTypeService(IBookTypeRepository bookTypeRepository)
        {
            _bookTypeRepository = bookTypeRepository;
        }

        public async Task<List<BookType>> GetAsync()
        {
            return await _bookTypeRepository.GetAllAsync();
        }

        public async Task<List<BookType>> GetAsync(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return await GetAsync();

            return await _bookTypeRepository.GetAllAsync(search).ToListAsync();
        }

        public async Task<BookType?> GetByIdAsync(int id)
        {
            return await _bookTypeRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(BookType bookType)
        {
            await _bookTypeRepository.AddAsync(bookType);
        }

        public async Task UpdateAsync(BookType bookType)
        {
            await _bookTypeRepository.UpdateAsync(bookType);
        }

        public async Task EditAsync(BookType bookType)
        {
            await _bookTypeRepository.UpdateAsync(bookType);
        }

        public async Task<bool> BookTypeExists(int id)
        {
            var bookType = await _bookTypeRepository.GetByIdAsync(id);
            return bookType != null;
        }

        public async Task DeleteAsync(int id)
        {
            await _bookTypeRepository.DeleteAsync(id);
        }
    }
}
