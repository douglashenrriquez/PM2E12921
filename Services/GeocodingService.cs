using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PM0220242P.Services
{
    public class GeocodingService
    {
        private readonly string apiKey;

        public GeocodingService(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<(double latitude, double longitude)> GetCoordinatesAsync(string address)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var encodedAddress = Uri.EscapeDataString(address);
                    var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<GeocodingResponse>(json);

                        if (result?.results?.Length > 0)
                        {
                            var location = result.results[0].geometry.location;
                            return (location.lat, location.lng);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error calling Google Maps API: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting coordinates: {ex.Message}");
            }

            return (0.0, 0.0); // Default value in case of error
        }

        private class GeocodingResponse
        {
            public GeocodingResult[] results { get; set; }
        }

        private class GeocodingResult
        {
            public Geometry geometry { get; set; }
        }

        private class Geometry
        {
            public Location location { get; set; }
        }

        private class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
    }
}
