using BookStore.Core.Entities;
using Microsoft.EntityFrameworkCore;



namespace BookStore.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
    new Category
    {
        Id = 1,
        Name = "Kurgu",
        Description = "Kurgusal kitaplar",
        CreatedDate = new DateTime(2023, 1, 1),
    },
    new Category
    {
        Id = 2,
        Name = "Bilim",
        Description = "Bilimsel kitaplar",
        CreatedDate = new DateTime(2023, 1, 1),
    },
    new Category
    {
        Id = 3,
        Name = "Tarih",
        Description = "Tarihi kitaplar",
        CreatedDate = new DateTime(2023, 1, 1)
    }, new Category
    {
        Id = 4,
        Name = "Roman",
        Description = "Roman kitaplar",
        CreatedDate = new DateTime(2023, 1, 1)
    }
);


            modelBuilder.Entity<User>().HasData(
    new User
    {
        Id = 1,
        Username = "admin",
        Email = "admin@kitabika.com",
        PasswordHash = "$2a$11$Hk6rHq.lBn6A6CyK0cSxYOu5DdW6XV2BHD0ZwFWWAELIzHymX1kQm", // "admin123"
        CreatedDate = new DateTime(2023, 1, 1),
        Role = "Admin"
    },
    new User
    {
        Id = 2,
        Username = "user",
        Email = "user@kitabika.com",
        PasswordHash = "$2a$11$XHJ5WSm9EKYIbw4Jz/07f.zwEjJXPr9eT6Gu4ZTqs2sm8D6X8eJ7e", // "user123"
        CreatedDate = new DateTime(2023, 1, 1),

        Role = "User"
    }
);


            modelBuilder.Entity<Book>().HasData(
     new Book
     {
         Id = 1,
         Title = "Simyacı",
         Author = "Paulo Coelho",
         ISBN = "9789750726439",
         Price = 8.99m,
         Stock = 120,
         CreatedDate = new DateTime(2023, 1, 1),
         Description = "Kendi kaderini arayan bir çobanın hikayesi",
         CategoryId = 1,
         CoverImageUrl = "https://i.dr.com.tr/cache/600x600-0/originals/0000000064552-1.jpg"
     },
new Book
{
    Id = 2,
    Title = "Evrenin Kısa Tarihi",
    Author = "Stephen Hawking",
    ISBN = "9789754035209",
    Price = 13.50m,
    Stock = 60,
    CreatedDate = new DateTime(2023, 1, 1),
    Description = "Evrenin doğası ve kara delikler hakkında popüler bir bilim kitabı",
    CategoryId = 2,
    CoverImageUrl = "https://i.dr.com.tr/cache/500x400-0/originals/0000000059067-1.jpg"
},
new Book
{
    Id = 3,
    Title = "İnsanlığın Yükselişi",
    Author = "Yuval Noah Harari",
    ISBN = "9786058301751",
    Price = 14.99m,
    Stock = 80,
    CreatedDate = new DateTime(2023, 1, 1),
    Description = "İnsan türünün tarihi ve gelişimi üzerine bir inceleme",
    CategoryId = 3,
    CoverImageUrl = "https://m.media-amazon.com/images/I/61iFIQRYSZL._AC_UF1000,1000_QL80_.jpg"
},
new Book
{
    Id = 4,
    Title = "Çalıkuşu",
    Author = "Reşat Nuri Güntekin",
    ISBN = "9789751047236",
    Price = 415.80m,
    Stock = 45,
    CreatedDate = new DateTime(2023, 1, 1),
    Description = "Reşat Nuri Güntekin’in Çalıkuşu romanı, idealist bir öğretmen olan Feride’nin neşeli ama mücadeleci yaşamını anlatır. Toplumun sahte değerlerine karşı dururken, Anadolu’nun sorunlarına da ışık tutar. Feride karakteriyle Türk edebiyatında ilk ideal kahraman örneğini sunan eser, yalın üslubu ve güçlü gözlemleriyle klasikleşmiştir.",
    CategoryId = 4,
    CoverImageUrl = "https://m.media-amazon.com/images/I/51CNpBIb6VL._AC_UF1000,1000_QL80_.jpg"
}

 );

        }
    }
}
