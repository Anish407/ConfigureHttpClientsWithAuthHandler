﻿using System.Text.Json;

namespace Connect.Api.HttpClients
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient client, ILogger<WeatherService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<T> GetWeatherForecasts<T>()
            where  T: class
        {

            using var request = new HttpRequestMessage(HttpMethod.Get, "GetWeatherForecast");

            var response = await _client.SendAsync(request,HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Call Failed");
                throw new Exception($"Status Code: {response.StatusCode}, Call failed");
            }

            var result = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(result))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(result) ?? throw new Exception("Error While deserializing");
        }
    }
}
