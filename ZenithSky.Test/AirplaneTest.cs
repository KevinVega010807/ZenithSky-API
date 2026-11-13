using ZenithSky.Domain;

namespace ZenithSky.Test;

public class AirplaneTest
{
    [Test]
    public void Airplane_Constructor_Con_PropiedadesInicializadas()
    {
        // Arrange
        Airport airport = new Airport(1, "SJO", "MROC", "Aeropuerto Internacional Juan Santamaría");
        Airline airline = new Airline(1, "ZS", "ZenithSky Airlines", airport);
        AirplaneType type = new AirplaneType(2, "B737-800", "Boeing 737-800");

        // Act
        Airplane airplane = new Airplane(10, 186, type, airline);

        // Assert
        Assert.That(airplane.AirplaneId, Is.EqualTo(10));
        Assert.That(airplane.Capacity, Is.EqualTo(186));
        Assert.That(airplane.AirplaneType.Identifier, Is.EqualTo("B737-800"));
        Assert.That(airplane.Airline.AirlineName, Is.EqualTo("ZenithSky Airlines"));
        Assert.That(airplane.Airline.BaseAirport.Iata, Is.EqualTo("SJO"));
    }
}