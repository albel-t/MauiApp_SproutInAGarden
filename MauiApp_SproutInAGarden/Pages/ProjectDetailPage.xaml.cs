using MauiApp_SproutInAGarden.Models;

namespace MauiApp_SproutInAGarden.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }
}
