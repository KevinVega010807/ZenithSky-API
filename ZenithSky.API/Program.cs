using ZenithSky.API.Middleware;
using ZenithSky.API.Middleware;
using ZenithSky.Business;
using ZenithSky.Data;


var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("ZenithSky")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:ZenithSky en appsettings.json");

// Data 
builder.Services.AddScoped(_ => new AirportData(connectionString));
builder.Services.AddScoped(_ => new AirplaneTypeData(connectionString));
// Business 
builder.Services.AddScoped<AirportBusiness>();
builder.Services.AddScoped<AirplaneTypeBusiness>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Traduce NotFound/Conflict/BusinessRule a 404/409/400 con JSON { "message": "..." }
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// Sin UseHttpsRedirection: el emulador Android consume por http://10.0.2.2:5080
app.MapControllers();

app.Run();
