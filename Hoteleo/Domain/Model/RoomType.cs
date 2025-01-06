namespace Hoteleo.Domain.Model
{
    internal class RoomType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public IReadOnlyList<string> Amenities { get; set; }
        public IReadOnlyList<string> Features { get; set; }
    }
}
