using System;

namespace ReferMe.Models.Interaction;

public class TrackRequest
{
    public Guid Id { get; set; }
    public string? Sender { get; set; }
    public string? SenderName { get; set; }
    public string? Receiver { get; set; }
    public string? Message { get; set; }
    public bool Accepted { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public DateTimeOffset AcceptedDate { get; set; }
}