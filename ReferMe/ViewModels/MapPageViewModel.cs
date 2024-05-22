using System.Collections.ObjectModel;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Maps;

namespace ReferMe.ViewModels;

public partial class MapPageViewModel : BaseViewModel
{
    [ObservableProperty] private ObservableCollection<Pin> _pins;


    public MapPageViewModel()
    {
        GetCurrentLocationAsync().SafeFireAndForget(exception =>
        {
            Shell.Current.DisplayAlert("LOCATION", exception.Message, "Ok");
        });
    }

    async Task GetCurrentLocationAsync()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

            Location? location = await Geolocation.Default.GetLocationAsync(request);
            if (location is not null)
            {
                Pin currentUserLocation = new Pin
                {
                    Address = "My Location",
                    Label = "Current location",
                    Location = location,
                    Type = PinType.SavedPin
                };
                Pins = [currentUserLocation];
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}