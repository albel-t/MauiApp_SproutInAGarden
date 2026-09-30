using MauiApp_SproutInAGarden.GameObjects;  // ← вот это
using MauiApp_SproutInAGarden.GameLogic;  // ← вот это
using SkiaSharp;
using SkiaSharp.Views.Maui;
using static System.Net.Mime.MediaTypeNames;

namespace MauiApp_SproutInAGarden;


public partial class GameBoard : ContentPage, IQueryAttributable
{
    private string _currentUserName = string.Empty;
    HashSet<BaseGameObject> test_obj = new HashSet<BaseGameObject>(); 
    private System.Timers.Timer _timer;
    private Plant CurrentPlant;// = new Plant();
    InputOutputStream stream = new FakeStream();
    public GameBoard()
	{
        
        TextureManager.Init();
        InitializeComponent();

        TagsList.Init();

        //test_obj.Add( new BaseGameObject(50, 200,  "tomato_bigstem_example"));
        //test_obj.Add( new BaseGameObject(250, 200, "tomato_smallstem_example"));
        //test_obj.Add( new BaseGameObject(450, 200, "tomato_fruit_example"));

        _timer = new System.Timers.Timer(16);
        _timer.Elapsed += (s, e) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                GameCanvas.InvalidateSurface(); // ← Перерисовка
            });
        };
        _timer.Start();
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
    public void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        // Получаем холст для рисования
        SKCanvas canvas = e.Surface.Canvas;

        // Размеры холста
        SKImageInfo info = e.Info;

        // Очищаем холст (фон)
        canvas.Clear(SKColors.Black);


        using (var paint = new SKPaint())
        using (var font = new SKFont())
        {
            paint.Color = SKColors.White;
            paint.IsAntialias = true;

            font.Size = 30;  // ✅ РАЗМЕР ЗАДАЁТСЯ В SKFont

            canvas.DrawText("Привет", 50, 50, font, paint);
        }

        //foreach (BaseGameObject test in test_obj
        //test.Draw(canvas);
        if(CurrentPlant != null)
        {
            CurrentPlant.drowPlant().drow(new Vec2d(500, 1200));
            CurrentPlant.drowPlant().drowFlash(canvas);

        }

    }

    private void OnActionButtonClicked(object sender, EventArgs e)
    {
        CurrentPlant = new Plant();

        CurrentPlant.connect(stream);

        CurrentPlant.firatTickPlant();


        for (int i = 0; i < 10; i++)
        {
            CurrentPlant.tickPlant();
        }
    }
}