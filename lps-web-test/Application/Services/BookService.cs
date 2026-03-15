using lps_web_test.Application.Interfaces;
using lps_web_test.Domain.Entities;
using lps_web_test.Domain.Interface;
using lps_web_test.Helpers;
using Microsoft.EntityFrameworkCore;

namespace lps_web_test.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<List<Book>> GetAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<List<Book>> GetAsync(string? search)
        {
            return await _bookRepository.GetAllAsync(search).ToListAsync();
        }

        public Task<PaginatedList<Book>> GetPagedAsync(string? search, int pageIndex, int pageSize)
        {
            var books = _bookRepository.GetAllAsync(search);
            return PaginatedList<Book>.CreateAsync(books, pageIndex, pageSize);
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Book book)
        {
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task EditAsync(Book book)
        {
            await _bookRepository.UpdateAsync(book);
        }

        public async Task<bool> BookExists(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            return book != null;
        }

        public async Task DeleteAsync(int id)
        {
            await _bookRepository.DeleteAsync(id);
        }

        public async Task<List<BookType>> GetBooksWithTypesAsync()
        {
            return await _bookRepository.GetBooksWithTypesAsync();
        }
    }
}
