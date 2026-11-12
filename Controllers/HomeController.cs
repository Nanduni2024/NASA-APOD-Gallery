
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;
using NasaApodGallery.Models;
using NasaApodGallery.Services;

namespace NasaApodGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly NasaApiService _nasaApiService;
        private readonly ApodRepository _apodRepository;

        public HomeController(
            NasaApiService nasaApiService,
            ApodRepository apodRepository)
        {
            _nasaApiService = nasaApiService;
            _apodRepository = apodRepository;
        }

        // Display saved APOD data
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apodList =
                await _apodRepository.GetAllApodDataAsync();

            foreach (var apod in apodList)
            {
                string decodedExplanation = WebUtility.HtmlDecode(apod.Explanation ?? string.Empty);
                apod.Explanation = Regex.Replace(
                    Regex.Replace(decodedExplanation, "<[^>]*>", " "),
                    @"\s+",
                    " "
                ).Trim();
            }

            return View(apodList);
        }

        // Fetch NASA data and save it into database
        [HttpPost]
        public async Task<IActionResult> FetchApod(
            DateTime startDate,
            DateTime endDate)
        {
            if (startDate > endDate)
            {
                TempData["Error"] =
                    "Start date cannot be after end date.";

                return RedirectToAction("Index");
            }

            if (endDate.Date > DateTime.Today)
            {
                TempData["Error"] =
                    "Future dates are not allowed.";

                return RedirectToAction("Index");
            }

            try
            {
                var apodList =
                    await _nasaApiService.GetApodDataAsync(
                        startDate,
                        endDate
                    );

                await _apodRepository.SaveApodDataAsync(apodList);

                TempData["Message"] =
                    $"{apodList.Count} APOD records fetched successfully.";
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to fetch NASA data. Please check your API key and connection.";
            }

            return RedirectToAction("Index");
        }
    }
}
