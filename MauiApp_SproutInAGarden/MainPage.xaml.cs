namespace MauiApp_SproutInAGarden;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}
    private async void OnNavigateClicked(object sender, EventArgs e)
    {
        string userName = NameEntry.Text;

        if (string.IsNullOrWhiteSpace(userName))
        {
            await DisplayAlert("Внимание", "Пожалуйста, введите ваше имя!", "ОК");
            return;
        }
        await DisplayAlert("Внимание", $"вы хотите продоллжить как {Uri.EscapeDataString(userName)}!", "ОК");

        await Shell.Current.GoToAsync($"//menu?username={Uri.EscapeDataString(userName)}");
    }
}