using ZarmallStore.Application.Services.Interfaces;
using ZarmallStore.Data.DTOs.OrderDto;
using ZarmallStore.Data.DTOs.Paging;
using ZarmallStore.Data.DTOs.PaymentDto;
using ZarmallStore.Data.Entities.Account;
using ZarmallStore.Data.Entities.OrderEntities;
using ZarmallStore.Data.Entities.ProductEntities;
using ZarmallStore.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace ZarmallStore.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        #region CTOR
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductVariant> _variantRepository;
        private readonly IGenericRepository<PaymentRecord> _recordRepository;
        private readonly IGenericRepository<ProductSell> _productSellRepository;

        public OrderService(IGenericRepository<User> userRepository, IGenericRepository<Order> orderRepository, IGenericRepository<OrderDetail> orderDetailRepository, IGenericRepository<Product> productRepository, IGenericRepository<ProductVariant> variantRepository, IGenericRepository<PaymentRecord> recordRepository, IGenericRepository<ProductSell> productSellRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _recordRepository = recordRepository;
            _productSellRepository = productSellRepository;
        }

        public async ValueTask DisposeAsync()
        {
            await _userRepository.DisposeAsync();
            await _orderDetailRepository.DisposeAsync();
            await _orderRepository.DisposeAsync();
            await _productRepository.DisposeAsync();
            await _variantRepository.DisposeAsync();
            await _recordRepository.DisposeAsync();
            await _productSellRepository.DisposeAsync();
        }
        #endregion

        public async Task<FilterOrderDto> FilterOrders(FilterOrderDto filter)
        {
            #region Query
            var query = _orderRepository.GetQuery()
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderState != OrderState.Open)
                .OrderByDescending(d => d.CreateDate)
                .AsQueryable();
            #endregion

            #region Switch
            switch (filter.FilterOrderState)
            {
                case FilterOrderState.All:
                    break;
                case FilterOrderState.Open:
                    query = query.Where(d => d.OrderState == OrderState.Open);
                    break;
                case FilterOrderState.Paid:
                    query = query.Where(d => d.OrderState == OrderState.Paid);
                    break;
                case FilterOrderState.Sent:
                    query = query.Where(d => d.OrderState == OrderState.Sent);
                    break;
                case FilterOrderState.Canceled:
                    query = query.Where(d => d.OrderState == OrderState.Canceled);
                    break;
                case FilterOrderState.Returned:
                    query = query.Where(d => d.OrderState == OrderState.Returned);
                    break;
                case FilterOrderState.Pending:
                    query = query.Where(d => d.OrderState == OrderState.Pending);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region Filter

            #region String
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                query = query.Where(p => EF.Functions.Like(p.UserName, $"%{filter.UserName}%"));
            }

            if (!string.IsNullOrEmpty(filter.Description))
            {
                query = query.Where(p => EF.Functions.Like(p.Description, $"%{filter.Description}%"));
            }

            if (!string.IsNullOrEmpty(filter.TraceCode))
            {
                query = query.Where(p => EF.Functions.Like(p.PostTraceCode, $"%{filter.TraceCode}%"));
            }

            if (filter.PaymentRecordId is > 0)
            {
                query = query.Where(o => o.PaymentRecordId == filter.PaymentRecordId);
            }

            if (!string.IsNullOrEmpty(filter.DestinationCity))
            {
                query = query.Where(p => EF.Functions.Like(p.DestinationCity, $"%{filter.DestinationCity}%"));
            }
            #endregion

            #region Price
            if (filter.MinimumPrice is > 0)
            {
                query = query.Where(d => d.TotalPrice > filter.MinimumPrice);
            }
            #endregion

            #region Specification Ids
            if (filter.UserId is > 0)
            {
                query = query.Where(d => d.UserId == filter.UserId);
            }
            #endregion

            #endregion

            #region Paging
            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();
            #endregion

            return filter.SetData(allEntities).SetPaging(pager);
        }

        public async Task<OrderDetailDto> OrderDetail(long orderId)
        {
            var data = await _orderRepository.GetEntityById(orderId);
            return new OrderDetailDto
            {
                Address = data.Address,
                CreateDate = data.CreateDate,
                Description = data.Description,
                Id = data.Id,
                OrderState = data.OrderState,
                UserName = data.UserName,
                TotalPrice = data.TotalPrice,
                UserId = data.UserId,
                PostCode = data.PostCode,
                PostTraceCode = data.PostTraceCode,
                BankTraceCode = data.BankTraceCode,
                LastUpdateDate = data.LastUpdateDate,
                DestinationCity = data.DestinationCity,
                User = await _userRepository.GetEntityById(data.UserId),
                OrderDetails = await _orderDetailRepository.GetQuery()
                    .Include(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
                    .Where(d => d.OrderId == orderId).ToListAsync(),
                PaymentRecord = await _recordRepository.GetQuery().FirstAsync(r => r.InvoiceId == orderId)
            };
        }

        public async Task<Order> GetOrderById(long orderId)
        {
            return await _orderRepository.GetEntityById(orderId);
        }

        public async Task<int> GetOrderTotalPrice(long orderId)
        {
            var detail = await _orderDetailRepository.GetQuery()
                .Include(od => od.ProductVariant)
                .ThenInclude(v => v.Product)
                .Where(d => d.OrderId == orderId)
                .ToListAsync();
            var totalPrice = detail.Select(item => (item.ProductVariant.Product.BasePrice + item.ProductVariant.Price) * item.Count)
                .Aggregate(0, (current, price) => current + price);

            return totalPrice;
        }

        public async Task<int> UpdateOrderDetailPrices(long orderId)
        {
            var order = await _orderRepository.GetQuery()
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ProductVariant)
                .FirstAsync(o => o.Id == orderId);

            foreach (var item in order.OrderDetails)
            {
                var product = await _productRepository.GetEntityById(item.ProductVariant.ProductId);
                var variant = await _variantRepository.GetEntityById(item.ProductVariantId);

                item.TotalPrice = (product.BasePrice + item.ProductVariant.Price) * item.Count;
                item.ProductPrice = product.BasePrice;
                item.VariantPrice = variant.Price;
                _orderDetailRepository.EditEntity(item);
            }

            await _orderDetailRepository.SaveAsync();

            return order.OrderDetails.Select(item => (item.ProductVariant.Product.BasePrice + item.ProductVariant.Price) * item.Count).Aggregate(0, (current, price) => current + price);
        }

        public async Task<OpenOrderDto?> UserOpenOrderDetail(long userId)
        {
            var order = await _orderRepository.GetQuery()
                .FirstOrDefaultAsync(d => d.UserId == userId && d.OrderState == OrderState.Open);
            if (order == null) return null;

            return new OpenOrderDto
            {
                User = await _userRepository.GetEntityById(userId),
                Order = order,
                OrderDetails = await _orderDetailRepository.GetQuery()
                    .Include(d => d.ProductVariant)
                    .ThenInclude(v => v.Product)
                    .Where(d => d.OrderId == order.Id).ToListAsync()
            };
        }

        public async Task<Order?> GetUserOpenOrder(long userId)
        {
            return await _orderRepository.GetQuery()
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderState == OrderState.Open);
        }

        public async Task<bool> CheckOrderDetailExist(long orderId, long variantId)
        {
            return await _orderRepository.GetQuery().Include(o => o.OrderDetails)
                .Where(d => d.Id == orderId)
                .AnyAsync(o => o.OrderDetails.Any(d => d.ProductVariantId == variantId));
        }

        public async Task<ProcessOrderDto> GetProcessOrder(long orderId)
        {
            var data = await _orderRepository.GetEntityById(orderId);
            return new ProcessOrderDto
            {
                OrderState = data.OrderState,
                PostTraceCode = data.PostTraceCode,
                OrderId = data.Id
            };
        }

        public async Task<long> AddOrderForUser(long userId)
        {
            var order = await _orderRepository.GetQuery()
                .FirstOrDefaultAsync(o => o.UserId == userId && o.OrderState == OrderState.Open);
            if (order != null) return order.Id;

            var newOrder = new Order
            {
                UserId = userId,
                OrderState = OrderState.Open,
                TotalPrice = 0,
            };

            await _orderRepository.AddEntity(newOrder);
            await _orderRepository.SaveAsync();
            return newOrder.Id;
        }

        public async Task AddProductToOrder(SubmitOrderDetailDto dto)
        {
            var orderId = await AddOrderForUser(dto.UserId);
            var existVariant = await CheckOrderDetailExist(orderId , dto.ProductVariantId);

            if (existVariant)
            {
                var detail = await _orderDetailRepository.GetQuery()
                    .FirstAsync(d => d.ProductVariantId == dto.ProductVariantId && d.OrderId == orderId);
                detail.Count += dto.Count;
                _orderDetailRepository.EditEntity(detail);
                await _orderDetailRepository.SaveAsync();
            }
            else
            {
                var variant = await _variantRepository.GetEntityById(dto.ProductVariantId);
                var product = await _productRepository.GetEntityById(variant.ProductId);
                var orderDetail = new OrderDetail
                {
                    Count = dto.Count,
                    OrderId = orderId,
                    TotalPrice = (variant.Price + product.BasePrice) * dto.Count,
                    VariantPrice = variant.Price,
                    ProductPrice = product.BasePrice,
                    ProductVariantId = dto.ProductVariantId,
                };
                await _orderDetailRepository.AddEntity(orderDetail);
                await _orderRepository.SaveAsync();
            }
        }

        public async Task ChangeOrderDetailCount(long orderDetailId, int count)
        {
            var orderDetail = await _orderDetailRepository.GetEntityById(orderDetailId);

            if (count == 0)
            {
               await _orderDetailRepository.DeletePermanent(orderDetail);
               await _orderDetailRepository.SaveAsync();
            }
            else
            {
                orderDetail.Count = count;
                _orderDetailRepository.EditEntity(orderDetail);
                await _orderDetailRepository.SaveAsync();
            }
        }

        public async Task RemoveOrderDetail(long orderDetailId)
        {
            var orderDetail = await _orderDetailRepository.GetEntityById(orderDetailId);

            var order = await _orderRepository.GetQuery().FirstAsync(o => o.Id == orderDetail.OrderId);

            await _orderDetailRepository.DeletePermanent(orderDetail);
            await _orderDetailRepository.SaveAsync();

            if (!await _orderDetailRepository.GetQuery().AnyAsync(d => d.OrderId == order.Id))
            {
                await _orderRepository.DeletePermanent(order);
                await _orderDetailRepository.SaveAsync();
            }
        }

        public async Task ProcessOrder(ProcessOrderDto dto)
        {
            var data = await _orderRepository.GetEntityById(dto.OrderId);
            data.OrderState = dto.OrderState;
            data.PostTraceCode = dto.PostTraceCode;

            _orderRepository.EditEntity(data);
            await _orderRepository.SaveAsync();
        }

        public async Task PayOrderPrice(PaymentVerificationResultData dto)
        {
            #region Record
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(dto.payment_time).DateTime.ToLocalTime();
            var record = new PaymentRecord
            {
                Amount = dto.amount,
                TransactionId = dto.trans_id,
                RefId = dto.ref_id,
                PaymentTime = dateTime,
                InvoiceId = long.Parse(dto.invoice_id),
                CardPan = dto.card_pan,
                BuyerId = dto.buyer_ip,
                Authority = dto.authority,
            };
            await _recordRepository.AddEntity(record);
            await _recordRepository.SaveAsync();
            #endregion

            #region Product Sell
            var order = await _orderRepository.GetQuery().Include(o => o.OrderDetails)
                .ThenInclude(d => d.ProductVariant).FirstAsync(o => o.Id == long.Parse(dto.invoice_id));
            foreach (var item in order.OrderDetails)
            {
                await _productSellRepository.AddEntity(new ProductSell
                {
                    ProductId = item.ProductVariant.ProductId,
                    OrderId = order.Id,
                    SellCount = item.Count,
                    SellDate = dateTime,
                    ProductPrice = item.ProductPrice,
                });
            }
            #endregion

            #region Update Order
            var user = await _userRepository.GetEntityById(order.UserId);
            order.Address = user.Address;
            order.PostCode = user.PostCode;
            order.DestinationCity = user.UserCity;
            order.UserName = user.FullName;
            order.OrderState = OrderState.Paid;
            order.PaymentDate = dateTime;
            order.PaymentRecordId = record.Id;
            order.BankTraceCode = dto.ref_id;

            var totalPrice = order.OrderDetails.Select(item => (item.ProductVariant.Product.BasePrice + item.ProductVariant.Price) * item.Count)
                .Aggregate(0, (current, price) => current + price);
            order.TotalPrice = totalPrice;

            _orderRepository.EditEntity(order);
            await _orderRepository.SaveAsync();
            #endregion
        }

        public async Task DeleteOrder(long orderId)
        {
            var data = await _orderRepository.GetEntityById(orderId);
            _orderRepository.DeleteEntity(data);
            await _orderRepository.SaveAsync();
        }
    }
}
