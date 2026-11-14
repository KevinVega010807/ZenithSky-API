namespace ZenithSky.Domain
{
    public class Airline
    {
        private int airlineId;
        private string iata = string.Empty;
        private string airlineName = string.Empty;
        private Airport baseAirport;

        public Airline()
        {
            this.baseAirport = new Airport();
        }

        public Airline(int airlineId, string iata, string airlineName, Airport baseAirport)
        {
            this.airlineId = airlineId;
            this.iata = iata;
            this.airlineName = airlineName;
            this.baseAirport = baseAirport;
        }

        public int AirlineId { get => airlineId; set => airlineId = value; }
        public string Iata { get => iata; set => iata = value; }
        public string AirlineName { get => airlineName; set => airlineName = value; }
        public Airport BaseAirport { get => baseAirport; set => baseAirport = value; }
    }
}
