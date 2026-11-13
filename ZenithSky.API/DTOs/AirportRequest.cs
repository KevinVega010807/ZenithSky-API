namespace ZenithSky.API.DTOs
{
    public class AirportRequest
    {
        public string? Iata { get; set; }
        public string? Icao { get; set; }
        public string? Name { get; set; }
    }
}//esto representa el post y put  iata, nombre  y el icao  pero sin el id