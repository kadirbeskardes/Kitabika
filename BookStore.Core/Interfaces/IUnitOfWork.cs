using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Book> Books { get; }
        IRepository<Category> Categories { get; }
        IRepository<User> Users { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IReviewRepository Reviews { get; }
        ICouponRepository Coupons { get; }
        ILoanRepository Loans { get; }
        Task<int> CommitAsync();
    }
}
