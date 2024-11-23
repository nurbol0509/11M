using System;

public interface IUserService
{
    User Register(string username, string password);
    User Login(string username, string password);
}

public interface IProductService
{
    List<Product> GetProducts();
    Product AddProduct(Product product);
}

public interface IOrderService
{
    Order CreateOrder(int userId, List<int> productIds);
    Order GetOrderStatus(int orderId);
}

public interface IPaymentService
{
    bool ProcessPayment(int orderId, decimal amount);
}

public interface INotificationService
{
    void SendNotification(int userId, string message);
}

public class OrderService : IOrderService
{
    private readonly IProductService _productService;
    private readonly IPaymentService _paymentService;
    private readonly INotificationService _notificationService;

    public OrderService(IProductService productService, IPaymentService paymentService, INotificationService notificationService)
    {
        _productService = productService;
        _paymentService = paymentService;
        _notificationService = notificationService;
    }

    public Order CreateOrder(int userId, List<int> productIds)
    {
        var products = _productService.GetProducts().Where(p => productIds.Contains(p.Id)).ToList();
        if (!products.Any())
        {
            throw new Exception("Выбранные товары не найдены.");
        }

        var order = new Order { UserId = userId, Products = products, Status = "Created" };

        decimal totalAmount = products.Sum(p => p.Price);
        if (_paymentService.ProcessPayment(order.Id, totalAmount))
        {
            order.Status = "Paid";
            _notificationService.SendNotification(userId, "Ваш заказ успешно оплачен.");
        }
        else
        {
            order.Status = "Payment Failed";
            _notificationService.SendNotification(userId, "Платеж не прошел. Попробуйте снова.");
        }

        return order;
    }

    public Order GetOrderStatus(int orderId)
    {
        return new Order { Id = orderId, Status = "In Progress" };
    }
}

