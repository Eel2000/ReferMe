using System.Collections.Generic;
using System.Threading.Tasks;
using ReferMe.Models.Interaction;

namespace ReferMe.Services.Tracking;

public interface ITrackingService
{
    ValueTask<IEnumerable<TrackRequest>> GetIncomingRequestAsync(string token = "");
    ValueTask<IEnumerable<TrackRequest>> GetOutGoingRequestAsync(string token = "");
}