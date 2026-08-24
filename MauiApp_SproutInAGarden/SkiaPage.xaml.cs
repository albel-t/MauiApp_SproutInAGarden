using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace MauiApp_SproutInAGarden;

public partial class SkiaPage : ContentPage
{
    public SkiaPage()
    {
        InitializeComponent();
    }

    private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        // Белый фон
        canvas.Clear(SKColors.White);

        // Синий круг
        using (var paint = new SKPaint())
        {
            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.Green;
            paint.IsAntialias = true;
            canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);
        }

        // Чёрный текст
        using (var paint = new SKPaint())
        using (var font = new SKFont())
        {
            paint.Color = SKColors.Black;
            paint.IsAntialias = true;
            font.Size = 30;

            string text = "Hello SkiaSharp!";
            canvas.DrawText(text, info.Width / 2 - 80, info.Height / 2 + 10, font, paint);
        }
    }
}