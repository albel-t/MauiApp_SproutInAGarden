using SkiaSharp;

namespace MauiApp_SproutInAGarden.GameObjects;

public class BaseGameObject
{
    public Vec2d Start;

    public Vec2d F1;
    public Vec2d F2;

    public Vec2d F1_;
    public Vec2d F2_;

    public float Angle { get; set; } = 0;

    public float Size { get; set; } = 1;

    public string texture { get; set; }

    public SKColor Bacground { get; set; } = SKColors.Blue;


    public BaseGameObject(Vec2d f1, Vec2d f2, string texture_ = "27_24_27_165_tomato_bigstem_example")
    {
        F1 = f1;
        F2 = f2;
        texture = texture_;

        var tmp = TextureManager.GetTexture_f(texture);

        F1_ = new Vec2d(tmp[0], tmp[1]);
        F2_ = new Vec2d(tmp[2], tmp[3]);

    }


    public void Draw(SKCanvas canvas)
    {
        Fit();
        Texture _texture = TextureManager.GetTexture(texture);

        if (_texture == null || _texture.img == null)
        {
            return;
        }

        canvas.Save();

        // 1. Переходим в левый верхний угол текстуры в мире
        canvas.Translate((float)Start.x, (float)Start.y);

        // 2. Поворачиваем вокруг этого угла
        canvas.RotateDegrees(Angle);

        // 3. Масштабируем от этого же угла
        canvas.Scale(Size, Size);

        // 4. Рисуем текстуру так, чтобы её левый верхний угол был в (0,0)
        using (var paint = new SKPaint())
        {
            paint.Color = Bacground;
            paint.Style = SKPaintStyle.Fill;
            paint.IsAntialias = true;
            canvas.DrawRect(0, 0, _texture.width, _texture.height, paint);
        }

        canvas.DrawBitmap(_texture.img, 0, 0);

        canvas.Restore();
    }
    public void Fit()
    {
        // Локальный вектор между фокусами в системе текстуры (в пикселях)
        double localDx = F2_.x - F1_.x;
        double localDy = F2_.y - F1_.y;
        double localLen = Math.Sqrt(localDx * localDx + localDy * localDy);

        // Мировой вектор между целевыми точками
        double worldDx = F2.x - F1.x;
        double worldDy = F2.y - F1.y;
        double worldLen = Math.Sqrt(worldDx * worldDx + worldDy * worldDy);

        if (localLen == 0)
        {
            // Фокусы совпадают в текстуре — нельзя определить поворот/масштаб
            Size = 1;
            Angle = 0;
            Start = new Vec2d(F1.x - F1_.x, F1.y - F1_.y);
            return;
        }

        // Масштаб
        Size = (float)(worldLen / localLen);

        // Углы. Используем соглашение: 0° = вверх, по часовой стрелке, Y вниз.
        // Это соответствует Vec2dPlant / RotateVector.
        double localAngle = Math.Atan2(localDx, -localDy) * 180.0 / Math.PI;
        double worldAngle = Math.Atan2(worldDx, -worldDy) * 180.0 / Math.PI;

        Angle = (float)(worldAngle - localAngle);

        // Нормализуем угол в диапазон (-180, 180]
        while (Angle > 180) Angle -= 360;
        while (Angle <= -180) Angle += 360;

        // Находим Start.
        // Локальный вектор от левого верхнего угла текстуры до focus1:
        Vec2d localFocus1 = new Vec2d(F1_.x, F1_.y);

        // Масштабируем и поворачиваем его
        Vec2d scaledFocus1 = localFocus1 * Size;
        scaledFocus1.RotateVector(Angle);

        // Start = F1 - (повёрнутый и масштабированный focus1)
        Start = F1 - scaledFocus1;
    }


};


public class Vec2d
{
    public double x, y;
    public Vec2d(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
    public Vec2d(Vec2d other)
    {
        this.x = other.x;
        this.y = other.y;
    }
    public override string ToString()
    {
        return $"({x}, {y})";
    }
    public double Length()
    {
        return Math.Sqrt(x * x + y * y);
    }
    public Vec2d Normalize()
    {
        double len = Length();
        if (len == 0) return new Vec2d(0, 0);
        return new Vec2d(x / len, y / len);
    }
    public void Vec2dPlant(double angle, double len)
    {
        double angleRadians = angle * Math.PI / 180.0;
        x = Math.Sin(angleRadians) * len;
        y = -Math.Cos(angleRadians) * len;

    }
    public void RotateVector(double angle)
    {
        double length = Math.Sqrt(x * x + y * y);
        double currentAngleRad = Math.Atan2(x, -y);
        double newAngleRad = currentAngleRad + (angle * Math.PI / 180.0);

        x = Math.Sin(newAngleRad) * length;
        y = -Math.Cos(newAngleRad) * length;
    }
    public int GetSide(Vec2d p2)
    {
        double cross = x * p2.y - y * p2.x;

        if (cross < 0)
            return -1;
        else
            return 1;
    }

    public static Vec2d operator +(Vec2d a, Vec2d b)
    {
        return new Vec2d(a.x + b.x, a.y + b.y);
    }

    public static Vec2d operator -(Vec2d a, Vec2d b)
    {
        return new Vec2d(a.x - b.x, a.y - b.y);
    }

    public static Vec2d operator *(Vec2d v, double scalar)
    {
        return new Vec2d(v.x * scalar, v.y * scalar);
    }

    public static Vec2d operator *(double scalar, Vec2d v)
    {
        return new Vec2d(v.x * scalar, v.y * scalar);
    }
    public static double operator *(Vec2d a, Vec2d b)
    {
        return a.x * b.x + a.y * b.y;
    }
    public static Vec2d operator /(Vec2d v, double scalar)
    {
        return new Vec2d(v.x / scalar, v.y / scalar);
    }
    public static Vec2d operator +(Vec2d v, double scalar)
    {
        if (v == null) return null;

        double len = v.Length();
        if (len == 0) return new Vec2d(0, 0);

        double newLen = len + scalar;
        double scale = newLen / len;

        return new Vec2d(v.x * scale, v.y * scale);
    }
    public static Vec2d operator +(double scalar, Vec2d v)
    {
        return v + scalar;
    }
}