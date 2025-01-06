using Hoteleo.Domain.Model;
using Hoteleo.Utilities;
using Hoteleo.Utilities.Serialization;

namespace Hoteleo.Domain.Repositories
{
    internal interface IBookingsRepository
    {
        IReadOnlyList<Booking> GetBookingsInRange(string hotelId, string roomType, DatesRange range);
    }

    internal interface IJsonBookingsRepositoryConfiguration
    {
        string BookingsJsonFilePath { get; }
    }

    internal class JsonBookingsRepository : IBookingsRepository
    {
        private readonly List<Booking> _bookings;

        public JsonBookingsRepository(IJsonBookingsRepositoryConfiguration configuration, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            var json = fileSystem.ReadAllText(configuration.BookingsJsonFilePath);
            _bookings = jsonSerializer.Deserialize<List<Booking>>(json);
        }

        public IReadOnlyList<Booking> GetBookingsInRange(string hotelId, string roomType, DatesRange range)
        {
            return
                _bookings
                .Where(booking =>
                    booking.HotelId == hotelId &&
                    booking.RoomType == roomType &&
                    booking.Arrival <= range.End &&
                    range.Start <= booking.Departure
                )
                .ToList();
        }
    }
}
