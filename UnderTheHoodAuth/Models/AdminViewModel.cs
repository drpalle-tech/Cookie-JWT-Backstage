using UnderTheHoodAuth.DTO;

namespace UnderTheHoodAuth.Models
{
    public class AdminViewModel
    {
        public List<WeatherForecastDTO> WeatherForecasts { get; set; } = new List<WeatherForecastDTO>();
    }
}
