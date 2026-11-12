
using Microsoft.Data.SqlClient;
using System.Globalization;
using NasaApodGallery.Models;

namespace NasaApodGallery.Services
{
    public class ApodRepository
    {
        private readonly IConfiguration _configuration;

        public ApodRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Save APOD data into SQL Server
        public async Task SaveApodDataAsync(List<ApodDto> apodList)
        {
            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is missing.");
            }

            using SqlConnection connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            string query = @"
                IF NOT EXISTS
                (
                    SELECT 1 FROM dbo.Apod WHERE [Date] = @Date
                )
                BEGIN
                    INSERT INTO dbo.Apod
                    (
                        [Date], Title, Explanation, Url,
                        MediaType, ServiceVersion
                    )
                    VALUES
                    (
                        @Date, @Title, @Explanation, @Url,
                        @MediaType, @ServiceVersion
                    )
                END";

            foreach (var apod in apodList)
            {
                DateTime apodDate = DateTime.ParseExact(
                    apod.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture
                );

                using SqlCommand command =
                    new SqlCommand(query, connection);

                command.Parameters.Add("@Date", System.Data.SqlDbType.Date)
                    .Value = apodDate;

                command.Parameters.Add("@Title", System.Data.SqlDbType.NVarChar, 255)
                    .Value = apod.Title;

                command.Parameters.Add("@Explanation", System.Data.SqlDbType.NVarChar)
                    .Value = (object)apod.Explanation ?? DBNull.Value;

                command.Parameters.Add("@Url", System.Data.SqlDbType.NVarChar, 2048)
    .Value = !string.IsNullOrWhiteSpace(apod.HdUrl)
        ? apod.HdUrl
        : apod.Url;

                command.Parameters.Add("@MediaType", System.Data.SqlDbType.NVarChar, 50)
                    .Value = (object)apod.MediaType ?? DBNull.Value;

                command.Parameters.Add("@ServiceVersion", System.Data.SqlDbType.NVarChar, 20)
                    .Value = (object)apod.ServiceVersion ?? DBNull.Value;

                await command.ExecuteNonQueryAsync();
            }
        }

        // Retrieve APOD data from SQL Server
        public async Task<List<ApodDto>> GetAllApodDataAsync()
        {
            var apodList = new List<ApodDto>();

            string? connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is missing.");
            }

            using SqlConnection connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            string query = @"
                SELECT [Date], Title, Explanation, Url,
                       MediaType, ServiceVersion
                FROM dbo.Apod
                ORDER BY [Date] DESC";

            using SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                apodList.Add(new ApodDto
                {
                    Date = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                    Title = reader.GetString(1),
                    Explanation = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Url = reader.GetString(3),
                    MediaType = reader.IsDBNull(4) ? "" : reader.GetString(4),
                    ServiceVersion = reader.IsDBNull(5) ? "" : reader.GetString(5)
                });
            }

            return apodList;
        }
    }
}
