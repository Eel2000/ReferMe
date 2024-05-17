using CommunityToolkit.Mvvm.ComponentModel;

namespace ReferMe.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] private bool _isBusy;
}