using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using ReferMe.Models;
using ReferMe.Services.Authentication;
using ReferMe.Views;

namespace ReferMe.ViewModels;

public partial class LoginPageViewModel(ILoginService loginService) : BaseViewModel
{
    [ObservableProperty] private string _userName;

    [ObservableProperty] private string _password;

    [RelayCommand]
    async Task Login()
    {
        try
        {
            if (await Permissions.RequestAsync<Permissions.LocationWhenInUse>() is PermissionStatus.Denied
                or PermissionStatus.Disabled)
            {
                await Shell.Current.DisplayAlert("LOCATION", "Please Allow or activate your location and retry.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("VALIDATIONS", "Please enter your credentials.", "OK");
                return;
            }

            var loginResponse =
                await loginService.LoginAsync(new LoginRequest { Username = UserName, Password = Password });
            if (loginResponse is not null && loginResponse.Status is true)
            {
                await Shell.Current.DisplayAlert("LOGIN",
                    $"You're logged in and the session will expire in {TimeSpan.FromSeconds(loginResponse.ExpiresIn).TotalHours} Hours",
                    "OK");

                Preferences.Set("Token", loginResponse.Data);

                var info = await loginService.GetInformationsAsync(Preferences.Get("Token", String.Empty));

                await Shell.Current.DisplayAlert("LOGIN",
                    $"You're connected as {info.User.UserName}",
                    "OK");

                if (info!.IsConnected)
                {
                    Preferences.Set("User", JsonConvert.SerializeObject(info?.User));
                    await Shell.Current.GoToAsync("///" + nameof(MainPage), true);
                }

                return;
            }

            await Shell.Current.DisplayAlert("LOGIN_FAILED", "Invalid Credentials", "OK");
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            await Shell.Current.DisplayAlert("ERROR", e.Message, "OK");
        }
    }
}