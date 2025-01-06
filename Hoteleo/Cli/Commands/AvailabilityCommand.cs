using Hoteleo.Cli.Generic;
using Hoteleo.Domain;
using Hoteleo.Domain.Services;

namespace Hoteleo.Cli.Commands
{
    internal class AvailabilityCommand : ICliApplicationCommand
    {
        private readonly IRoomFindingService _roomFindingService;

        public AvailabilityCommand(IRoomFindingService roomFindingService)
        {
            _roomFindingService = roomFindingService;
        }
        
        public bool CanRun(string command) => command.StartsWith("Availability(") && command.EndsWith(')');

        public CommandResult Run(string command)
        {
            var arguments = ExtractArguments(command);
            
            var availableRoomsCount = _roomFindingService.GetNumberOfAvailableRoomsInRange(arguments.HotelId, arguments.RoomType, arguments.DatesRange);

            return new CommandResult(availableRoomsCount.ToString(), false, false);
        }

        private (string HotelId, string RoomType, DatesRange DatesRange) ExtractArguments(string command)
        {
            var args = CommandUtilities.ExtractArguments(command);

            var hotelId = args[0];
            
            var datesRange = args[1].Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToArray();
            var from = DateTime.ParseExact(datesRange[0], "yyyyMMdd", null);
            var to = datesRange.Length > 1 ? DateTime.ParseExact(datesRange[1], "yyyyMMdd", null) : from.AddDays(1);

            var roomType = args[2];

            return (hotelId, roomType, new DatesRange(from, to));
        }
    }
}
