using Microsoft.Maui.Controls;

namespace MauiApp_SproutInAGarden;

// Обязательно указываем интерфейс IQueryAttributable
public partial class MainMenu : ContentPage, IQueryAttributable
{
    // Переменная для хранения имени внутри класса (если понадобится позже)
    private string _currentUserName = string.Empty;

    public MainMenu()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("username", out var usernameObj) && usernameObj is string username)
        {
            // Декодируем спецсимволы и пробелы
            _currentUserName = Uri.UnescapeDataString(username);

            // Показываем приветствие на основном потоке
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Привет", $"Добро пожаловать, {_currentUserName}!", "OK");
            });
        }
    }

    private async void OnPlayClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//game?username={Uri.EscapeDataString(_currentUserName)}");

    }
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        // Возврат на главную страницу через абсолютный роут из вашего AppShell
        //await Shell.Current.GoToAsync("///main");
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        // Возврат на главную страницу через абсолютный роут из вашего AppShell
        await Shell.Current.GoToAsync("///main");
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Оставляем ваше уведомление о загрузке, но без опасного разбора строки URL
        DisplayAlert("загрузка", "загрузка прошла успешно", "OK");
    }
}
