using Hoteleo.Domain.Repositories;

namespace Hoteleo.Cli
{
    internal class HoteleoApplicationConfiguration : IJsonBookingsRepositoryConfiguration, IJsonHotelsRepositoryConfiguration
    {
        public string? BookingsJsonFilePath { get; set; }
        public string? HotelsJsonFilePath { get; set; }
    }
}
