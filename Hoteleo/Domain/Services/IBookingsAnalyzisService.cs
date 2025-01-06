using Hoteleo.Domain.Model;

namespace Hoteleo.Domain.Services
{
    internal enum BookingEventType
    {
        Departure,
        Arrival,
    }

    internal record BookingEvent(DateTime Date, BookingEventType Type);

    internal interface IBookingsAnalyzisService
    {
        IEnumerable<BookingEvent> GetBookingEvents(IEnumerable<Booking> bookings);

        IReadOnlyList<(DatesRange DatesRange, int RoomsRequired)> GetNumberOfRoomsReservedPerTimeRange(IEnumerable<BookingEvent> bookingEvents);

        int CountRequiredRooms(IEnumerable<BookingEvent> bookingEvents);
    }

    internal class BookingsAnalyzisService : IBookingsAnalyzisService
    {
        public IEnumerable<BookingEvent> GetBookingEvents(IEnumerable<Booking> bookings)
        {
            return
                bookings
                .SelectMany(booking =>
                    new BookingEvent[] {
                        new BookingEvent(booking.Arrival, BookingEventType.Arrival),
                        new BookingEvent(booking.Departure, BookingEventType.Departure),
                    });
        }

        public int CountRequiredRooms(IEnumerable<BookingEvent> bookingEvents)
        {
            var roomsPerRange = GetNumberOfRoomsReservedPerTimeRange(bookingEvents);
            if (roomsPerRange.Count == 0)
            {
                return 0;
            }

            return roomsPerRange.Select(r => r.RoomsRequired).Max();
        }

        public IReadOnlyList<(DatesRange DatesRange, int RoomsRequired)> GetNumberOfRoomsReservedPerTimeRange(IEnumerable<BookingEvent> bookingEvents)
        {
            var ranges = new List<(DatesRange DatesRange, int RoomsRequired)>();
            if (!bookingEvents.Any())
            {
                return ranges;
            }

            DateTime rangeStart = bookingEvents.First().Date;
            var requiredRooms = 0;
            var isFirstIteration = true;

            var eventsByDay = bookingEvents.GroupBy(e => e.Date).OrderBy(group => group.Key);
            var lastDate = eventsByDay.Last().Key;
            
            foreach (var dayEvents in eventsByDay)
            {
                var dayChange = dayEvents.Select(dayEvent => dayEvent.Type is BookingEventType.Arrival ? 1 : -1).Sum();
                if (isFirstIteration)
                {
                    isFirstIteration = false;
                    requiredRooms = dayChange;
                    continue;
                }

                if (dayChange == 0 && dayEvents.Key != lastDate)
                {
                    continue;
                }

                ranges.Add((new (rangeStart, dayEvents.Key), requiredRooms));
                rangeStart = dayEvents.Key;

                requiredRooms += dayChange;
            }

            return ranges;
        }
    }
}
