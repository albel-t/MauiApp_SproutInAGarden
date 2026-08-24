namespace MauiApp_SproutInAGarden;

public partial class GameBoard : ContentPage, IQueryAttributable
{
    private string _currentUserName = string.Empty;

    public GameBoard()
	{
		InitializeComponent();
	}
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("username", out var usernameObj) && usernameObj is string username)
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Привет", $"Добро пожаловать, {_currentUserName}!", "OK");
            });
        }
    }
}