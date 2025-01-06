namespace Hoteleo.Domain.Model
{
    internal class Hotel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<RoomType> RoomTypes { get; set; }
        public IReadOnlyList<Room> Rooms { get; set; }
    }
}
