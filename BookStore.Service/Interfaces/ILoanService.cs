using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDto>> GetUserLoansAsync(int userId);
        Task<bool> IsBookAvailableAsync(int bookId);
        Task<IEnumerable<LoanDto>> GetAllLoansAsync();

        Task<IEnumerable<LoanDto>> GetOverdueLoansAsync();
        Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
        Task<LoanDto> ReturnLoanAsync(int loanId, ReturnLoanDto returnLoanDto);
        Task<LoanDto> GetLoanByIdAsync(int id);
        Task CheckOverdueLoansAsync();
    }
}
