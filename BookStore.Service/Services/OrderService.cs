using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return null;

            var orderDto = _mapper.Map<OrderDto>(order);
            orderDto.Username = (await _unitOfWork.Users.GetByIdAsync(order.UserId))?.Username;

            var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
            orderDto.OrderItems = _mapper.Map<List<OrderItemDto>>(orderItems);

            foreach (var item in orderDto.OrderItems)
            {
                var book = await _unitOfWork.Books.GetByIdAsync(item.BookId);
                item.BookTitle = book?.Title;
            }

            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId);
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.Username = (await _unitOfWork.Users.GetByIdAsync(order.UserId))?.Username;

                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
                orderDto.OrderItems = _mapper.Map<List<OrderItemDto>>(orderItems);

                foreach (var item in orderDto.OrderItems)
                {
                    var book = await _unitOfWork.Books.GetByIdAsync(item.BookId);
                    item.BookTitle = book?.Title;
                }

                orderDtos.Add(orderDto);
            }

            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.Username = (await _unitOfWork.Users.GetByIdAsync(order.UserId))?.Username;

                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id);
                orderDto.OrderItems = _mapper.Map<List<OrderItemDto>>(orderItems);

                foreach (var item in orderDto.OrderItems)
                {
                    var book = await _unitOfWork.Books.GetByIdAsync(item.BookId);
                    item.BookTitle = book?.Title;
                }

                orderDtos.Add(orderDto);
            }

            return orderDtos;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(createOrderDto.UserId);
            if (user == null) throw new Exception("User not found");

            var order = new Order
            {
                UserId = createOrderDto.UserId,
                Status = "Pending",
                OrderDate = DateTime.Now
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in createOrderDto.OrderItems)
            {
                var book = await _unitOfWork.Books.GetByIdAsync(item.BookId);
                if (book == null) throw new Exception($"ID'si {item.BookId} olan kitap bulunamadı.");

                if (book.Stock < item.Quantity) throw new Exception($"'{book.Title}' kitabı için yeterli stok yok.");


                var orderItem = new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = book.Price
                };

                totalAmount += book.Price * item.Quantity;
                orderItems.Add(orderItem);

                book.Stock -= item.Quantity;
                _unitOfWork.Books.Update(book);
            }

            order.TotalAmount = totalAmount;
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CommitAsync();

            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
                await _unitOfWork.OrderItems.AddAsync(item);
            }

            await _unitOfWork.CommitAsync();

            return await GetOrderByIdAsync(order.Id);
        }

        public async Task UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) throw new Exception("Spiariş bulunamadı");

            order.Status = status;
            order.UpdatedDate = DateTime.Now;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CommitAsync();
        }
    }
}
