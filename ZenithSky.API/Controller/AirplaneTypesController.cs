using Microsoft.AspNetCore.Mvc;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [ApiController]
    [Route("api/airplane-types")]
    public class AirplaneTypesController : ControllerBase
    {
        private readonly AirplaneTypeBusiness airplaneTypeBusiness;

        public AirplaneTypesController(AirplaneTypeBusiness airplaneTypeBusiness)
        {
            this.airplaneTypeBusiness = airplaneTypeBusiness;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AirplaneType>>> GetAll()
        {
            return Ok(await airplaneTypeBusiness.GetAirplaneTypes());
        }
    }
}