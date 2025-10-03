using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Data.Repositories;

namespace BookStore.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookStoreContext _context;

        public UnitOfWork(BookStoreContext context)
        {
            _context = context;
            Books = new Repository<Book>(_context);
            Categories = new Repository<Category>(_context);
            Users = new Repository<User>(_context);
            Orders = new Repository<Order>(_context);
            OrderItems = new Repository<OrderItem>(_context);
            Reviews = new ReviewRepository(_context);
            Coupons= new CouponRepository(_context);
            Loans= new LoanRepository(_context);
            Wishlists = new WishlistRepository(_context);
            Favorites = new FavoriteRepository(_context);
        }

        public IRepository<Book> Books { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<User> Users { get; private set; }
        public IRepository<Order> Orders { get; private set; }
        public IRepository<OrderItem> OrderItems { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public ICouponRepository Coupons { get; private set; }
        public ILoanRepository Loans { get; private set; }
        public IWishlistRepository Wishlists { get; private set; }
        public IFavoriteRepository Favorites { get; private set; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
