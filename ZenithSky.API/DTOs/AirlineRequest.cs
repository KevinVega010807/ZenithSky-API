namespace ZenithSky.API.DTOs
{
    public class AirlineRequest
    {
        public string? Iata { get; set; }
        public string? AirlineName { get; set; }
        public int BaseAirportId { get; set; }
    }
}
