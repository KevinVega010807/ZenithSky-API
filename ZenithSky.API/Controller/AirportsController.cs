using Microsoft.AspNetCore.Mvc;
using ZenithSky.API.DTOs;
using ZenithSky.Business;
using ZenithSky.Domain;

namespace ZenithSky.API.Controllers
{
    [Produces("application/json")]
    [Route("api/airports")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly AirportBusiness airportBusiness;

        public AirportsController(AirportBusiness airportBusiness)
        {
            this.airportBusiness = airportBusiness;
        }

        // GET: api/airports  o  api/airports?search=sjo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airport>>> Get([FromQuery] string? search)
        {
            return Ok(await airportBusiness.GetAirports(search));
        }

        // GET: api/airports/5
        [HttpGet("{id:int}", Name = "GetAirportById")]
        public async Task<ActionResult<Airport>> GetById(int id)
        {
            return Ok(await airportBusiness.GetAirportById(id));
        }

        // POST: api/airports
        [HttpPost]
        public async Task<ActionResult<Airport>> Post([FromBody] AirportRequest request)
        {
            var airport = new Airport
            {
                Iata = request.Iata ?? string.Empty,
                Icao = request.Icao ?? string.Empty,
                Name = request.Name ?? string.Empty
            };

            Airport created = await airportBusiness.CreateAirport(airport);
            return CreatedAtRoute("GetAirportById", new { id = created.AirportId }, created); // 201
        }

        // PUT: api/airports/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Airport>> Put(int id, [FromBody] AirportRequest request)
        {
            var airport = new Airport
            {
                Iata = request.Iata ?? string.Empty,
                Icao = request.Icao ?? string.Empty,
                Name = request.Name ?? string.Empty
            };

            return Ok(await airportBusiness.UpdateAirport(id, airport)); // 200
        }

        // DELETE: api/airports/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await airportBusiness.DeleteAirport(id);
            return NoContent(); // 204
        }
    }
}