using ZenithSky.Business.Exceptions;
using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Business
{
    public class AirportBusiness
    {
        private readonly AirportData airportData;

        public AirportBusiness(AirportData airportData)
        {
            this.airportData = airportData;
        }

        // Lista los aeropuertos
        public async Task<IEnumerable<Airport>> GetAirports(string? search)
        {
            return await airportData.GetAirports(search);
        }

        // Un aeropuerto por id
        public async Task<Airport> GetAirportById(int id)
        {
            Airport? airport = await airportData.GetAirportById(id);
            if (airport == null)
            {
                throw new NotFoundException($"No existe un aeropuerto con id {id}.");
            }
            return airport;
        }

        
        public async Task<Airport> CreateAirport(Airport airport)
        {
            Validate(airport);

            if (await airportData.ExistsIata(airport.Iata, 0))
            {
                throw new ConflictException($"Ya existe un aeropuerto con IATA {airport.Iata}.");
            }
            if (await airportData.ExistsIcao(airport.Icao, 0))
            {
                throw new ConflictException($"Ya existe un aeropuerto con ICAO {airport.Icao}.");
            }

            airport.AirportId = await airportData.InsertAirport(airport);
            return airport;
        }

        //  revisa que exista
        public async Task<Airport> UpdateAirport(int id, Airport airport)
        {
            await GetAirportById(id); // muestra 404 

            Validate(airport);

            if (await airportData.ExistsIata(airport.Iata, id))
            {
                throw new ConflictException($"Ya existe otro aeropuerto con IATA {airport.Iata}.");
            }
            if (await airportData.ExistsIcao(airport.Icao, id))
            {
                throw new ConflictException($"Ya existe otro aeropuerto con ICAO {airport.Icao}.");
            }

            airport.AirportId = id;
            await airportData.UpdateAirport(airport);
            return airport;
        }

        // borrar si no existe  404; si es base de una aerolínea -> 409
        public async Task DeleteAirport(int id)
        {
            await GetAirportById(id); // lanza 404 si no existe

            if (await airportData.IsBaseAirportOfAnyAirline(id))
            {
                throw new ConflictException(
                    "No se puede eliminar el aeropuerto porque es la base de una o más aerolíneas.");
            }

            await airportData.DeleteAirport(id);
        }

        // las reglas que pedia el profe, con iata, icao y nombre
        private static void Validate(Airport airport)
        {
            airport.Iata = (airport.Iata ?? string.Empty).Trim().ToUpperInvariant();
            airport.Icao = (airport.Icao ?? string.Empty).Trim().ToUpperInvariant();
            airport.Name = (airport.Name ?? string.Empty).Trim();

            if (airport.Iata.Length == 0)
            {
                throw new BusinessRuleException("El IATA es obligatorio.");
            }
            if (airport.Iata.Length != 3 || !airport.Iata.All(char.IsAsciiLetter))
            {
                throw new BusinessRuleException("El IATA debe tener exactamente 3 letras.");
            }

            if (airport.Icao.Length == 0)
            {
                throw new BusinessRuleException("El ICAO es obligatorio.");
            }
            if (airport.Icao.Length != 4 || !airport.Icao.All(char.IsAsciiLetter))
            {
                throw new BusinessRuleException("El ICAO debe tener exactamente 4 letras.");
            }

            if (airport.Name.Length == 0)
            {
                throw new BusinessRuleException("El nombre es obligatorio.");
            }
            if (airport.Name.Length > 100)
            {
                throw new BusinessRuleException("El nombre no puede pasar de 100 caracteres.");
            }
        }
    }
}