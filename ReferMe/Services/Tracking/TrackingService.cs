using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using ReferMe.Models;
using ReferMe.Models.Interaction;

namespace ReferMe.Services.Tracking;

internal sealed class TrackingService(HttpClient client) : ITrackingService
{
    public async ValueTask<IEnumerable<TrackRequest>> GetIncomingRequestAsync(string token = "")
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var reponse = await client.GetAsync("https://192.168.11.111:45455/api/Tracking/incoming-requests");
            reponse.EnsureSuccessStatusCode();

            var jsonResponse = await reponse.Content.ReadAsStringAsync();

            var typedResponse = JsonConvert.DeserializeObject<Response<IEnumerable<TrackRequest>>>(jsonResponse);

            return typedResponse.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Enumerable.Empty<TrackRequest>();
        }
    }

    public async ValueTask<IEnumerable<TrackRequest>> GetOutGoingRequestAsync(string token = "")
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var reponse =
                await client.GetFromJsonAsync<Response<IEnumerable<TrackRequest>>>(
                    "https://192.168.11.111:45455/api/Tracking/outgoing-requests");

            return reponse.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Enumerable.Empty<TrackRequest>();
        }
    }
}