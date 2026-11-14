using ZenithSky.Domain;

namespace ZenithSky.Test;

public class AirlineTest
{
    [Test]
    public void Airline_Constructor_Con_PropiedadesInicializadas()
    {
        // Arrange
        Airport airport = new Airport(1, "SJO", "MROC", "Aeropuerto Internacional Juan Santamaría");

        // Act
        Airline airline = new Airline(1, "ZS", "ZenithSky Airlines", airport);

        // Assert
        Assert.That(airline.AirlineId, Is.EqualTo(1));
        Assert.That(airline.Iata, Is.EqualTo("ZS"));
        Assert.That(airline.AirlineName, Is.EqualTo("ZenithSky Airlines"));
        Assert.That(airline.BaseAirport.AirportId, Is.EqualTo(1));
        Assert.That(airline.BaseAirport.Iata, Is.EqualTo("SJO"));
        Assert.That(airline.BaseAirport.Name, Is.EqualTo("Aeropuerto Internacional Juan Santamaría"));
    }
}
