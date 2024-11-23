using System;

public interface IHotelService
{
    IEnumerable<Hotel> SearchHotels(string location, string roomClass, decimal maxPrice);
    void AddHotel(Hotel hotel);
}

public interface IBookingService
{
    bool BookRoom(int hotelId, int roomId, DateTime checkIn, DateTime checkOut, string userName);
    IEnumerable<Booking> GetUserBookings(string userName);
}

public interface IPaymentService
{
    bool ProcessPayment(string userName, decimal amount, string paymentMethod);
}

public interface INotificationService
{
    void SendNotification(string userName, string message);
}

public interface IUserManagementService
{
    bool RegisterUser(string userName, string password);
    bool LoginUser(string userName, string password);
}

public class HotelService : IHotelService
{
    private List<Hotel> _hotels = new List<Hotel>();

    public IEnumerable<Hotel> SearchHotels(string location, string roomClass, decimal maxPrice)
    {
        return _hotels.Where(h => h.Location == location &&
                                  h.Rooms.Any(r => r.Class == roomClass && r.Price <= maxPrice));
    }

    public void AddHotel(Hotel hotel)
    {
        _hotels.Add(hotel);
    }
}

public class BookingService : IBookingService
{
    private List<Booking> _bookings = new List<Booking>();
    private IHotelService _hotelService;

    public BookingService(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    public bool BookRoom(int hotelId, int roomId, DateTime checkIn, DateTime checkOut, string userName)
    {
        var hotel = _hotelService.SearchHotels("", "", 0).FirstOrDefault(h => h.Id == hotelId);
        var room = hotel?.Rooms.FirstOrDefault(r => r.Id == roomId);

        if (room == null || _bookings.Any(b => b.RoomId == roomId &&
                                               b.HotelId == hotelId &&
                                               b.CheckIn < checkOut && b.CheckOut > checkIn))
        {
            return false;
        }

        _bookings.Add(new Booking
        {
            UserName = userName,
            HotelId = hotelId,
            RoomId = roomId,
            CheckIn = checkIn,
            CheckOut = checkOut
        });

        return true;
    }

    public IEnumerable<Booking> GetUserBookings(string userName)
    {
        return _bookings.Where(b => b.UserName == userName);
    }
}

public class NotificationService : INotificationService
{
    public void SendNotification(string userName, string message)
    {
        Console.WriteLine($"Уведомление для {userName}: {message}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        IHotelService hotelService = new HotelService();
        IBookingService bookingService = new BookingService(hotelService);
        INotificationService notificationService = new NotificationService();

        hotelService.AddHotel(new Hotel
        {
            Id = 1,
            Name = "Grand Hotel",
            Location = "Москва",
            Rooms = new List<Room>
            {
                new Room { Id = 101, Class = "Standard", Price = 5000 },
                new Room { Id = 102, Class = "Deluxe", Price = 10000 }
            }
        });

        var hotels = hotelService.SearchHotels("Москва", "Standard", 6000);
        Console.WriteLine("Доступные отели:");
        foreach (var hotel in hotels)
        {
            Console.WriteLine($"{hotel.Name}, {hotel.Location}");
        }

        bool success = bookingService.BookRoom(1, 101, DateTime.Today, DateTime.Today.AddDays(2), "Иван");
        if (success)
        {
            notificationService.SendNotification("Иван", "Ваше бронирование подтверждено!");
        }
        else
        {
            Console.WriteLine("Не удалось забронировать номер.");
        }
    }
}
