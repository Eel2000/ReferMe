using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using ReferMe.Models;
using ReferMe.Models.Interaction;
using ReferMe.Services.Authentication;
using ReferMe.Services.Interactions;

namespace ReferMe.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly HubConnection _hubConnection;
    private readonly IRealTimeService _realTimeService;
    private readonly ILoginService _loginService;


    public MainPageViewModel(IRealTimeService realTimeService, ILoginService loginService)
    {
        _realTimeService = realTimeService;
        _loginService = loginService;

        var token = Preferences.Get("Token", String.Empty);
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://192.168.43.177:45455/hubs/tracker",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                    options.HttpMessageHandlerFactory = htttpHandlerConfig =>
                    {
                        var handler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };
                        htttpHandlerConfig = handler;

                        return htttpHandlerConfig;
                    };
                })
            .WithAutomaticReconnect()
            .Build();

        Task.Run(() => { Dispatcher.GetForCurrentThread()?.Dispatch(async () => await _hubConnection.StartAsync()); });

        InitializeAsync().SafeFireAndForget(exception =>
        {
            Shell.Current.DisplayAlert("INITIALIZATION",
                exception.Message + "\r\n" + exception?.InnerException?.Message + "\r\n" +
                exception?.InnerException?.StackTrace, "OK");
        });

        ListenAsync().SafeFireAndForget(exception =>
        {
            Shell.Current.DisplayAlert("LISTENING",
                exception.Message + "\r\n" + exception?.InnerException?.Message + "\r\n" +
                exception?.InnerException?.StackTrace, "OK");
        });
    }

    [ObservableProperty] private UserInfo? _userInfo;
    [ObservableProperty] private UserInfo? _selectedUserInfo;
    [ObservableProperty] private ObservableCollection<UserInfo> _userInfos;
    [ObservableProperty] private string? _connectionStatus;

    async ValueTask InitializeAsync()
    {
        IsBusy = true;
        if (!Preferences.ContainsKey("User"))
        {
            await Shell.Current.DisplayAlert("LOGIN", "You're session has expired", "OK");

            await Shell.Current.GoToAsync("..");

            return;
        }

        var token = Preferences.Get("Token", String.Empty);

        var infos = await _loginService.GetUserInformationsAsync(token);
        UserInfos = [..infos];

        var user = JsonConvert.DeserializeObject<UserInfo>(Preferences.Get("User", string.Empty));
        UserInfo = user;


        // _hubConnection = new HubConnectionBuilder()
        //     .WithUrl("https://192.168.43.177:45455/hubs/tracker",
        //         options => { options.AccessTokenProvider = () => Task.FromResult(token); })
        //     .WithAutomaticReconnect()
        //     .Build();
        //
        // _hubConnection.StartAsync();

        IsBusy = !IsBusy;
    }

    [RelayCommand]
    async Task RefreshData()
    {
        try
        {
            UserInfos = [];
            // Shell.Current.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(5), async () =>
            // {
            var token = Preferences.Get("Token", String.Empty);
            IsBusy = true;
            var infos = await _loginService.GetUserInformationsAsync(token);
            UserInfos = [..infos];
            IsBusy = false;
            // });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Shell.Current.DisplayAlert("LOADING",
                e.Message + "\r\n" + e?.InnerException?.Message + "\r\n" +
                e?.InnerException?.StackTrace, "OK");
        }
    }

    [RelayCommand]
    async Task RefreshConnection()
    {
        try
        {
            _hubConnection.StopAsync();

            // var token = Preferences.Get("Token", String.Empty);
            //
            // _hubConnection = new HubConnectionBuilder()
            //     .WithUrl("https://192.168.43.177:45455/hubs/tracker",
            //         options => { options.AccessTokenProvider = () => Task.FromResult(token)!; })
            //     .WithAutomaticReconnect()
            //     .Build();

            _hubConnection.StartAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await Shell.Current.DisplayAlert("CONNECTION",
                "Connection to the server failed. please force it by clicking the reconnect button below", "OK");
        }
    }

    [RelayCommand]
    async Task SendRequest()
    {
        try
        {
            if (UserInfo is null)
            {
                await Shell.Current.DisplayAlert("REQUEST", "You're not connected! please connect to send a request",
                    "OK");
                return;
            }

            if (SelectedUserInfo is null)
            {
                await Shell.Current.DisplayAlert("REQUEST",
                    "Please Ensure that you've selected a Friend to request to.",
                    "OK");
                return;
            }

            var request = new TrackingRequest
            {
                Message = $"{UserInfo.UserName} has request you to share your location",
                Receiver = SelectedUserInfo.UserId,
                Sender = UserInfo.UserId,
            };

            if (string.IsNullOrWhiteSpace(request.Sender) || string.IsNullOrWhiteSpace(request.Receiver))
            {
                await Shell.Current.DisplayAlert("REQUEST",
                    "Please Ensure that you're authenticated and you've selected a Friend to request to.",
                    "OK");
                return;
            }

            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("SendPositionRequestAsync", request).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await Shell.Current.DisplayAlert("LISTENING",
                e.Message + "\r\n" + e?.InnerException?.Message + "\r\n" +
                e?.InnerException?.StackTrace, "OK");
        }
    }

    async ValueTask ListenAsync()
    {
        if (_hubConnection is null)
        {
            Shell.Current.DisplayAlert("LISTENING", "Connection not established please refresh it.", "OK");
            return;
        }

        _hubConnection!.Reconnecting += HubConnectionOnReconnecting;

        _hubConnection!.Reconnected += OnReconnected;

        _hubConnection.On<string>("RequestPositionAsync", OnRequestPositionAsync);

        _hubConnection.On<bool, string>("RequestSentAsync", OnRequestSentAsync);

        _hubConnection.On<bool, string>("AcceptanceFailedAsync", OnRequestSentAsync);

        _hubConnection.On<bool, string>("RequestAcceptedAsync", OnRequestSentAsync);

        _hubConnection.On<Guid>("RequestAcceptedAsync", OnRequestAcceptedAsync);

        _hubConnection.On<Position>("ReceivePositionUpdatesAsync", OnReceivePositionUpdatesAsync);
    }

    private async Task OnReceivePositionUpdatesAsync(Position arg)
    {
        await Shell.Current.DisplayAlert("POSITION UPDATES",
            $"New location at Long {arg.Longitute} and Lat {arg.Longitute}", "OK");
    }

    private async Task OnRequestAcceptedAsync(Guid arg)
    {
        await Shell.Current.DisplayAlert("REQUEST", $"You're sharing group has been created it ID is {arg}", "OK");
    }

    private async Task OnRequestSentAsync(bool arg1, string arg2)
    {
        await Shell.Current.DisplayAlert("REQUEST", arg2, "OK");
    }

    private async Task OnRequestPositionAsync(string arg)
    {
        await Shell.Current.DisplayAlert("REQUEST", arg, "OK");
        //TODO Load the requests from the api and display theme
    }

    private Task OnReconnected(string? arg)
    {
        ConnectionStatus = _hubConnection.State.ToString().ToUpper() + "...";
        return Task.CompletedTask;
    }

    private Task HubConnectionOnReconnecting(Exception? arg)
    {
        ConnectionStatus = _hubConnection.State.ToString().ToUpper() + "...";
        return Task.CompletedTask;
    }
}