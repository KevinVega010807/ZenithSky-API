using Microsoft.Data.SqlClient;
using ZenithSky.Domain;

namespace ZenithSky.Data
{
    public class AirplaneTypeData
    {
        private readonly string connectionString;

        public AirplaneTypeData(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<AirplaneType>> GetAirplaneTypes()
        {
            var types = new List<AirplaneType>();
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT type_id, identifier, description FROM airplane_type ORDER BY identifier";
                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            types.Add(Map(reader));
                        }
                    }
                }
            }
            return types;
        }

        public async Task<AirplaneType?> GetAirplaneTypeById(int typeId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                string sql = "SELECT type_id, identifier, description FROM airplane_type WHERE type_id = @typeId";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@typeId", typeId);
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

        private static AirplaneType Map(SqlDataReader reader)
        {
            return new AirplaneType
            {
                TypeId = reader.GetInt32(0),
                Identifier = reader.GetString(1),
                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
            };
        }
    }
}