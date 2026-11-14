using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirlineData
    {
        private readonly string connectionString;

        private const string SelectBase = @"
            SELECT a.airline_id, a.iata, a.airlinename,
                   ap.airport_id, ap.iata, ap.icao, ap.name
            FROM airline a
            INNER JOIN airport ap ON ap.airport_id = a.base_airport";

        public AirlineData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //obtener lista de aerolíneas
        public async Task<IEnumerable<Airline>> GetAirlines(string? search)
        {
            var airlines = new List<Airline>();

            string query = SelectBase;
            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " WHERE a.airlinename LIKE @search OR a.iata LIKE @search";
            }
            query += " ORDER BY a.airlinename";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        command.Parameters.AddWithValue("@search", $"%{search.Trim()}%");
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airlines.Add(Map(reader));
                        }
                    }
                }
            }

            return airlines;
        }

        //obtener aerolínea por ID
        public async Task<Airline?> GetAirlineById(int id)
        {
            Airline? airline = null;
            string query = SelectBase + " WHERE a.airline_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            airline = Map(reader);
                        }
                    }
                }
            }

            return airline;
        }

        //insertar una nueva aerolínea
        public async Task<int> InsertAirline(Airline airline)
        {
            string query = @"INSERT INTO airline (iata, airlinename, base_airport)
                             OUTPUT INSERTED.airline_id
                             VALUES (@iata, @airlinename, @baseAirport)";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@iata", airline.Iata);
                    command.Parameters.AddWithValue("@airlinename", airline.AirlineName);
                    command.Parameters.AddWithValue("@baseAirport", airline.BaseAirport.AirportId);

                    object? result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        //actualizar
        public async Task<bool> UpdateAirline(Airline airline)
        {
            string query = @"UPDATE airline
                             SET iata = @iata, airlinename = @airlinename, base_airport = @baseAirport
                             WHERE airline_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@iata", airline.Iata);
                    command.Parameters.AddWithValue("@airlinename", airline.AirlineName);
                    command.Parameters.AddWithValue("@baseAirport", airline.BaseAirport.AirportId);
                    command.Parameters.AddWithValue("@id", airline.AirlineId);

                    int rows = await command.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        //eliminar por id
        public async Task<bool> DeleteAirline(int id)
        {
            string query = "DELETE FROM airline WHERE airline_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    int rows = await command.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        //revisa si ya esxite un iata ligado a la areolinea
        public async Task<bool> ExistsIata(string iata, int excludeId)
        {
            string query = "SELECT COUNT(*) FROM airline WHERE iata = @iata AND airline_id <> @excludeId";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@iata", iata);
                    command.Parameters.AddWithValue("@excludeId", excludeId);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

       
        public async Task<bool> AirportExists(int airportId)
        {
            string query = "SELECT COUNT(*) FROM airport WHERE airport_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", airportId);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }


        //revisa si la aerolinea tiene aviones si no manda error
        public async Task<bool> HasAirplanes(int airlineId)
        {
            string query = "SELECT COUNT(*) FROM airplane WHERE airline_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", airlineId);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        private static Airline Map(SqlDataReader reader)
        {
            return new Airline
            {
                AirlineId = reader.GetInt32(0),
                Iata = reader.GetString(1),
                AirlineName = reader.GetString(2),
                BaseAirport = new Airport
                {
                    AirportId = reader.GetInt32(3),
                    Iata = reader.GetString(4),
                    Icao = reader.GetString(5),
                    Name = reader.GetString(6)
                }
            };
        }
    }
}
