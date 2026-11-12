
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Http.Json;
using NasaApodGallery.Models;

namespace NasaApodGallery.Services
{
    public class NasaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NasaApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Nasa:ApiKey"] ?? "DEMO_KEY";
        }

        public async Task<List<ApodDto>> GetApodDataAsync(
            DateTime startDate,
            DateTime endDate)
        {
            string start = startDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            string end = endDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            string url =
                "https://api.nasa.gov/planetary/apod" +
                $"?start_date={start}" +
                $"&end_date={end}" +
                $"&api_key={Uri.EscapeDataString(_apiKey)}";

            var response =
                await _httpClient.GetFromJsonAsync<List<ApodDto>>(url);

            if (response == null)
            {
                return new List<ApodDto>();
            }

            foreach (var apod in response)
            {
                // Remove HTML tags from explanation
                string decodedText = WebUtility.HtmlDecode(apod.Explanation ?? string.Empty);
                string cleanText = Regex.Replace(
                    decodedText,
                    "<[^>]+>",
                    " "
                );

                apod.Explanation = Regex.Replace(cleanText, @"\s+", " ").Trim();
            }

            return response;
        }
    }
}
