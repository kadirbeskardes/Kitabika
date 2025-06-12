using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface ILoanRepository : IRepository<Loan>
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(int userId);
        Task<IEnumerable<Loan>> GetOverdueLoansAsync();
        Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId);
        Task<bool> IsBookAvailableAsync(int bookId);
    }
}
