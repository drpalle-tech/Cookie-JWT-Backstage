using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using UnderTheHoodAuth.DTO;
using UnderTheHoodAuth.Models;

namespace UnderTheHoodAuth.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Admin()
        {
            var tokenInSession = HttpContext.Session.GetString("access_token");
            var token = new JwtToken();

            //If not in session, call web api
            if (string.IsNullOrEmpty(tokenInSession)) {
                token = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient("DpWebAPI");

            //Call session, see if it has token
            token = JsonConvert.DeserializeObject<JwtToken>(tokenInSession ?? string.Empty) ?? new JwtToken();

            //If the session's token expires, call web api for fresh token.
            if (token == null || token.ExpiresAt <= DateTime.UtcNow || string.IsNullOrEmpty(token.AccessToken))
            {
                token = await Authenticate();
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);

            //Throws 401 if we do not include JWT token in the http client header
            var _forecasts = httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast").Result ?? new List<WeatherForecastDTO>();
            return View(new AdminViewModel { WeatherForecasts = _forecasts});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<JwtToken> Authenticate()
        {
            //Create client so we can access API project
            var httpClient = _httpClientFactory.CreateClient("DpWebAPI");

            //Create JWT token to include in the httpclient header before calling for weather forecasts.
            var res = await httpClient.PostAsJsonAsync("auth", new Credential { UserName = "s", Password = "s" });

            res.EnsureSuccessStatusCode();

            string jwt = await res.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", jwt);

            return JsonConvert.DeserializeObject<JwtToken>(jwt);
        }
    }

    public class Credential
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
