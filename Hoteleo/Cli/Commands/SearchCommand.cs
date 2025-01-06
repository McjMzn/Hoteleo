using Hoteleo.Cli.Generic;
using Hoteleo.Domain;
using Hoteleo.Domain.Services;
using Hoteleo.Utilities;

namespace Hoteleo.Cli.Commands
{
    internal class SearchCommand : ICliApplicationCommand
    {
        private readonly IRoomFindingService _roomFindingService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public SearchCommand(IRoomFindingService roomFindingService, IDateTimeProvider dateTimeProvider)
        {
            _roomFindingService = roomFindingService;
            _dateTimeProvider = dateTimeProvider;
        }

        public bool CanRun(string command) => command.StartsWith("Search(") && command.EndsWith(')');

        public CommandResult Run(string command)
        {
            var arguments = ExtractArguments(command);

            var availableRoomsCount = _roomFindingService.GetNumberOfAvailableRoomsOverTime(arguments.HotelId, arguments.RoomType, arguments.DatesRange);

            var dateFormat = "yyyyMMdd";
            var output = string.Join(',', availableRoomsCount.Select(x => $"({x.Range.Start.ToString(dateFormat)}-{x.Range.End.ToString(dateFormat)}, {x.FreeRoomsCount})"));

            return new CommandResult(output, false, false);
        }

        private (string HotelId, string RoomType, DatesRange DatesRange) ExtractArguments(string command)
        {
            var args = CommandUtilities.ExtractArguments(command);

            var hotelId = args[0];
            var daysAhead = int.Parse(args[1]);
            var roomType = args[2];

            var from = _dateTimeProvider.UtcNow;
            var to = from.AddDays(daysAhead);

            return (hotelId, roomType, new DatesRange(from, to));
        }
    }
}
