namespace ReferMe.Models;

public sealed record Response<TData> where TData : class
{
    public bool Status { get; init; }
    public string? Message { get; init; }
    public TData? Data { get; init; }
}