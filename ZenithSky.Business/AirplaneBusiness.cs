using ZenithSky.Business.Exceptions;
using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Business
{
    public class AirplaneBusiness
    {
        private readonly AirplaneData airplaneData;
        private readonly AirplaneTypeData airplaneTypeData;

        public AirplaneBusiness(AirplaneData airplaneData, AirplaneTypeData airplaneTypeData)
        {
            this.airplaneData = airplaneData;
            this.airplaneTypeData = airplaneTypeData;
        }

        public async Task<IEnumerable<Airplane>> GetAirplanes(int? typeId, int? airlineId)
        {
            return await airplaneData.GetAirplanes(typeId, airlineId);
        }

        public async Task<Airplane> GetAirplaneById(int airplaneId)
        {
            Airplane? airplane = await airplaneData.GetAirplaneById(airplaneId);
            if (airplane == null)
            {
                throw new NotFoundException($"No existe el avión con id {airplaneId}.");
            }
            return airplane;
        }

        public async Task<Airplane> CreateAirplane(Airplane airplane)
        {
            await Validate(airplane);
            int newId = await airplaneData.InsertAirplane(airplane);
            return await GetAirplaneById(newId);
        }

        public async Task<Airplane> UpdateAirplane(Airplane airplane)
        {
            await GetAirplaneById(airplane.AirplaneId); 
            await Validate(airplane);
            await airplaneData.UpdateAirplane(airplane);
            return await GetAirplaneById(airplane.AirplaneId);
        }

        public async Task DeleteAirplane(int airplaneId)
        {
            int rows = await airplaneData.DeleteAirplane(airplaneId);
            if (rows == 0)
            {
                throw new NotFoundException($"No existe el avión con id {airplaneId}.");
            }
        }

        private async Task Validate(Airplane airplane)
        {
            if (airplane.Capacity <= 0)
            {
                throw new BusinessRuleException("La capacidad de pasajeros debe ser un entero mayor que cero.");
            }
            if (await airplaneTypeData.GetAirplaneTypeById(airplane.AirplaneType.TypeId) == null)
            {
                throw new BusinessRuleException($"No existe el tipo de avión con id {airplane.AirplaneType.TypeId}.");
            }
            if (!await airplaneData.AirlineExists(airplane.Airline.AirlineId))
            {
                throw new BusinessRuleException($"No existe la aerolínea con id {airplane.Airline.AirlineId}.");
            }
        }
    }
}