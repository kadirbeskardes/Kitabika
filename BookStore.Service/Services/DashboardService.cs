using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Enums;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            return await GetDashboardStatsAsync(null, null);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(DateTime? startDate, DateTime? endDate)
        {
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var firstDayOfYear = new DateTime(currentDate.Year, 1, 1);

            // Temel istatistikler
            var allBooks = await _unitOfWork.Books.GetAllAsync();
            var totalBooks = allBooks.Count();
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            var totalCategories = allCategories.Count();
            var allUsers = await _unitOfWork.Users.GetAllAsync();
            var totalUsers = allUsers.Count();
            var allOrders = await _unitOfWork.Orders.GetAllAsync();
            var totalOrders = allOrders.Count();

            // Gelir hesaplamaları
            var completedOrders = allOrders.Where(o => o.Status == OrderStatus.Completed).ToList();
            var totalRevenue = completedOrders.Sum(o => o.TotalAmount);
            var monthlyOrders = completedOrders.Where(o => o.OrderDate >= firstDayOfMonth).ToList();
            var monthlyRevenue = monthlyOrders.Sum(o => o.TotalAmount);

            // Sipariş durumları
            var pendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending);

            // Stok bilgileri
            var lowStockBooks = allBooks.Count(b => b.Stock < 10);

            // Ödünç kitap sayısı
            var allLoans = await _unitOfWork.Loans.GetAllAsync();
            var activeLoans = allLoans.Count(l => l.ReturnDate == null);

            // Değerlendirme ortalaması
            var allReviews = await _unitOfWork.Reviews.GetAllAsync();
            var averageRating = allReviews.Any() ? allReviews.Average(r => r.Rating) : 0;
            var totalReviews = allReviews.Count();

            // Aylık satış verileri (son 12 ay)
            var monthlySales = await GetMonthlySalesData();

            // Kategori istatistikleri
            var categoryStats = await GetCategoryStatsData();

            // Popüler kitaplar
            var popularBooks = await GetPopularBooksData();

            // Son aktiviteler
            var recentActivities = await GetRecentActivitiesData();

            return new DashboardStatsDto
            {
                TotalBooks = totalBooks,
                TotalCategories = totalCategories,
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                MonthlyOrders = monthlyOrders.Count,
                PendingOrders = pendingOrders,
                LowStockBooks = lowStockBooks,
                ActiveLoans = activeLoans,
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = totalReviews,
                MonthlySales = monthlySales,
                CategoryStats = categoryStats,
                PopularBooks = popularBooks,
                RecentActivities = recentActivities
            };
        }

        private async Task<List<MonthlySalesDto>> GetMonthlySalesData()
        {
            var currentDate = DateTime.Now;
            var twelveMonthsAgo = currentDate.AddMonths(-11);
            var firstDayOfTwelveMonthsAgo = new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1);

            var orders = await _unitOfWork.Orders.GetAllAsync();
            var completedOrders = orders.Where(o => 
                o.Status == OrderStatus.Completed && 
                o.OrderDate >= firstDayOfTwelveMonthsAgo).ToList();

            var monthlySales = completedOrders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new MonthlySalesDto
                {
                    Year = g.Key.Year,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy", System.Globalization.CultureInfo.InvariantCulture),
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => DateTime.ParseExact(x.Month, "MMM yyyy", System.Globalization.CultureInfo.InvariantCulture).Month)
                .ToList();

            return monthlySales;
        }

        private async Task<List<CategoryStatsDto>> GetCategoryStatsData()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var books = await _unitOfWork.Books.GetAllAsync();
            var orderItems = await _unitOfWork.OrderItems.GetAllAsync();

            var categoryStats = categories.Select(c =>
            {
                var categoryBooks = books.Where(b => b.CategoryId == c.Id).ToList();
                var categoryOrderItems = orderItems.Where(oi => 
                    categoryBooks.Any(cb => cb.Id == oi.BookId)).ToList();

                return new CategoryStatsDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    BookCount = categoryBooks.Count,
                    SoldQuantity = categoryOrderItems.Sum(oi => oi.Quantity),
                    TotalSales = categoryOrderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
                };
            }).OrderByDescending(cs => cs.TotalSales).ToList();

            return categoryStats;
        }

        private async Task<List<PopularBookDto>> GetPopularBooksData()
        {
            var books = await _unitOfWork.Books.GetAllAsync();
            var orderItems = await _unitOfWork.OrderItems.GetAllAsync();
            var reviews = await _unitOfWork.Reviews.GetAllAsync();

            var popularBooks = books
                .Select(b =>
                {
                    var bookOrderItems = orderItems.Where(oi => oi.BookId == b.Id).ToList();
                    var bookReviews = reviews.Where(r => r.BookId == b.Id).ToList();

                    return new PopularBookDto
                    {
                        BookId = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        SoldQuantity = bookOrderItems.Sum(oi => oi.Quantity),
                        Revenue = bookOrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                        AverageRating = bookReviews.Any() ? Math.Round(bookReviews.Average(r => r.Rating), 2) : 0,
                        ReviewCount = bookReviews.Count
                    };
                })
                .Where(pb => pb.SoldQuantity > 0)
                .OrderByDescending(pb => pb.SoldQuantity)
                .Take(10)
                .ToList();

            return popularBooks;
        }

        private async Task<List<RecentActivityDto>> GetRecentActivitiesData()
        {
            var activities = new List<RecentActivityDto>();

            // Son siparişler
            var recentOrders = await _unitOfWork.Orders.GetAllAsync();
            var lastOrders = recentOrders.OrderByDescending(o => o.OrderDate).Take(5);

            foreach (var order in lastOrders)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(order.UserId);
                activities.Add(new RecentActivityDto
                {
                    Date = order.OrderDate,
                    ActivityType = "Sipariş",
                    Description = $"Yeni sipariş oluşturuldu - #{order.Id}",
                    UserName = user?.Username ?? "Bilinmeyen",
                    Amount = order.TotalAmount
                });
            }

            // Son incelemeler
            var recentReviews = await _unitOfWork.Reviews.GetAllAsync();
            var lastReviews = recentReviews.OrderByDescending(r => r.CreatedDate).Take(5);

            foreach (var review in lastReviews)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(review.UserId);
                var book = await _unitOfWork.Books.GetByIdAsync(review.BookId);
                
                activities.Add(new RecentActivityDto
                {
                    Date = review.CreatedDate,
                    ActivityType = "İnceleme",
                    Description = $"{book?.Title} kitabına {review.Rating} yıldız verildi",
                    UserName = user?.Username ?? "Bilinmeyen"
                });
            }

            return activities.OrderByDescending(a => a.Date).Take(10).ToList();
        }
    }
}