namespace ZenithSky.Business.Exceptions
{
    // Se traduce a HTTP 404 Not Found en ExceptionMiddleware
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
