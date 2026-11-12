namespace ZenithSky.Business.Exceptions
{
    // Se traduce a HTTP 400 Bad Request en ExceptionMiddleware
    // Uso: datos inválidos (capacity <= 0, IATA con largo incorrecto, FK que no existe, etc.)
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
