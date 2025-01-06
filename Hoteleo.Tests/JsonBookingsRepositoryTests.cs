using Hoteleo.Domain;
using Hoteleo.Domain.Model;
using Hoteleo.Domain.Repositories;
using Hoteleo.Utilities;
using Hoteleo.Utilities.Serialization;
using NSubstitute;

namespace Hoteleo.Tests
{
    public class JsonBookingsRepositoryTests
    {
        [Theory]
        [InlineData(1, 2, 1)]
        [InlineData(1, 3, 2)]
        [InlineData(3, 4, 3)]
        [InlineData(5, 6, 1)]
        [InlineData(10, 30, 0)]
        public void GetBookingsInRange_ShouldReturnExpectedNumberOfBookings(int fromDay, int toDay, int expectedBookingsCount)
        {
            // Arrange
            var datesRange = new DatesRange(new(2024, 12, fromDay), new(2024, 12, toDay));
            var hotelId = "TestHotel";
            var roomType = "RoomType";

            // | 01.12 | 02.12 | 03.12 | 04.12 | 05.12 | 06.12 |
            //   ---------------------
            //                   -------------
            //                           ---------------------
            var bookings = new List<Booking>
            {
                new (){ Arrival = new (2024, 12, 1), Departure = new DateTime(2024, 12, 3), HotelId = hotelId, RoomType = roomType },
                new (){ Arrival = new (2024, 12, 3), Departure = new DateTime(2024, 12, 4), HotelId = hotelId, RoomType = roomType },
                new (){ Arrival = new (2024, 12, 4), Departure = new DateTime(2024, 12, 6), HotelId = hotelId, RoomType = roomType },
            };

            var configuration = Substitute.For<IJsonBookingsRepositoryConfiguration>();
            var fileSystem = Substitute.For<IFileSystem>();
            var jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.Deserialize<List<Booking>>(Arg.Any<string>()).Returns(bookings);

            var sut = new JsonBookingsRepository(configuration, fileSystem, jsonSerializer);
            
            // Act
            var foundBookings = sut.GetBookingsInRange(hotelId, roomType, datesRange);

            // Assert
            Assert.Equal(expectedBookingsCount, foundBookings.Count);
        }
    }
}