using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IGenericRepository<Product> _productsRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethodsRepo;
        //private readonly IGenericRepository<Order> _ordersRepo;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            //IGenericRepository<Product> productsRepo,
            //IGenericRepository<DeliveryMethod> deliveryMethodsRepo,
            //IGenericRepository<Order> ordersRepo
            IPaymentService paymentService
            )
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            //_productsRepo = productsRepo;
            //_deliveryMethodsRepo = deliveryMethodsRepo;
            //_ordersRepo = ordersRepo;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            // 1. Get Basket from Baskets Repo
            var basket = await _basketRepository.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket from Products Repo
            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
                foreach (var item in basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                    orderItems.Add(orderItem);
                }

            // 3. Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4. Get DeliveryMethod from DeliveryMethods Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            // 5. Create Order
            var spec = new OrderWithPaymentIntentIdSpecifications(basket.PaymentIntentId);

            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (existingOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);

                await _paymentService.CreateOrUpdatePaymentIntent(basket.Id);
            }

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

            await _unitOfWork.Repository<Order>().Add(order);

            // 6. Save To Database [TO DO]

            var result = await _unitOfWork.Complete();
            if (result <= 0)
                return null;
            return order;
        }

        public async Task<Order> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var spec = new OrderSpecifications(orderId, buyerEmail);

            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);

            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }


        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

            return deliveryMethods;
        }
    }
}
