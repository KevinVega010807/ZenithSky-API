using ZenithSky.Data;
using ZenithSky.Domain;

namespace ZenithSky.Business
{
    public class AirplaneTypeBusiness
    {
        private readonly AirplaneTypeData airplaneTypeData;

        public AirplaneTypeBusiness(AirplaneTypeData airplaneTypeData)
        {
            this.airplaneTypeData = airplaneTypeData;
        }

        public async Task<IEnumerable<AirplaneType>> GetAirplaneTypes()
        {
            return await airplaneTypeData.GetAirplaneTypes();
        }
    }
}