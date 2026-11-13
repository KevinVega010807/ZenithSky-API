using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirportData
    {
        private readonly string connectionString;

        public AirportData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        //lista de los aeropuertos, se filtra por nombre, iata o icao si se pasa un string de busqueda
        public async Task<IEnumerable<Airport>> GetAirports(string? search)
        {
            var airports = new List<Airport>();

            string query = "SELECT airport_id, iata, icao, name FROM airport";
            if (!string.IsNullOrWhiteSpace(search))
            {
                query += " WHERE name LIKE @search OR iata LIKE @search OR icao LIKE @search";
            }
            query += " ORDER BY name";
            //esto ejecuta la consulta a la base de datos y llena la lista de aeropuertos con los resultados
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
                            var airport = new Airport
                            {
                                AirportId = reader.GetInt32(reader.GetOrdinal("airport_id")),
                                Iata = reader.GetString(reader.GetOrdinal("iata")),
                                Icao = reader.GetString(reader.GetOrdinal("icao")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                            airports.Add(airport);
                        }
                    }
                }
            }
            return airports;
        }

        // Busca un aeropuerto por su id
        public async Task<Airport?> GetAirportById(int id)
        {
            Airport? airport = null;

            string query = "SELECT airport_id, iata, icao, name FROM airport WHERE airport_id = @id";

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
                            airport = new Airport
                            {
                                AirportId = reader.GetInt32(reader.GetOrdinal("airport_id")),
                                Iata = reader.GetString(reader.GetOrdinal("iata")),
                                Icao = reader.GetString(reader.GetOrdinal("icao")),
                                Name = reader.GetString(reader.GetOrdinal("name"))
                            };
                        }
                    }
                }
            }

            //esto luego nos retona el aeropuerto encontrado o null si no se encontro
            return airport;
        }


        public async Task<int> InsertAirport(Airport airport)
        {
            string query = @"INSERT INTO airport (iata, icao, name)
                             OUTPUT INSERTED.airport_id
                             VALUES (@iata, @icao, @name)";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@iata", airport.Iata);
                    command.Parameters.AddWithValue("@icao", airport.Icao);
                    command.Parameters.AddWithValue("@name", airport.Name);

                    object? result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        // Actualizar un aeropuerto
        public async Task<bool> UpdateAirport(Airport airport)
        {
            string query = @"UPDATE airport
                             SET iata = @iata, icao = @icao, name = @name
                             WHERE airport_id = @id";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@iata", airport.Iata);
                    command.Parameters.AddWithValue("@icao", airport.Icao);
                    command.Parameters.AddWithValue("@name", airport.Name);
                    command.Parameters.AddWithValue("@id", airport.AirportId);

                    int rows = await command.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        // Eliminar un aeropuerto
        public async Task<bool> DeleteAirport(int id)
        {
            string query = "DELETE FROM airport WHERE airport_id = @id";

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

        //metodo que verifica si ya existe otro aeropuerto con ese IATA 
        public async Task<bool> ExistsIata(string iata, int excludeId)
        {
            string query = "SELECT COUNT(*) FROM airport WHERE iata = @iata AND airport_id <> @excludeId";

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

        // igual que el metodo anterior pero para el ICAO
        public async Task<bool> ExistsIcao(string icao, int excludeId)
        {
            string query = "SELECT COUNT(*) FROM airport WHERE icao = @icao AND airport_id <> @excludeId";

            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@icao", icao);
                    command.Parameters.AddWithValue("@excludeId", excludeId);

                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return count > 0;
                }
            }
        }

        // esto es para lo del 409 verifica si el aeropuerto es base de alguna aerolinea para poder borrarlo 
        public async Task<bool> IsBaseAirportOfAnyAirline(int airportId)
        {
            string query = "SELECT COUNT(*) FROM airline WHERE base_airport = @id";

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
    }
}