using ReferMe.ViewModels;

namespace ReferMe;

public partial class MainPage : ContentPage
{

    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    
}