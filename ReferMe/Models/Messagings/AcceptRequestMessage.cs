using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ReferMe.Models.Messagings;

public class AcceptRequestMessage : ValueChangedMessage<AcceptRequest>
{
    public AcceptRequestMessage(AcceptRequest value) : base(value)
    {
    }
}