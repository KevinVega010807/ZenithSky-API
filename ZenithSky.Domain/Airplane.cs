namespace ZenithSky.Domain
{
    public class Airplane
    {
        private int airplaneId;
        private int capacity;
        private AirplaneType airplaneType;
        private Airline airline;

        public Airplane()
        {
            this.airplaneType = new AirplaneType();
            this.airline = new Airline();
        }

        public Airplane(int airplaneId, int capacity, AirplaneType airplaneType, Airline airline)
        {
            this.airplaneId = airplaneId;
            this.capacity = capacity;
            this.airplaneType = airplaneType;
            this.airline = airline;
        }

        public int AirplaneId { get => airplaneId; set => airplaneId = value; }
        public int Capacity { get => capacity; set => capacity = value; }
        public AirplaneType AirplaneType { get => airplaneType; set => airplaneType = value; }
        public Airline Airline { get => airline; set => airline = value; }
    }
}