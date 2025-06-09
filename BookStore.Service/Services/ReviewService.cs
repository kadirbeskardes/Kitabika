using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByBookAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(int userId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByUserAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto, int userId)
        {
            var review = _mapper.Map<Review>(createReviewDto);
            review.UserId = userId;

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CommitAsync();

            return await GetReviewByIdAsync(review.Id);
        }

        public async Task UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto, int userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null || review.UserId != userId)
                throw new Exception("İnceleme bulunamadı");

            _mapper.Map(updateReviewDto, review);
            review.UpdatedDate = DateTime.Now;

            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteReviewAsync(int id, int userId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null || review.UserId != userId)
                throw new Exception("İnceleme bulunamadı");

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.CommitAsync();
        }
    }
}
