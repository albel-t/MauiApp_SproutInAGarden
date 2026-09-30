namespace MauiApp_SproutInAGarden.GameObjects;

using Microsoft.Maui.Storage;
using SkiaSharp;
using System.IO;

public static class TextureManager
{
    public static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

    public static void Init()
    {
        AddTexture("tomato_example");
        AddTexture("27_24_27_165_tomato_bigstem_example");
        AddTexture("tomato_smallstem_example");
        AddTexture("tomato_fruit_example");
    }

    public static Texture GetTexture(string name)
    {
        return _textures[name];
    }
    public static int[] GetTexture_f(string name)
    {
        return _textures[name].GetF();
    }
    public static void AddTexture(string name)
    {
        _textures.Add(name, new Texture(name));
    }
}

public class Texture
{
    public string name = string.Empty;
    public int width = 0;
    public int height = 0;

    public int focus1_x = 0;
    public int focus1_y = 0;

    public int focus2_x = 0;
    public int focus2_y = 0;


    public SKBitmap img;

    public Texture(string name)
    {
        this.name = name;
        LoadTexture(name); 
    }
    public int[] GetF()
    {
        return new int[4]
        {
        focus1_x,
        focus1_y,
        focus2_x,
        focus2_y
        };
    }

    private void LoadTexture(string name)
    {
        try
        {
            using Stream stream = FileSystem.OpenAppPackageFileAsync($"{name}.jpg").Result;

            if (stream != null)
            {
                var bitmap = SKBitmap.Decode(stream);
                if (bitmap != null)
                {
                    img = bitmap;
                    width = bitmap.Width;
                    height = bitmap.Height;
                    System.Diagnostics.Debug.WriteLine($"Успешно загружена текстура: {name} ({width}x{height})");
                }
            }
            //ConvertPixelToNumbers(img, 0, 0);
            //ConvertPixelToNumbers(img, 1, 0);
            int[] numbers = name.Split('_')
                             .Take(4)
                             .Select(int.Parse)
                             .ToArray();
            focus1_x = numbers[0];
            focus1_y = numbers[1];

            focus2_x = numbers[2];
            focus2_y = numbers[3];




        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось загрузить {name}: {ex.Message}");
        }
    }


    
}
