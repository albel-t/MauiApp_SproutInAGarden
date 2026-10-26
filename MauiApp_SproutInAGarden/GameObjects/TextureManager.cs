namespace MauiApp_SproutInAGarden.GameObjects;

using System.Collections.Generic;
using Microsoft.Maui.Storage;
using SkiaSharp;
using System.IO;
public class TextureDB
{
    public string name;
    public string my_full_path;
    public Dictionary<string, TextureDB> folder = new Dictionary<string, TextureDB>();
    public Dictionary<string, Texture> image = new Dictionary<string, Texture>();

    public TextureDB(string path, bool files = true)
    {
        name = Path.GetFileName(path.TrimEnd('\\', '/'));
        my_full_path = path.TrimEnd('\\', '/');

        if (!Directory.Exists(path))
            return;

        foreach (string dir in Directory.GetDirectories(path))
            folder.Add(dir, new TextureDB(dir));
        foreach (string file in Directory.GetFiles(path))
            image.Add(file, new Texture(file));
    }

    public void my_full_path_update(string path)
    {
        my_full_path = path.TrimEnd('\\', '/');

        foreach (var sub in folder.Values)
            sub.my_full_path_update(Path.Combine(my_full_path, sub.name));
    }
    public Texture this[string path]
    {
        get
        {
            if (string.IsNullOrEmpty(path))
                return null;

            int sep = path.IndexOfAny(new[] { '\\', '/' });

            string head, tail;
            if (sep < 0)
            {
                head = path;
                tail = "";
            }
            else
            {
                head = path.Substring(0, sep);
                tail = path.Substring(sep + 1);
            }

            if (tail.Length == 0)
            {
                image.TryGetValue(head, out var tex);
                return tex;
            }

            if (folder.TryGetValue(head, out var next))
                return next[tail];

            return null;
        }
    }
    public bool Exist(string path)
    {
        if (string.IsNullOrEmpty(path))
            return true;

        int sep = path.IndexOfAny(new[] { '\\', '/' });

        string head, tail;
        if (sep < 0)
        {
            head = path;
            tail = "";
        }
        else
        {
            head = path.Substring(0, sep);
            tail = path.Substring(sep + 1);
        }

        if (folder.TryGetValue(head, out var next))
            return next.Exist(tail);

        if (tail.Length == 0 && image.ContainsKey(head))
            return true;

        return false;
    }
}
public static class TextureManager
{
    public static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();
    public static TextureDB textures_db;
    public static string WopkspaceFolder = "textures";
    public static void Init_textures()
    {
        AddTexture_ByName("tomato_example");
        AddTexture_ByName("27_24_27_165_tomato_bigstem_example");
        AddTexture_ByName("tomato_smallstem_example");
        AddTexture_ByName("tomato_fruit_example");
    }
    public static void Init_textures_db()
    {
        textures_db = new TextureDB(WopkspaceFolder);
    }
    public static void Init()
    {
        Init_textures();
        Init_textures_db();
    }
    public static Texture GetTexture(string name)
    {
        return textures_db[name];
    }
    public static TextureManagerData GetTexture(TextureManagerParams Params)
    {
        TextureManagerData result;// = new TextureManagerData();
        string new_path = WopkspaceFolder;
        for (string f = Params.Get(); !Params.Empty(); f = Params.Get())
        {
            new_path = new_path + "\\" + f;
            if(!CheckFolder(new_path))
            {
                result = new TextureManagerData(
                    GetTexture_f(WopkspaceFolder+"\\"+Params.GetReserveData()), 
                    GetTexture(WopkspaceFolder + "\\" + Params.GetReserveData()));
                return result;
            }
        }
        result = new TextureManagerData(GetTexture_f(new_path), GetTexture(new_path));
        return result;
    }
    private static bool CheckFolder(string path)
    {
        if (Directory.Exists(path))
            return true;
        else
            return false;
        /*
        if (Directory.Exists(path))
            return true;
        else
            return false;
        */
    }
    public static FocusesImg GetTexture_f(string name)
    {
        return textures_db[name].GetF();
    }
    public static void AddTexture_ByName(string name)
    {
        _textures.Add(name, new Texture(name));
    }
}
public class TextureManagerParams
{
    private Queue<string> Params;
    private string reserve = "trunk";
    public TextureManagerParams()
    {
        Params = new Queue<string>();
    }
    public TextureManagerParams(Queue<string> input)
    {
        Params = new Queue<string>(input);
    }
    public void Add(string parameter)
    {
        if((parameter == "trunk") || (parameter == "leaf") || (parameter == "fruit"))
            reserve = parameter;
        Params.Enqueue(parameter);
    }
    public string Get()
    {
        return Params.Dequeue();
    }
    public string GetReserveData()
    {
        return reserve;
    }
    public bool Empty()
    {
        return Params.Count == 0;
    }
}
public class TextureManagerData
{
    public FocusesImg focus;
    public Texture texture;
    public TextureManagerData(FocusesImg focus_, Texture texture_)
    {
        focus = focus_;
        texture = texture_;
    }
    public Texture GetTexture()
    {
        return texture;
    }
    public FocusesImg GetFocus()
    {
        return focus;
    }
}

public struct FocusesImg
{
    public Vec2d first;
    public Vec2d second;
    public FocusesImg(Vec2d focus1_, Vec2d focus2_)
    {
        first = focus1_;
        second = focus2_;
    }
    public double this[int index]
    {
        get
        {
            return index switch
            {
                0 => first.x,
                1 => first.y,
                2 => second.x,
                3 => second.y,
                _ => throw new IndexOutOfRangeException(
                        $"Индекс {index} вне диапазона [0..3]")
            };
        }
    }
}
public class Texture
{
    public string name = string.Empty;
    public int width = 0;
    public int height = 0;

    public FocusesImg f;


    public SKBitmap img;

    public Texture(string name)
    {
        this.name = name;
        LoadTexture(name); 
    }
    public FocusesImg GetF()
    {
        return f;
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
                    .Skip(1)
                    .Take(4)
                    .Select(int.Parse)
                    .ToArray();
            f = new FocusesImg(
            new Vec2d(numbers[0], numbers[1]),
            new Vec2d(numbers[2], numbers[3]));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось загрузить {name}: {ex.Message}");
        }
    }


    
}
