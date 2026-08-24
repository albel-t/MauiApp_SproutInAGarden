using Java.Lang.Annotation;
using SkiaSharp;

namespace MauiApp_SproutInAGarden.GameObjects;

public class BaseGameObject : ContentPage
{        
    public float X { get; set; }
    public float Y { get; set; }

    public float Angle { get; set; } = 0;

    public float Size { get; set; } = 1;

    public float texture { get; set; }

    public SKColor Bacground { get; set; } = SKColors.Blue;

    public BaseGameObject()
	{

    }

    public void Draw(SKCanvas canvas)
    {

        canvas.Save();

        // Поворачиваем холст вокруг центра объекта
        canvas.Translate(X + Width / 2, Y + Height / 2);
        canvas.RotateDegrees(Angle);
        canvas.Translate(-Width / 2, -Height / 2);

        // Рисуем прямоугольник (текстура)
        using (var paint = new SKPaint())
        {
            paint.Color = Bacground;
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = true;
            canvas.DrawRect(0, 0, Width, Height, paint);
        }


        }

        canvas.Restore();
    }
};