using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Service.DTOs;

namespace BookStore.Web.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();
            CreateMap<Book, BookDto>();

            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, CategoryDto>();

            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<Loan, LoanDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookCoverImage, opt => opt.MapFrom(src => src.Book.CoverImageUrl))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

            CreateMap<CreateLoanDto, Loan>()
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.Now.AddDays(src.LoanDays)))
                .ForMember(dest => dest.LoanDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.FineAmount, opt => opt.Ignore()) 
                .ForMember(dest => dest.ReturnDate, opt => opt.Ignore());

            CreateMap<ReturnLoanDto, Loan>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Condition == "Lost" ? "Overdue" : "Returned"))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Coupon, CouponDto>();

            CreateMap<CreateCouponDto, Coupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UsedCount, opt => opt.MapFrom(src => 0));

            
            CreateMap<CreateOrderDto, Order>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<CreateReviewDto, Review>();
            CreateMap<UpdateReviewDto, Review>();
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserDisplayName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Username : "Deleted User"))
                .ForMember(dest => dest.BookTitle,
                    opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : "Unknown Book"));

            // Wishlist mappings
            CreateMap<Wishlist, WishlistDto>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));
            CreateMap<AddToWishlistDto, Wishlist>()
                .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => DateTime.Now));

            // Favorite mappings
            CreateMap<Favorite, FavoriteDto>()
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));
            CreateMap<AddToFavoritesDto, Favorite>()
                .ForMember(dest => dest.AddedDate, opt => opt.MapFrom(src => DateTime.Now));
        }
    }


}
