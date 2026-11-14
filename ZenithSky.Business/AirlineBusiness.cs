using ZenithSky.Business.Exceptions;
using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Business
{
    public class AirlineBusiness
    {
        private readonly AirlineData airlineData;

        public AirlineBusiness(AirlineData airlineData)
        {
            this.airlineData = airlineData;
        }

        public async Task<IEnumerable<Airline>> GetAirlines(string? search)
        {
            return await airlineData.GetAirlines(search);
        }

        //lanza 404 si no existe
        public async Task<Airline> GetAirlineById(int id)
        {
            Airline? airline = await airlineData.GetAirlineById(id);
            if (airline == null)
            {
                throw new NotFoundException($"No existe una aerolínea con id {id}.");
            }
            return airline;
        }

        public async Task<Airline> CreateAirline(Airline airline)
        {
            await Validate(airline);

            if (await airlineData.ExistsIata(airline.Iata, 0))
            {
                throw new ConflictException($"Ya existe una aerolínea con IATA {airline.Iata}.");
            }

            int newId = await airlineData.InsertAirline(airline);
            return await GetAirlineById(newId);
        }

        //lanza 404 si no existe, valida que el aeropuerto base exista y IATA no repetido
        public async Task<Airline> UpdateAirline(int id, Airline airline)
        {
            await GetAirlineById(id); // Lanza 404 si no existe

            airline.AirlineId = id;
            await Validate(airline);

            if (await airlineData.ExistsIata(airline.Iata, id))
            {
                throw new ConflictException($"Ya existe otra aerolínea con IATA {airline.Iata}.");
            }

            await airlineData.UpdateAirline(airline);
            return await GetAirlineById(id);
        }

        //lanza 404 si no existe, y 409 si tiene aviones asignados
        public async Task DeleteAirline(int id)
        {
            await GetAirlineById(id); 

            if (await airlineData.HasAirplanes(id))
            {
                throw new ConflictException("No se puede eliminar la aerolínea porque tiene uno o más aviones asociados.");
            }

            await airlineData.DeleteAirline(id);
        }

        private async Task Validate(Airline airline)
        {
            airline.Iata = (airline.Iata ?? string.Empty).Trim().ToUpperInvariant();
            airline.AirlineName = (airline.AirlineName ?? string.Empty).Trim();

            if (airline.Iata.Length == 0)
            {
                throw new BusinessRuleException("El código IATA de la aerolínea es obligatorio.");
            }
            if (airline.Iata.Length != 2 || !airline.Iata.All(char.IsLetterOrDigit))
            {
                throw new BusinessRuleException("El código IATA de la aerolínea debe tener exactamente 2 caracteres alfanuméricos.");
            }

            if (airline.AirlineName.Length == 0)
            {
                throw new BusinessRuleException("El nombre de la aerolínea es obligatorio.");
            }
            if (airline.AirlineName.Length > 100)
            {
                throw new BusinessRuleException("El nombre de la aerolínea no puede superar los 100 caracteres.");
            }

            if (airline.BaseAirport == null || airline.BaseAirport.AirportId <= 0)
            {
                throw new BusinessRuleException("El aeropuerto base es obligatorio.");
            }

            //valida que el aeropuerto base exista en la base de datos
            if (!await airlineData.AirportExists(airline.BaseAirport.AirportId))
            {
                throw new BusinessRuleException($"No existe el aeropuerto base con id {airline.BaseAirport.AirportId}.");
            }
        }
    }
}
