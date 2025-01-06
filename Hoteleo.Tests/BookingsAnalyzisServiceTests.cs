using Hoteleo.Domain;
using Hoteleo.Domain.Model;
using Hoteleo.Domain.Services;

namespace Hoteleo.Tests
{
    public class BookingsAnalyzisServiceTests
    {
        [Fact]
        public void GetBookingEvents_ShouldReturnExpectedSequenceOfBookingEvents()
        {
            // Arrange
            var bookings = new Booking[]
            {
                new (){ Arrival = new(2024, 12, 1), Departure = new DateTime(2024, 12, 3) },
                new (){ Arrival = new(2024, 12, 3), Departure = new DateTime(2024, 12, 4) },
                new (){ Arrival = new(2024, 12, 4), Departure = new DateTime(2024, 12, 6) },
            };

            var expectedBookingEvents = new BookingEvent[]
            {
                new BookingEvent(new(2024, 12, 1), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 6), BookingEventType.Departure),
            };

            var sut = new BookingsAnalyzisService();

            // Act
            var bookingEvents = sut.GetBookingEvents(bookings);

            // Assert
            Assert.Equal(expectedBookingEvents, bookingEvents);
        }

        [Fact]
        public void CountRequiredRooms_BookingsAreNotOverlapping_ShouldReturn1()
        {
            // Arrange
            // | 01.12 | 02.12 | 03.12 | 04.12 | 05.12 | 06.12 |
            //      ---------------
            //                      -------
            //                              ---------------
            var bookingEvents = new BookingEvent[]
            {
                new BookingEvent(new(2024, 12, 1), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 6), BookingEventType.Departure),
            };

            var sut = new BookingsAnalyzisService();

            // Act
            var count = sut.CountRequiredRooms(bookingEvents);

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public void CountRequiredRooms_OnePairOfBookingsIsOverlapping_ShouldReturn2()
        {
            // Arrange
            // | 01.12 | 02.12 | 03.12 | 04.12 | 05.12 | 06.12 |
            //      -----------------------
            //                      -------
            //                              ---------------
            var bookingEvents = new BookingEvent[]
            {
                new BookingEvent(new(2024, 12, 1), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 6), BookingEventType.Departure),
            };

            var sut = new BookingsAnalyzisService();

            // Act
            var count = sut.CountRequiredRooms(bookingEvents);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void CountRequiredRooms_ThereAreNoBookings_ShouldReturn0()
        {
            // Arrange
            // | 01.12 | 02.12 | 03.12 | 04.12 | 05.12 | 06.12 |
            //
            var bookingEvents = new BookingEvent[0];

            var sut = new BookingsAnalyzisService();

            // Act
            var count = sut.CountRequiredRooms(bookingEvents);

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetNumberOfRoomsReservedPerTimeRange_ShouldReturnExpectedResult()
        {
            // Arrange
            // | 01.12 | 02.12 | 03.12 | 04.12 | 05.12 | 06.12 | 07.12 |
            //      -------         -------         -------
            //                      ---------------
            //                              -----------------------
            var bookingEvents = new List<BookingEvent>
            {
                new BookingEvent(new(2024, 12, 1), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 2), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 3), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 4), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 5), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 5), BookingEventType.Arrival),
                new BookingEvent(new(2024, 12, 6), BookingEventType.Departure),
                new BookingEvent(new(2024, 12, 7), BookingEventType.Departure),
            };

            var expectedResult = new (DatesRange DatesRange, int RoomsRequired)[]
            {
                (new DatesRange(2024, 12, 1, 2024, 12, 2), 1),
                (new DatesRange(2024, 12, 2, 2024, 12, 3), 0),
                (new DatesRange(2024, 12, 3, 2024, 12, 6), 2),
                (new DatesRange(2024, 12, 6, 2024, 12, 7), 1),
            };

            var sut = new BookingsAnalyzisService();

            // Act
            var result = sut.GetNumberOfRoomsReservedPerTimeRange(bookingEvents);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
