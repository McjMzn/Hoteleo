namespace Hoteleo.Domain.Model
{
    internal class Booking
    {
        public string HotelId { get; set; }
        public string RoomType { get; set; }
        public string RoomRate { get; set; }
        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
    }
}
