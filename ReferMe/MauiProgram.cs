using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ReferMe.Services.Authentication;
using ReferMe.Services.Interactions;
using ReferMe.Services.Tracking;
using ReferMe.ViewModels;
using ReferMe.Views;

namespace ReferMe;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginPageViewModel>();

        builder.Services.AddScoped<MainPage>();
        builder.Services.AddTransient<MainPageViewModel>();

        builder.Services.AddTransient<RequestsPage>();
        builder.Services.AddTransient<RequestPageViewModel>();

        builder.Services.AddHttpClient<ILoginService, LoginService>(ops =>
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            ops = new(handler);
        });
        builder.Services.AddHttpClient<ITrackingService, TrackingService>();

        builder.Services.AddSingleton<IRealTimeService, RealTimeService>();

        return builder.Build();
    }
}