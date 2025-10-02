using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> AddBookAsync(CreateBookDto createBookDto);
        Task UpdateBookAsync(int id, UpdateBookDto updateBookDto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<PagedResult<BookDto>> SearchBooksAdvancedAsync(BookSearchDto searchDto);
        Task<IEnumerable<string>> GetAllAuthorsAsync();
        Task<IEnumerable<int>> GetPublicationYearsAsync();
    }
}
