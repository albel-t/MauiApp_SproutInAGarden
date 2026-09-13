using SkiaSharp;

namespace MauiApp_SproutInAGarden.GameObjects;

public class BaseGameObject
{        
    public float X { get; set; }
    public float Y { get; set; }

    public float Angle { get; set; } = 0;

    public float Size { get; set; } = 1;

    public string texture { get; set; }

    public SKColor Bacground { get; set; } = SKColors.Blue;


    public BaseGameObject(int x, int y, string texture_ = "tomato_example")
	{
        X = x;
        Y = y;
        texture = texture_;

    }
    /*
    public void Draw(SKCanvas canvas)
    {
        Texture _texture = TextureManager.GetTexture(texture);

        canvas.Save();

        canvas.Translate(X + _texture.width / 2, Y + _texture.height / 2);
        canvas.RotateDegrees(Angle);
        canvas.Translate(-_texture.width / 2, -_texture.height / 2);

        using (var paint = new SKPaint())
        {
            paint.Color = Bacground;
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = true;
            canvas.DrawRect(0, 0, _texture.width, _texture.height, paint);
        }



        canvas.Save();
        canvas.Translate(X, Y);
        canvas.RotateDegrees(Angle);
        canvas.Scale(Size, Size);

        canvas.DrawBitmap(_texture.img, 0, 0);

        canvas.Restore();


    }
    */

    public void Draw(SKCanvas canvas)
    {
        Texture _texture = TextureManager.GetTexture(texture);

        if (_texture == null || _texture.img == null)
        {
            return;
        }

        canvas.Save();

        canvas.Translate(X, Y);

        canvas.RotateDegrees(Angle);

        canvas.Scale(Size, Size);

        float halfWidth = _texture.width / 2f;
        float halfHeight = _texture.height / 2f;

        using (var paint = new SKPaint())
        {
            paint.Color = Bacground; 
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = true;
            canvas.DrawRect(-halfWidth, -halfHeight, _texture.width, _texture.height, paint);
        }

        canvas.DrawBitmap(_texture.img, -halfWidth, -halfHeight);

        canvas.Restore();
    }

};