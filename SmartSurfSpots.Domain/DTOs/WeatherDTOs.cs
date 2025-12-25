namespace SmartSurfSpots.Domain.DTOs
{
    /// <summary>
    /// Resposta combinada com dados meteorológicos e de mar para um Spot.
    /// </summary>
    public class WeatherDto
    {
        public string SpotName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
        /// Dados meteorológicos no momento atual.
        /// </summary>
        public CurrentWeather Current { get; set; }

        /// <summary>
        /// Previsão horária para as próximas 24h.
        /// </summary>
        public List<HourlyForecast> Forecast { get; set; }
    }

    /// <summary>
    /// Condições atuais simplificadas.
    /// </summary>
    public class CurrentWeather
    {
        /// <summary>
        /// Temperatura em graus Celsius (°C).
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Velocidade do vento em km/h.
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Direção do vento em graus (0° = Norte, 90° = Este).
        /// </summary>
        public double WindDirection { get; set; }

        /// <summary>
        /// Código WMO do estado do tempo (ex: 0 = Limpo, 61 = Chuva).
        /// </summary>
        public int WeatherCode { get; set; }

        /// <summary>
        /// Descrição textual do tempo (ex: "Céu limpo").
        /// </summary>
        public string WeatherDescription { get; set; }
    }

    /// <summary>
    /// Previsão detalhada para uma hora específica.
    /// </summary>
    public class HourlyForecast
    {
        /// <summary>
        /// Hora da previsão (formato ISO 8601).
        /// </summary>
        public string Time { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }

        /// <summary>
        /// Altura das ondas em metros.
        /// </summary>
        public double WaveHeight { get; set; }

        /// <summary>
        /// Período entre ondas em segundos (s).
        /// </summary>
        public double WavePeriod { get; set; }

        /// <summary>
        /// Direção das ondas (ex: "NW", "W").
        /// </summary>
        public string WaveDirection { get; set; }
    }

    // ==========================================================
    // CLASSES INTERNAS (Mapeamento da API Open-Meteo)
    // Estas classes não aparecem no Swagger se não forem retornadas diretamente pelos Controllers
    // ==========================================================

    public class OpenMeteoResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CurrentWeatherData Current { get; set; }
        public HourlyWeatherData Hourly { get; set; }
    }

    public class CurrentWeatherData
    {
        public string Time { get; set; }
        public double Temperature_2m { get; set; }
        public double Wind_speed_10m { get; set; }
        public double Wind_direction_10m { get; set; }
        public int Weather_code { get; set; }
    }

    public class HourlyWeatherData
    {
        public List<string> Time { get; set; }
        public List<double> Temperature_2m { get; set; }
        public List<double> Wind_speed_10m { get; set; }
        public List<double> Wind_direction_10m { get; set; }
    }

    public class OpenMeteoMarineResponse
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public HourlyMarineData Hourly { get; set; }
    }

    public class HourlyMarineData
    {
        public List<string> Time { get; set; }
        public List<double> Wave_height { get; set; }
        public List<double> Wave_period { get; set; }
        public List<double> Wave_direction { get; set; }
    }
}