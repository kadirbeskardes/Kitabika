using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data.Repositories
{
    public class LoanRepository : Repository<Loan>, ILoanRepository
    {
        public LoanRepository(BookStoreContext context) : base(context) { }
        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            return await _entities
                .Include(l => l.Book)  
                .Include(l => l.User) 
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansByUserAsync(int userId)
        {
            return await _entities
                .Where(l => l.UserId == userId && l.Status == "Active")
                .Include(l => l.Book)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
        {
            return await _entities
                .Where(l => l.Status == "Active" && l.DueDate < DateTime.Now)
                .Include(l => l.Book)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Loan>> GetLoansByBookAsync(int bookId)
        {
            return await _entities
                .Where(l => l.BookId == bookId)
                .Include(l => l.User)
                .ToListAsync();
        }

        public async Task<bool> IsBookAvailableAsync(int bookId)
        {
            return !await _entities
                .AnyAsync(l => l.BookId == bookId && l.Status == "Active");
        }
    }
}
