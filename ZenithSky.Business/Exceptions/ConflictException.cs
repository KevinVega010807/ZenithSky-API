namespace ZenithSky.Business.Exceptions
{
    // Se traduce a HTTP 409 Conflict en ExceptionMiddleware
    // Uso: eliminar aeropuerto que es base_airport, eliminar aerolínea con aviones, IATA/ICAO duplicado
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
