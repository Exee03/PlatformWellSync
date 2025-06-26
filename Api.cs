using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net.Http.Headers;

public class Api
{
    protected string baseUrl = Environment.GetEnvironmentVariable("API_ENDPOINT") ?? "";
    protected HttpClient client = new HttpClient();

    public async Task<bool> login(string username, string password)
    {
        var payload = new {
            username = username,
            password = password
        };
        var response = await client.PostAsync(
            $"{baseUrl}/Account/Login",
            new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode) return false;
        
        var content = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<string>(content);
        
        if (string.IsNullOrEmpty(token)) return false;
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return true;
    }

    public async Task<List<Platform>> getPlatforms() {
        var endpoint = $"{baseUrl}/PlatformWell/GetPlatformWellActual";
        // var endpoint = $"{baseUrl}/PlatformWell/GetPlatformWellDummy";

        var response = await client.GetAsync(endpoint);

        if (!response.IsSuccessStatusCode) return new List<Platform>();

        var content = await response.Content.ReadAsStringAsync();
        var platforms = new List<Platform>();

        try {
            platforms = JsonConvert.DeserializeObject<List<Platform>>(content);
            return platforms ?? new List<Platform>();
        } catch (JsonException e) {
            Console.WriteLine(e.Message);
            var jsonArray = JArray.Parse(content);

            foreach (var platformJson in jsonArray)
            {
                var platform = new Platform();
                
                platform.Id = platformJson["Id"]?.Value<int>() ?? 0;
                platform.UniqueName = platformJson["UniqueName"]?.Value<string>() ?? "";
                platform.Latitude = platformJson["Latitude"]?.Value<double>() ?? 0.0;
                platform.Longitude = platformJson["Longitude"]?.Value<double>() ?? 0.0;
                platform.CreatedAt = platformJson["CreatedAt"]?.Value<DateTime?>();
                platform.UpdatedAt = platformJson["UpdatedAt"]?.Value<DateTime?>();

                var wellsArray = platformJson["Well"] as JArray;
                if (wellsArray != null)
                {
                    foreach (var wellJson in wellsArray)
                    {
                        var well = new Well();
                        well.Id = wellJson["Id"]?.Value<int>() ?? 0;
                        well.PlatformId = wellJson["PlatformId"]?.Value<int>() ?? 0;
                        well.UniqueName = wellJson["UniqueName"]?.Value<string>() ?? "";
                        well.Latitude = wellJson["Latitude"]?.Value<double>() ?? 0.0;
                        well.Longitude = wellJson["Longitude"]?.Value<double>() ?? 0.0;
                        well.CreatedAt = wellJson["CreatedAt"]?.Value<DateTime?>();
                        well.UpdatedAt = wellJson["UpdatedAt"]?.Value<DateTime?>();
                        
                        platform.Well.Add(well);
                    }
                }

                platforms?.Add(platform);
            }
            return platforms ?? new List<Platform>();
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            return platforms ?? new List<Platform>();
        }
    }
}

