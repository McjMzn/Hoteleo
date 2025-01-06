using Hoteleo.Domain;
using Hoteleo.Domain.Repositories;
using Hoteleo.Domain.Services;
using NSubstitute;

namespace Hoteleo.Tests
{
    public class RoomFindServiceTests
    {
        [Theory]
        [InlineData(10, 10, 0)]
        [InlineData(10, 3, 7)]
        [InlineData(10, 15, -5)]
        public void GetNumberOfAvailableRoomsInRange_ShouldReturnProperNumberOfRoomsAvailable(int allRoomsCount, int bookedRoomsCount, int expectedAvailableRoomsCount)
        {
            // Arrange
            var hotelsRepository = Substitute.For<IHotelsRepository>();
            hotelsRepository.GetRoomsCount(Arg.Any<string>(), Arg.Any<string>()).Returns(allRoomsCount);

            var bookingsRepository = Substitute.For<IBookingsRepository>();

            var bookingAnalyzisService = Substitute.For<IBookingsAnalyzisService>();
            bookingAnalyzisService.CountRequiredRooms(Arg.Any<IEnumerable<BookingEvent>>()).Returns(bookedRoomsCount);

            var sut = new RoomFindingService(hotelsRepository, bookingsRepository, bookingAnalyzisService);

            // Act
            var result = sut.GetNumberOfAvailableRoomsInRange("HotelId", "RoomType", new DatesRange(2024, 12, 1, 2024, 12, 31));

            // Assert
            Assert.Equal(expectedAvailableRoomsCount, result);
        }

        [Fact]
        public void GetNumberOfAvailableRoomsInRange_ShouldCheckTheRoomsOfTheGivenTypeInTheGivenHotel()
        {
            // Arrange
            const string hotelId = "HotelId";
            const string roomType = "RoomType";

            var hotelsRepository = Substitute.For<IHotelsRepository>();
            var bookingsRepository = Substitute.For<IBookingsRepository>();
            var bookingAnalyzisService = Substitute.For<IBookingsAnalyzisService>();

            var sut = new RoomFindingService(hotelsRepository, bookingsRepository, bookingAnalyzisService);

            // Act
            var result = sut.GetNumberOfAvailableRoomsInRange(hotelId, roomType, new DatesRange(2024, 12, 1, 2024, 12, 31));

            // Assert
            hotelsRepository.Received(1).GetRoomsCount(hotelId, roomType);
        }

        [Fact]
        public void GetNumberOfAvailableRoomsInRange_ShouldCheckTheBookingsOfRoomsOfTheGivenTypeInTheGivenHotelInTheGivenDatesRange()
        {
            // Arrange
            const string hotelId = "HotelId";
            const string roomType = "RoomType";
            var datesRange = new DatesRange(2024, 12, 1, 2024, 12, 31);

            var hotelsRepository = Substitute.For<IHotelsRepository>();
            var bookingsRepository = Substitute.For<IBookingsRepository>();
            var bookingAnalyzisService = Substitute.For<IBookingsAnalyzisService>();

            var sut = new RoomFindingService(hotelsRepository, bookingsRepository, bookingAnalyzisService);

            // Act
            var result = sut.GetNumberOfAvailableRoomsInRange(hotelId, roomType, new DatesRange(2024, 12, 1, 2024, 12, 31));

            // Assert
            bookingsRepository.Received(1).GetBookingsInRange(hotelId, roomType, datesRange);
        }

        [Fact]
        public void GetNumberOfAvailableRoomsOverTime_ThereAreNoBookingsInRange_ShouldReturnSingleRecordWithAllRoomsAvailableOverTheGivenRange()
        {
            // Arrange
            const string hotelId = "HotelId";
            const string roomType = "RoomType";
            const int roomsCount = 5;
            var datesRange = new DatesRange(2024, 12, 1, 2024, 12, 31);

            var hotelsRepository = Substitute.For<IHotelsRepository>();
            hotelsRepository.GetRoomsCount(Arg.Any<string>(), Arg.Any<string>()).Returns(roomsCount);
            
            var bookingsRepository = Substitute.For<IBookingsRepository>();
            var bookingAnalyzisService = Substitute.For<IBookingsAnalyzisService>();

            var sut = new RoomFindingService(hotelsRepository, bookingsRepository, bookingAnalyzisService);

            // Act
            var result = sut.GetNumberOfAvailableRoomsOverTime(hotelId, roomType, datesRange);

            // Assert
            Assert.Equal([(datesRange, roomsCount)], result);
        }
    }

}