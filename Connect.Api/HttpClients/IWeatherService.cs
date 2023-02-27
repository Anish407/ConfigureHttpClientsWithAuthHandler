namespace Connect.Api.HttpClients
{
    public interface IWeatherService
    {
        Task<T> GetWeatherForecasts<T>()
           where T : class;
    }
}