using SmartSurfSpots.Data.Repositories;
using SmartSurfSpots.Domain.DTOs;
using SmartSurfSpots.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartSurfSpots.Services.Implementations
{
    public class WeatherService : IWeatherService
    {
        private readonly ISpotRepository _spotRepository;
        private readonly HttpClient _httpClient;

        public WeatherService(ISpotRepository spotRepository, HttpClient httpClient)
        {
            _spotRepository = spotRepository;
            _httpClient = httpClient;
        }

        public async Task<WeatherDto> GetWeatherForSpotAsync(int spotId)
        {
            var spot = await _spotRepository.GetByIdAsync(spotId);
            if (spot == null) throw new Exception("Spot not found");

            // InvariantCulture é CRUCIAL aqui:
            // Garante que a latitude 10.5 seja "10.5" (formato API) e não "10,5" (formato PT)
            var lat = spot.Latitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
            var lon = spot.Longitude.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);

            // 1. Obter dados Meteorológicos (Vento, Temp)
            var weatherUrl = $"https://api.open-meteo.com/v1/forecast" +
                           $"?latitude={lat}&longitude={lon}" +
                           $"&current=temperature_2m,wind_speed_10m,wind_direction_10m,weather_code" +
                           $"&hourly=temperature_2m,wind_speed_10m,wind_direction_10m" +
                           $"&timezone=auto&forecast_days=3";

            string weatherResponse;
            try
            {
                weatherResponse = await _httpClient.GetStringAsync(weatherUrl);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao conectar com Open-Meteo: {ex.Message}");
            }

            var weatherData = JsonSerializer.Deserialize<OpenMeteoResponse>(weatherResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // 2. Obter dados Marinhos (Ondas)
            // Usamos try-catch separado porque alguns locais (terra firme) podem dar erro na API marinha,
            // mas queremos mostrar os dados de vento na mesma.
            var marineUrl = $"https://marine-api.open-meteo.com/v1/marine" +
                          $"?latitude={lat}&longitude={lon}" +
                          $"&hourly=wave_height,wave_period,wave_direction" +
                          $"&timezone=auto&forecast_days=3";

            OpenMeteoMarineResponse marineData = null;
            try
            {
                var marineResponse = await _httpClient.GetStringAsync(marineUrl);
                marineData = JsonSerializer.Deserialize<OpenMeteoMarineResponse>(marineResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch
            {
                // Graceful degradation: Se falhar (ex: spot não é no mar), continua sem ondas.
            }

            // 3. Mapear Current Weather
            var current = new CurrentWeather
            {
                Temperature = weatherData.Current.Temperature_2m,
                WindSpeed = weatherData.Current.Wind_speed_10m,
                WindDirection = weatherData.Current.Wind_direction_10m,
                WeatherCode = weatherData.Current.Weather_code,
                WeatherDescription = GetWeatherDescription(weatherData.Current.Weather_code)
            };

            // 4. Mapear Forecast (Merge de dados de Vento + Ondas)
            var forecast = new List<HourlyForecast>();

            // Math.Min evita IndexOutOfRange se as APIs retornarem tamanhos diferentes
            int count = weatherData.Hourly.Time.Count;
            if (marineData?.Hourly?.Time != null) count = Math.Min(count, marineData.Hourly.Time.Count);

            // Limitamos a 24 horas para não sobrecarregar o frontend
            for (int i = 0; i < Math.Min(24, count); i++)
            {
                var hourlyForecast = new HourlyForecast
                {
                    Time = weatherData.Hourly.Time[i],
                    Temperature = weatherData.Hourly.Temperature_2m[i],
                    WindSpeed = weatherData.Hourly.Wind_speed_10m[i],
                    WindDirection = weatherData.Hourly.Wind_direction_10m[i],
                    // Operador ?. e ?? (Null Coalescing) trata o caso de não haver dados marinhos
                    WaveHeight = marineData?.Hourly?.Wave_height?[i] ?? 0,
                    WavePeriod = marineData?.Hourly?.Wave_period?[i] ?? 0,
                    WaveDirection = marineData?.Hourly?.Wave_direction?[i].ToString() ?? "N/A"
                };
                forecast.Add(hourlyForecast);
            }

            return new WeatherDto
            {
                SpotName = spot.Name,
                Latitude = spot.Latitude,
                Longitude = spot.Longitude,
                Current = current,
                Forecast = forecast
            };
        }

        private string GetWeatherDescription(int weatherCode)
        {
            return weatherCode switch
            {
                0 => "Céu limpo",
                1 => "Principalmente limpo",
                2 => "Parcialmente nublado",
                3 => "Nublado",
                45 => "Nevoeiro",
                48 => "Nevoeiro com geada",
                51 => "Chuvisco leve",
                53 => "Chuvisco moderado",
                55 => "Chuvisco intenso",
                61 => "Chuva leve",
                63 => "Chuva moderada",
                65 => "Chuva forte",
                71 => "Neve leve",
                73 => "Neve moderada",
                75 => "Neve forte",
                77 => "Granizo",
                80 => "Aguaceiros leves",
                81 => "Aguaceiros moderados",
                82 => "Aguaceiros violentos",
                95 => "Trovoada",
                96 => "Trovoada com granizo leve",
                99 => "Trovoada com granizo forte",
                _ => "Desconhecido"
            };
        }
    }
}