using MauiApp_SproutInAGarden.Models;
using MauiApp_SproutInAGarden.PageModels;

namespace MauiApp_SproutInAGarden.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}