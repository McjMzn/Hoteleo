using Hoteleo.Cli;
using Hoteleo.Cli.Generic;
using Hoteleo.Domain.Model;
using Hoteleo.Utilities;
using Hoteleo.Utilities.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace Hoteleo.Tests
{
    public class HoteleoApplicationTests
    {
        private readonly DateTime CurrentDate = new DateTime(2024, 8, 1);

        private readonly IReadOnlyList<Hotel> Hotels = [
            new Hotel
            {
                Id = "H1",
                Rooms =
                [
                    new (){ RoomId = "101", RoomType = "SGL"},
                    new (){ RoomId = "102", RoomType = "SGL"},
                    new (){ RoomId = "201", RoomType = "DBL"},
                    new (){ RoomId = "202", RoomType = "DBL"},
                ]
            }
        ];

        private readonly IReadOnlyList<Booking> Bookings = [
            new() { HotelId = "H1", Arrival = new DateTime(2024, 8, 1), Departure = new DateTime(2024, 8, 31), RoomType = "SGL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 8, 1), Departure = new DateTime(2024, 8, 31), RoomType = "SGL" },


            new() { HotelId = "H1", Arrival = new DateTime(2024, 9, 1), Departure = new DateTime(2024, 9, 3), RoomType = "DBL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 9, 2), Departure = new DateTime(2024, 9, 5), RoomType = "SGL" },

            new() { HotelId = "H1", Arrival = new DateTime(2024, 1, 10), Departure = new DateTime(2024, 1, 15), RoomType = "SGL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 1, 11), Departure = new DateTime(2024, 1, 15), RoomType = "SGL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 1, 12), Departure = new DateTime(2024, 1, 15), RoomType = "SGL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 1, 13), Departure = new DateTime(2024, 1, 15), RoomType = "SGL" },
            new() { HotelId = "H1", Arrival = new DateTime(2024, 1, 14), Departure = new DateTime(2024, 1, 15), RoomType = "SGL" },
        ];

        private Action<IServiceCollection> OverrideServices(string[] inputLines, out ISystemConsole systemConsole)
        {
            var fileSystem = Substitute.For<IFileSystem>();

            var dateTimeProvider = Substitute.For<IDateTimeProvider>();
            dateTimeProvider.UtcNow.Returns(CurrentDate);

            var console = Substitute.For<ISystemConsole>();
            console.ReadLine().Returns(inputLines[0], inputLines.Skip(1).ToArray());

            var jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.Deserialize<List<Hotel>>(Arg.Any<string>()).Returns(Hotels.ToList());
            jsonSerializer.Deserialize<List<Booking>>(Arg.Any<string>()).Returns(Bookings.ToList());

            systemConsole = console;

            return 
                services =>
                {
                    services.Replace(new(typeof(IDateTimeProvider), dateTimeProvider));
                    services.Replace(new(typeof(ISystemConsole), console));
                    services.Replace(new(typeof(IJsonSerializer), jsonSerializer));
                    services.Replace(new(typeof(IFileSystem), fileSystem));
                };
        }

        [Theory]
        [InlineData(new string[] { "" }, ExitCode.Success, null)]
        [InlineData(new string[] { "INPUT NOT MATCHING ANY COMMAND" }, ExitCode.Aborted, null)]
        [InlineData(new string[] { "Availability(H1,20240901,SGL)", "" }, ExitCode.Success, new string[] { "1", "" })]
        [InlineData(new string[] { "Availability( H1 , 20240901 , SGL)", "" }, ExitCode.Success, new string[] { "1", "" })]
        [InlineData(new string[] { "Availability(H1, 20241201, SGL)", "" }, ExitCode.Success, new string[] { "2", "" })]
        [InlineData(new string[] { "Availability(H1, 20240114, SGL)", "" }, ExitCode.Success, new string[] { "-3", "" })]
        [InlineData(new string[] { "Availability(H1, 20240101-20240114, SGL)", "" }, ExitCode.Success, new string[] { "-3", "" })]
        [InlineData(new string[] { "Availability(H1, 20231231-20240110, SGL)", "" }, ExitCode.Success, new string[] { "1", "" })]
        [InlineData(new string[] { "Search(H1,365,SGL)", "" }, ExitCode.Success, new string[] { "(20240831-20240902, 2),(20240902-20240905, 1),(20240905-20250801, 2)", "" })]
        [InlineData(new string[] { "Search(H1,30,SGL)", "" }, ExitCode.Success, new string[] { "", "" })]
        [InlineData(new string[] { "Search(H1, 365, DBL)", "" }, ExitCode.Success, new string[] { "(20240801-20240901, 2),(20240901-20240903, 1),(20240903-20250801, 2)", "" })]
        public void Run_ProvidedSearchCommandFollowedByEmptyLine_ShouldPrintExpectedOutputAndExitWithExpectedExitCode(string[] input, int expectedExitCode, string[] expectedOutputLines)
        {
            // Arrange
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            // Act
            var exitCode = new HoteleoApplicationLauncher([]).Launch(OverrideServices(input, out var systemConsole), cts.Token);

            // Assert
            Assert.Equal(expectedExitCode, exitCode);
            
            if (expectedOutputLines is not null)
            {
                var outputLines = systemConsole.ReceivedCalls().Where(c => c.GetMethodInfo().Name == nameof(ISystemConsole.WriteLine)).Select(c => c.GetArguments()[0]);
                Assert.Equal(expectedOutputLines, outputLines);
            }
        }
    }
}
