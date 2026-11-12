using Microsoft.Data.SqlClient;
using ZenithSky.Business.Exceptions;

namespace ZenithSky.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                await Write(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (ConflictException ex)
            {
                await Write(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                await Write(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // Respaldo: violación de llave foránea en SQL Server
                await Write(context, StatusCodes.Status409Conflict,
                    "La operación viola la integridad referencial (el registro está en uso).");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Respaldo: UNIQUE / PK duplicada
                await Write(context, StatusCodes.Status409Conflict, "Ya existe un registro con esos datos.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error no controlado");
                await Write(context, StatusCodes.Status500InternalServerError, "Error interno del servidor.");
            }
        }

        private static async Task Write(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { message });
        }
    }
}
