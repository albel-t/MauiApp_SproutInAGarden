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
        AddTexture("tomato_bigstem_example");
        AddTexture("tomato_smallstem_example");
        AddTexture("tomato_fruit_example");
    }

    public static Texture GetTexture(string name)
    {
        return _textures[name];
    }

    public static void AddTexture(string name)
    {
        _textures.Add(name, new Texture(name));
    }
}

// 1. ИЗМЕНИЛИ НА class. Теперь объект передается по ссылке, и данные не будут теряться
public class Texture
{
    public string name = string.Empty;
    public int width = 0;
    public int height = 0;
    public SKBitmap img;

    public Texture(string name)
    {
        this.name = name;
        LoadTexture(name); 
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
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось загрузить {name}: {ex.Message}");
        }
    }
}
