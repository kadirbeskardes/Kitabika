using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<BookDto> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return null;

            var bookDto = _mapper.Map<BookDto>(book);
            var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId);
            bookDto.CategoryName = category?.Name;

            return bookDto;
        }

        public async Task<BookDto> AddBookAsync(CreateBookDto createBookDto)
        {
            var book = _mapper.Map<Book>(createBookDto);
            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<BookDto>(book);
        }

        public async Task UpdateBookAsync(int id, UpdateBookDto updateBookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) throw new Exception("Kitap bulunamadı");

            _mapper.Map(updateBookDto, book);
            book.UpdatedDate = DateTime.Now;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) throw new Exception("Kitap bulunamadı");

            _unitOfWork.Books.Remove(book);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _unitOfWork.Books.FindAsync(b => b.CategoryId == categoryId);
            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
        {
            var books = await _unitOfWork.Books.FindAsync(b =>
                b.Title.Contains(searchTerm) ||
                b.Author.Contains(searchTerm) ||
                b.ISBN.Contains(searchTerm));

            return _mapper.Map<IEnumerable<BookDto>>(books);
        }

        public async Task<PagedResult<BookDto>> SearchBooksAdvancedAsync(BookSearchDto searchDto)
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var query = books.AsQueryable();

            // Genel arama terimi
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.ISBN.Contains(searchDto.SearchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Yazar filtresi
            if (!string.IsNullOrEmpty(searchDto.Author))
            {
                query = query.Where(b => b.Author.Contains(searchDto.Author, StringComparison.OrdinalIgnoreCase));
            }

            // Fiyat aralığı filtresi
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(b => b.Price >= searchDto.MinPrice.Value);
            }
            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= searchDto.MaxPrice.Value);
            }

            // Yayın yılı filtresi
            if (searchDto.MinYear.HasValue)
            {
                query = query.Where(b => b.PublicationYear >= searchDto.MinYear.Value);
            }
            if (searchDto.MaxYear.HasValue)
            {
                query = query.Where(b => b.PublicationYear <= searchDto.MaxYear.Value);
            }

            // Kategori filtresi
            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == searchDto.CategoryId.Value);
            }

            // Stok durumu filtresi
            if (searchDto.InStock.HasValue)
            {
                if (searchDto.InStock.Value)
                {
                    query = query.Where(b => b.Stock > 0);
                }
                else
                {
                    query = query.Where(b => b.Stock == 0);
                }
            }

            // Sıralama
            query = searchDto.SortBy?.ToLower() switch
            {
                "title" => searchDto.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                "author" => searchDto.SortDescending ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
                "price" => searchDto.SortDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                "year" => searchDto.SortDescending ? query.OrderByDescending(b => b.PublicationYear) : query.OrderBy(b => b.PublicationYear),
                _ => query.OrderBy(b => b.Title)
            };

            var totalCount = query.Count();
            var pagedBooks = query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            var bookDtos = new List<BookDto>();
            foreach (var book in pagedBooks)
            {
                var bookDto = _mapper.Map<BookDto>(book);
                var category = await _unitOfWork.Categories.GetByIdAsync(book.CategoryId);
                bookDto.CategoryName = category?.Name;
                bookDtos.Add(bookDto);
            }

            return new PagedResult<BookDto>
            {
                Items = bookDtos,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<IEnumerable<string>> GetAllAuthorsAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return books.Select(b => b.Author).Distinct().OrderBy(a => a);
        }

        public async Task<IEnumerable<int>> GetPublicationYearsAsync()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            return books.Select(b => b.PublicationYear).Distinct().OrderByDescending(y => y);
        }
    }
}
