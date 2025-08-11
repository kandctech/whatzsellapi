using AutoMapper;
using XYZ.WShop.Application.Constants;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos;
using XYZ.WShop.Application.Exceptions;
using XYZ.WShop.Application.Helpers;
using XYZ.WShop.Application.Interfaces.Services;
using XYZ.WShop.Domain;
using XZY.WShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace XZY.WShop.Infrastructure.Services
    {
        public class OrderService : IOrderService
        {
            private readonly ApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly IProductService _productService;

            public OrderService(
                ApplicationDbContext context,
                IMapper mapper,
                IProductService productService)
            {
                _context = context;
                _mapper = mapper;
                _productService = productService;
            }

            public async Task<ResponseModel<OrderResponse>> AddAsync(CreateOrder createOrder)
            {
                // Validate products exist and have sufficient stock
                await ValidateOrderItems(createOrder.OrderItems);

                var order = _mapper.Map<Order>(createOrder);
                order.CreatedDate = DateTime.UtcNow;

                // Calculate total amount
                order.Amount = createOrder.OrderItems.Sum(item => item.Price);

            string last10;

            // Check if string is at least 10 characters long
            if (order.CustomerPhoneNumber.Length >= 10)
            {
                last10 = order.CustomerPhoneNumber.Substring(order.CustomerPhoneNumber.Length - 10);
            }
            else
            {
                last10 = order.CustomerPhoneNumber; 
            }

            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c=> c.PhoneNumber.Contains(last10));



            if (existingCustomer == null)
            {
                var names = createOrder.CustomerName.Split(new char[] {' ',',', ';'});
                    var newCustomer = new Customer
                    {
                        Address = createOrder.CustomerAddress,
                        FirstName = names[0],
                        LastName = names.Length > 1 ? names[1] : names[0],
                        PhoneNumber = createOrder.CustomerPhoneNumber,
                        BusinessId = createOrder.BusinessId,
                    };
                    _context.Customers.Add(newCustomer);

            }

            var orderId = Guid.NewGuid();
            order.Id = orderId;
            order.PaymentDate = createOrder.PaymentDate.ToUniversalTime();
            await _context.Orders.AddAsync(order);

            var count = await _context.Orders.Where(o=> o.BusinessId == order.BusinessId).CountAsync();

            order.OrderNumber = count.ToString("D6");

            await _context.SaveChangesAsync();

            var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    string.Format(ApplicationContants.Messages.CreatedSuccessfulMessage, "Order"),
                    true);
            }

            public async Task<ResponseModel<OrderResponse>> DeleteAsync(Guid id)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    throw new EntityNotFoundException($"Order with ID {id} not found");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    string.Format(ApplicationContants.Messages.DeletedSuccessfully, "Order"),
                    true);
            }

            public async Task<ResponseModel<PagedList<OrderResponse>>> GetAllAsync(
                Guid businessId,
                int page = 1,
                int pageSize = 10,
                string? searchTerm = null)
            {
                var query = _context.Orders
                    .Where(o => o.BusinessId == businessId)
                    .OrderByDescending(q => q.CreatedDate)
                    .Include(o => o.OrderItems)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(o =>
                        o.CustomerName.Contains(searchTerm) ||
                        o.CustomerEmail.Contains(searchTerm) ||
                         o.OrderNumber.Contains(searchTerm) ||
                          o.OrderItems.Any(item=> item.ProductName.Contains(searchTerm)) ||
                       o.OrderItems.Any(item => item.ProductDescription.Contains(searchTerm)) ||
                        o.CustomerPhoneNumber.Contains(searchTerm));
                }

                var totalCount = await query.CountAsync();
                var orders = await query
                    .OrderByDescending(o => o.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
                var pagedList = new PagedList<OrderResponse>(orderResponses, totalCount, page, pageSize);

                return ResponseModel<PagedList<OrderResponse>>.CreateResponse(
                    pagedList,
                    ApplicationContants.Messages.RetrievedSuccessfully,
                    true);
            }

            public async Task<ResponseModel<OrderResponse>> GetByIdAsync(Guid id)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    throw new EntityNotFoundException($"Order with ID {id} not found");
                }

                var result = _mapper.Map<OrderResponse>(order);
                return ResponseModel<OrderResponse>.CreateResponse(
                    result,
                    ApplicationContants.Messages.RetrievedSuccessfully,
                    true);
            }

            private async Task ValidateOrderItems(List<OrderItemDto> orderItems)
            {
                foreach (var item in orderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                    {
                        throw new EntityNotFoundException($"Product with ID {item.ProductId} not found");
                    }
                    // Add additional validation logic here if needed
                }
            }
        }
    }
