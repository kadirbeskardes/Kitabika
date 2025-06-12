using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class LoanService : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private readonly decimal _dailyFineAmount = 0.50m;

        public LoanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IBookService bookService,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bookService = bookService;
            _userService = userService;
        }
        public async Task<LoanDto> GetLoanByIdAsync(int id)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(id);
            return _mapper.Map<LoanDto>(loan);
        }
        public async Task<IEnumerable<LoanDto>> GetAllLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetAllLoansAsync();
            var loanDtos = new List<LoanDto>();

            foreach (var loan in loans)
            {
                var loanDto = _mapper.Map<LoanDto>(loan);

                if (loan.Book != null)
                {
                    loanDto.BookTitle = loan.Book.Title;
                    loanDto.BookCoverImage = loan.Book.CoverImageUrl;
                }

                if (loan.User != null)
                {
                    loanDto.UserName = loan.User.Username;
                }

                loanDtos.Add(loanDto);
            }

            return loanDtos;
        }
        public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(int userId)
        {
            var loans = await _unitOfWork.Loans.GetActiveLoansByUserAsync(userId);
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<IEnumerable<LoanDto>> GetOverdueLoansAsync()
        {
            var loans = await _unitOfWork.Loans.GetOverdueLoansAsync();
            return _mapper.Map<IEnumerable<LoanDto>>(loans);
        }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
        {
            var book = await _bookService.GetBookByIdAsync(createLoanDto.BookId);
            if (book == null)
                throw new Exception("Kitap bulunamadı");

            if (!await _unitOfWork.Loans.IsBookAvailableAsync(createLoanDto.BookId))
                throw new Exception("Kitap zaten ödünçte");

            var user = await _userService.GetUserByIdAsync(createLoanDto.UserId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı");

            var loan = new Loan
            {
                BookId = createLoanDto.BookId,
                UserId = createLoanDto.UserId,
                LoanDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(createLoanDto.LoanDays),
                Status = "Active"
            };

            await _unitOfWork.Loans.AddAsync(loan);
            await _unitOfWork.CommitAsync();

            return await GetLoanDtoAsync(loan.Id);
        }

        public async Task<LoanDto> ReturnLoanAsync(int loanId, ReturnLoanDto returnLoanDto)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);
            if (loan == null)
                throw new Exception("Ödünç bulunamadı");

            if (loan.Status != "Active")
                throw new Exception("Ödünç kitap zaten teslim edildi");

            loan.ReturnDate = DateTime.Now;
            loan.Status = "Returned";

            if (returnLoanDto.Condition == "Damaged")
            {
                loan.FineAmount = 10.00m; 
            }
            else if (returnLoanDto.Condition == "Lost")
            {
                var book = await _bookService.GetBookByIdAsync(loan.BookId);
                loan.FineAmount = book.Price * 1.2m; 
            }
            else if (loan.DueDate < DateTime.Now)
            {
                var daysOverdue = (DateTime.Now - loan.DueDate).Days;
                loan.FineAmount = daysOverdue * _dailyFineAmount;
            }

            _unitOfWork.Loans.Update(loan);
            await _unitOfWork.CommitAsync();

            return await GetLoanDtoAsync(loan.Id);
        }

        public async Task CheckOverdueLoansAsync()
        {
            var overdueLoans = await _unitOfWork.Loans.GetOverdueLoansAsync();
            foreach (var loan in overdueLoans)
            {
                loan.Status = "Overdue";
                _unitOfWork.Loans.Update(loan);
            }
            await _unitOfWork.CommitAsync();
        }

        private async Task<LoanDto> GetLoanDtoAsync(int loanId)
        {
            var loan = await _unitOfWork.Loans.GetByIdAsync(loanId);
            var loanDto = _mapper.Map<LoanDto>(loan);

            var book = await _bookService.GetBookByIdAsync(loan.BookId);
            loanDto.BookTitle = book.Title;
            loanDto.BookCoverImage = book.CoverImageUrl;

            var user = await _userService.GetUserByIdAsync(loan.UserId);
            loanDto.UserName = user.Username;

            return loanDto;
        }

        public async Task<bool> IsBookAvailableAsync(int bookId)
        {
            var isAvailable = await _unitOfWork.Loans.IsBookAvailableAsync(bookId);
            return isAvailable;
        }
    }
}
