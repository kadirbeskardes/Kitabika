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
    }
}
