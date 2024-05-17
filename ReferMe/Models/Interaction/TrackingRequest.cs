namespace ReferMe.Models.Interaction;

public record TrackingRequest
{
    /// <summary>
    /// User who want to be referred to.
    /// </summary>
    public string? Sender { get; init; }

    /// <summary>
    /// The reference user.
    /// </summary>
    public string? Receiver { get; init; }

    /// <summary>
    /// the topic of the request.
    /// </summary>
    public string? Message { get; init; }
}