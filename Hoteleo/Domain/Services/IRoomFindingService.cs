using Hoteleo.Domain.Repositories;

namespace Hoteleo.Domain.Services
{
    internal interface IRoomFindingService
    {
        int GetNumberOfAvailableRoomsInRange(string hotelId, string roomType, DatesRange range);

        IReadOnlyList<(DatesRange Range, int FreeRoomsCount)> GetNumberOfAvailableRoomsOverTime(string hotelId, string roomType, DatesRange datesRange);
    }

    internal class RoomFindingService : IRoomFindingService
    {
        private readonly IBookingsRepository _bookingsRepository;
        private readonly IBookingsAnalyzisService _bookingsAnalyzisService;
        private readonly IHotelsRepository _hotelsRepository;

        public RoomFindingService(IHotelsRepository hotelsRepository, IBookingsRepository bookingsRepository, IBookingsAnalyzisService bookingsAnalyzisService)
        {
            _hotelsRepository = hotelsRepository;
            _bookingsRepository = bookingsRepository;
            _bookingsAnalyzisService = bookingsAnalyzisService;
        }

        public int GetNumberOfAvailableRoomsInRange(string hotelId, string roomType, DatesRange datesRange)
        {
            var matchingRoomsCount = _hotelsRepository.GetRoomsCount(hotelId, roomType);
            var bookings = _bookingsRepository.GetBookingsInRange(hotelId, roomType, datesRange);

            var bookingEvents = _bookingsAnalyzisService.GetBookingEvents(bookings);
            var requiredRoomsCount = _bookingsAnalyzisService.CountRequiredRooms(bookingEvents);

            return matchingRoomsCount - requiredRoomsCount;
        }

        public IReadOnlyList<(DatesRange Range, int FreeRoomsCount)> GetNumberOfAvailableRoomsOverTime(string hotelId, string roomType, DatesRange datesRange)
        {
            var matchingRoomsCount = _hotelsRepository.GetRoomsCount(hotelId, roomType);
            var bookings = _bookingsRepository.GetBookingsInRange(hotelId, roomType, datesRange);
            var bookingEvents = _bookingsAnalyzisService.GetBookingEvents(bookings);

            var roomsPerRange = 
                _bookingsAnalyzisService
                .GetNumberOfRoomsReservedPerTimeRange(bookingEvents)
                .Select(record => (record.DatesRange, matchingRoomsCount - record.RoomsRequired))
                .ToList();

            if (roomsPerRange.Count == 0)
            {
                roomsPerRange.Add((datesRange, matchingRoomsCount));
                return roomsPerRange;
            }

            if (datesRange.Start < roomsPerRange[0].DatesRange.Start)
            {
                roomsPerRange.Insert(0, (new(datesRange.Start, roomsPerRange[0].DatesRange.Start), matchingRoomsCount));
            }

            if (datesRange.End > roomsPerRange.Last().DatesRange.End)
            {
                roomsPerRange.Add((new(roomsPerRange.Last().DatesRange.End, datesRange.End), matchingRoomsCount));
            }

            return roomsPerRange;
        }        
    }
}
