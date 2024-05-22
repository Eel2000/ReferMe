using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ReferMe.ViewModels;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace ReferMe.Views;

public partial class MapPage : ContentPage
{
    public MapPage(MapPageViewModel viewModel)
    {
        BindingContext = viewModel;

        InitializeComponent();

        InitializeMap().SafeFireAndForget();
    }


    async Task InitializeMap()
    {
        var currentLocation = await Geolocation.GetLastKnownLocationAsync();
        var lushiPin = new Pin
        {
            Label = "Lubumbashi",
            Location = currentLocation,
            Address = "Republic democratic of Congo"
        };

        Map.Pins.Add(lushiPin);

        Map.MoveToRegion(new MapSpan(currentLocation, 0.01, 0.01));
    }
}