using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using ReferMe.Models.Interaction;

namespace ReferMe.Services.Interactions;

internal sealed class RealTimeService(ILogger<RealTimeService> logger) : IRealTimeService
{
    private HubConnection _hubConnection;


    public async ValueTask<HubConnection> ConnectAsync()
    {
        ArgumentNullException.ThrowIfNull(_hubConnection, "NotHubFound");
        await _hubConnection.StartAsync();

        return _hubConnection;
    }

    public ValueTask<bool> BuildAsync(string accessToken)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken, "Authentication-Token-missing");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://192.168.43.177:45455/hubs/tracker",
                    options => { options.AccessTokenProvider = () => Task.FromResult(accessToken)!; })
                .WithAutomaticReconnect()
                .Build();

            return ValueTask.FromResult(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Something went wront!!");
            return ValueTask.FromResult(false);
        }
    }

    public async ValueTask SendTrackingRequestAsync(TrackingRequest request)
    {
        await _hubConnection.SendAsync("SendPositionRequestAsync", request);
    }

    public async ValueTask AcceptTrackingRequestAsync(Guid requestId)
    {
        await _hubConnection.SendAsync("AcceptRequestAsync", requestId);
    }

    public async ValueTask SendPositionUpdateAsync(Position position)
    {
        await _hubConnection.SendAsync("SendPositionAsync", position);
    }

    public async ValueTask StreamPositionUpdateAsync(IAsyncEnumerable<Position> positions)
    {
        await _hubConnection.SendAsync("StreamPositionAsync", positions);
    }
}