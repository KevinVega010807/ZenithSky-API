using Microsoft.AspNetCore.Mvc;
using ZenithSky.API.DTOs;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [ApiController]
    [Route("api/airplanes")]
    public class AirplanesController : ControllerBase
    {
        private readonly AirplaneBusiness airplaneBusiness;

        public AirplanesController(AirplaneBusiness airplaneBusiness)
        {
            this.airplaneBusiness = airplaneBusiness;
        }

    
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airplane>>> GetAll([FromQuery] int? typeId, [FromQuery] int? airlineId)
        {
            return Ok(await airplaneBusiness.GetAirplanes(typeId, airlineId));
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Airplane>> GetById(int id)
        {
            return Ok(await airplaneBusiness.GetAirplaneById(id));
        }

       
        [HttpPost]
        public async Task<ActionResult<Airplane>> Create([FromBody] AirplaneRequest request)
        {
            Airplane created = await airplaneBusiness.CreateAirplane(ToDomain(0, request));
            return CreatedAtAction(nameof(GetById), new { id = created.AirplaneId }, created);
        }

      
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Airplane>> Update(int id, [FromBody] AirplaneRequest request)
        {
            return Ok(await airplaneBusiness.UpdateAirplane(ToDomain(id, request)));
        }

       
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await airplaneBusiness.DeleteAirplane(id);
            return NoContent();
        }

        private static Airplane ToDomain(int id, AirplaneRequest request)
        {
            return new Airplane
            {
                AirplaneId = id,
                Capacity = request.Capacity,
                AirplaneType = new AirplaneType { TypeId = request.TypeId },
                Airline = new Airline { AirlineId = request.AirlineId }
            };
        }
    }
}