using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReferMe.ViewModels;

namespace ReferMe.Views;

public partial class RequestsPage : ContentPage
{
    public RequestsPage(RequestPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}