using System;
using System.Collections.Generic;

namespace BookStore.Service.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalBooks { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyOrders { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockBooks { get; set; }
        public int ActiveLoans { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<MonthlySalesDto> MonthlySales { get; set; } = new List<MonthlySalesDto>();
        public List<CategoryStatsDto> CategoryStats { get; set; } = new List<CategoryStatsDto>();
        public List<PopularBookDto> PopularBooks { get; set; } = new List<PopularBookDto>();
        public List<RecentActivityDto> RecentActivities { get; set; } = new List<RecentActivityDto>();
    }

    public class MonthlySalesDto
    {
        public string Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CategoryStatsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BookCount { get; set; }
        public decimal TotalSales { get; set; }
        public int SoldQuantity { get; set; }
    }

    public class PopularBookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Revenue { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class RecentActivityDto
    {
        public DateTime Date { get; set; }
        public string ActivityType { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public decimal? Amount { get; set; }
    }
}