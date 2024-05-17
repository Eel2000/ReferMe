using Microsoft.AspNetCore.SignalR.Client;
using ReferMe.Models.Interaction;

namespace ReferMe.Services.Interactions;

public interface IRealTimeService
{
    /// <summary>
    /// Start the hub connection
    /// </summary>
    /// <returns><see cref="HubConnection"/> the connection object to be manipulated</returns>
    ValueTask<HubConnection> ConnectAsync();

    /// <summary>
    /// Build a hub authenticated connection to the server
    /// </summary>
    /// <param name="accessToken">The BEARER access token of the current user.</param>
    /// <returns><see cref="bool"/> <code>True</code> if the hub connection has been belt otherwise <code>False</code></returns>
    ValueTask<bool> BuildAsync(string accessToken);

    /// <summary>
    /// Send a request to a specified client(user) to ask for location sharing.
    /// </summary>
    /// <param name="request">The request object <see cref="TrackingRequest"/></param>
    /// <returns><see cref="ValueTask"/> Completed Value Task.</returns>
    ValueTask SendTrackingRequestAsync(TrackingRequest request);


    /// <summary>
    /// Accept the location sharing and notify the send(requester)
    /// </summary>
    /// <param name="requestId"><see cref="Guid"/> the ID of the request recieved.</param>
    /// <returns><see cref="ValueTask"/> Completed Value Task.</returns>
    ValueTask AcceptTrackingRequestAsync(Guid requestId);


    /// <summary>
    /// Send the position the requester or friend.
    /// </summary>
    /// <param name="position"><see cref="Position"/> The position object</param>
    /// <returns><see cref="ValueTask"/> Completed ValueTask.</returns>
    ValueTask SendPositionUpdateAsync(Position position);

    /// <summary>
    /// Stream the position the requester or friend.
    /// </summary>
    /// <param name="positions"><see cref="IAsyncEnumerable{T}"/> position streamed where T is <see cref="Position"/> object.</param>
    /// <returns><see cref="ValueTask"/> Completed ValueTask.</returns>
    ValueTask StreamPositionUpdateAsync(IAsyncEnumerable<Position> positions);
}