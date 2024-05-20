using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ReferMe.Models;
using ReferMe.Models.Interaction;

namespace ReferMe.Services.Tracking;

internal sealed class TrackingService(HttpClient client) : ITrackingService
{
    public async ValueTask<IEnumerable<TrackRequest>> GetIncomingRequestAsync()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);

            var reponse =
                await client.GetFromJsonAsync<Response<IEnumerable<TrackRequest>>>(
                    "https://192.168.43.177:45455/api/Tracking/incoming-requests");

            return reponse.Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Enumerable.Empty<TrackRequest>();
        }
    }

    public async ValueTask<IEnumerable<TrackRequest>> GetOutGoingRequestAsync()
    {
        try
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            client = new HttpClient(handler);

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