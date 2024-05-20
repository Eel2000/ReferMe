using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ReferMe.Models.Interaction;
using ReferMe.Models.Messagings;
using ReferMe.Services.Tracking;

namespace ReferMe.ViewModels;

public partial class RequestPageViewModel : BaseViewModel
{
    private readonly ITrackingService _trackingService;


    [ObservableProperty] private ObservableCollection<TrackRequest> _incomingRequests;

    // [ObservableProperty] private ObservableCollection<TrackRequest> _outGoingRequests;

    public RequestPageViewModel(ITrackingService trackingService)
    {
        _trackingService = trackingService;

        InitializeAsync().SafeFireAndForget(exception =>
        {
            Shell.Current.DisplayAlert("INITIALIZATION",
                exception.Message + "\r\n" + exception?.InnerException?.Message + "\r\n" +
                exception?.InnerException?.StackTrace, "OK");
        });
    }


    async Task InitializeAsync()
    {
        var token = Preferences.Get("Token", String.Empty);

        var incomingRequests = await _trackingService.GetIncomingRequestAsync(token).ConfigureAwait(false);
        IncomingRequests = [..incomingRequests];

        // var outgoingRequests = await _trackingService.GetOutGoingRequestAsync(token).ConfigureAwait(false);
        // OutGoingRequests = [..outgoingRequests];
    }

    [RelayCommand]
    async Task Refresh()
    {
        try
        {
            IsBusy = true;

            var token = Preferences.Get("Token", String.Empty);

            var incomingRequests = await _trackingService.GetIncomingRequestAsync(token);
            IncomingRequests = [..incomingRequests];

            IsBusy = !IsBusy;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Shell.Current.DisplayAlert("INITIALIZATION",
                e.Message + "\r\n" + e?.InnerException?.Message + "\r\n" +
                e?.InnerException?.StackTrace, "OK");
        }
    }

    [RelayCommand]
    async Task RequestSelected(TrackRequest? request)
    {
        try
        {
            if (request is null) return;

            var hasAccept =
                await Shell.Current.DisplayAlert("REQUEST", "Would you like to accept the request??", "yes", "No");

            if (!hasAccept) return;

            WeakReferenceMessenger.Default.Send(new AcceptRequestMessage(new AcceptRequest(request.Id)));

            await Shell.Current.DisplayAlert("REQUEST",
                "Invitation accept, your position will now be shared with your guest.", "Ok");
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Shell.Current.DisplayAlert("REQUEST",
                exception.Message + "\r\n" + exception?.InnerException?.Message + "\r\n" +
                exception?.InnerException?.StackTrace, "OK");
        }
    }
}