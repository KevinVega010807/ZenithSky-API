using Microsoft.AspNetCore.Mvc;
using ZenithSky.API.DTOs;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airlines")]
    [ApiController]
    public class AirlinesController : ControllerBase
    {
        private readonly AirlineBusiness airlineBusiness;

        public AirlinesController(AirlineBusiness airlineBusiness)
        {
            this.airlineBusiness = airlineBusiness;
        }

        //get; api/airlines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airline>>> Get([FromQuery] string? search)
        {
            return Ok(await airlineBusiness.GetAirlines(search));
        }

        //get api/airlines/5
        [HttpGet("{id:int}", Name = "GetAirlineById")]
        public async Task<ActionResult<Airline>> GetById(int id)
        {
            return Ok(await airlineBusiness.GetAirlineById(id));
        }

        //post: api/airlines
        [HttpPost]
        public async Task<ActionResult<Airline>> Post([FromBody] AirlineRequest request)
        {
            var airline = ToDomain(0, request);
            Airline created = await airlineBusiness.CreateAirline(airline);
            return CreatedAtRoute("GetAirlineById", new { id = created.AirlineId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Airline>> Put(int id, [FromBody] AirlineRequest request)
        {
            var airline = ToDomain(id, request);
            return Ok(await airlineBusiness.UpdateAirline(id, airline));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await airlineBusiness.DeleteAirline(id);
            return NoContent();
        }

        private static Airline ToDomain(int id, AirlineRequest request)
        {
            return new Airline
            {
                AirlineId = id,
                Iata = request.Iata ?? string.Empty,
                AirlineName = request.AirlineName ?? string.Empty,
                BaseAirport = new Airport { AirportId = request.BaseAirport }
            };
        }
    }
}
