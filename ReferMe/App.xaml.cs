﻿namespace ReferMe;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        if (Preferences.ContainsKey("Token"))
            Shell.Current.GoToAsync(nameof(ReferMe.MainPage));
        base.OnStart();
    }

    protected override void OnResume()
    {
        if (Preferences.ContainsKey("Token"))
            Shell.Current.GoToAsync(nameof(ReferMe.MainPage));
        base.OnResume();
    }
}