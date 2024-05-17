using ReferMe.ViewModels;

namespace ReferMe.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}