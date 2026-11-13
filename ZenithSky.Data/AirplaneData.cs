using System.Data;
using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirplaneData
    {
        private readonly string connectionString;

        private const string SelectBase = @"
            SELECT p.airplane_id, p.capacity,
                   t.type_id, t.identifier, t.description,
                   a.airline_id, a.iata, a.airlinename,
                   ap.airport_id, ap.iata, ap.icao, ap.name
            FROM airplane p
            INNER JOIN airplane_type t ON t.type_id = p.type_id
            INNER JOIN airline a ON a.airline_id = p.airline_id
            LEFT JOIN airport ap ON ap.airport_id = a.base_airport";

        public AirplaneData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<Airplane>> GetAirplanes(int? typeId, int? airlineId)
        {
            var airplanes = new List<Airplane>();
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = SelectBase + @"
            WHERE (@typeId IS NULL OR p.type_id = @typeId)
              AND (@airlineId IS NULL OR p.airline_id = @airlineId)
            ORDER BY p.airplane_id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@typeId", SqlDbType.Int).Value = (object?)typeId ?? DBNull.Value;
                    command.Parameters.Add("@airlineId", SqlDbType.Int).Value = (object?)airlineId ?? DBNull.Value;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            airplanes.Add(Map(reader));
                        }
                    }
                }
            }
            return airplanes;
        }

        public async Task<Airplane?> GetAirplaneById(int airplaneId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = SelectBase + " WHERE p.airplane_id = @airplaneId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@airplaneId", SqlDbType.Int).Value = airplaneId;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Map(reader);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<int> InsertAirplane(Airplane airplane)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = @"INSERT INTO airplane (capacity, type_id, airline_id)
                               OUTPUT INSERTED.airplane_id
                               VALUES (@capacity, @typeId, @airlineId)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@capacity", SqlDbType.Int).Value = airplane.Capacity;
                    command.Parameters.Add("@typeId", SqlDbType.Int).Value = airplane.AirplaneType.TypeId;
                    command.Parameters.Add("@airlineId", SqlDbType.Int).Value = airplane.Airline.AirlineId;
                    object? result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<int> UpdateAirplane(Airplane airplane)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = @"UPDATE airplane
                               SET capacity = @capacity, type_id = @typeId, airline_id = @airlineId
                               WHERE airplane_id = @airplaneId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@capacity", SqlDbType.Int).Value = airplane.Capacity;
                    command.Parameters.Add("@typeId", SqlDbType.Int).Value = airplane.AirplaneType.TypeId;
                    command.Parameters.Add("@airlineId", SqlDbType.Int).Value = airplane.Airline.AirlineId;
                    command.Parameters.Add("@airplaneId", SqlDbType.Int).Value = airplane.AirplaneId;
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> DeleteAirplane(int airplaneId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = "DELETE FROM airplane WHERE airplane_id = @airplaneId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@airplaneId", SqlDbType.Int).Value = airplaneId;
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> AirlineExists(int airlineId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT COUNT(1) FROM airline WHERE airline_id = @airlineId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@airlineId", SqlDbType.Int).Value = airlineId;
                    object? result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        private static Airplane Map(SqlDataReader reader)
        {
            var airplane = new Airplane
            {
                AirplaneId = reader.GetInt32(0),
                Capacity = reader.GetInt32(1),
                AirplaneType = new AirplaneType
                {
                    TypeId = reader.GetInt32(2),
                    Identifier = reader.GetString(3),
                    Description = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                },
                Airline = new Airline
                {
                    AirlineId = reader.GetInt32(5),
                    Iata = reader.GetString(6),
                    AirlineName = reader.GetString(7)
                }
            };

            if (!reader.IsDBNull(8))
            {
                airplane.Airline.BaseAirport = new Airport
                {
                    AirportId = reader.GetInt32(8),
                    Iata = reader.GetString(9),
                    Icao = reader.GetString(10),
                    Name = reader.GetString(11)
                };
            }
            return airplane;
        }
    }
}