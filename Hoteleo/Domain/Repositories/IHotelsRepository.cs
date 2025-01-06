using Hoteleo.Domain.Model;
using Hoteleo.Utilities;
using Hoteleo.Utilities.Serialization;

namespace Hoteleo.Domain.Repositories
{
    internal interface IHotelsRepository
    {
        Hotel GetHotel(string hotelId);

        int GetRoomsCount(string hotelId, string roomType);
    }

    internal interface IJsonHotelsRepositoryConfiguration
    {
        string HotelsJsonFilePath { get; }
    }

    internal class JsonHotelsRepository : IHotelsRepository
    {
        private readonly List<Hotel> _hotels;

        public JsonHotelsRepository(IJsonHotelsRepositoryConfiguration configuration, IFileSystem fileSystem, IJsonSerializer jsonSerializer)
        {
            var json = fileSystem.ReadAllText(configuration.HotelsJsonFilePath);
            _hotels = jsonSerializer.Deserialize<List<Hotel>>(json);
        }

        public Hotel GetHotel(string hotelId)
        {
            return _hotels.FirstOrDefault(h => h.Id == hotelId);
        }

        public int GetRoomsCount(string hotelId, string roomType)
        {
            return GetHotel(hotelId).Rooms.Count(r => r.RoomType == roomType);
        }
    }
}
