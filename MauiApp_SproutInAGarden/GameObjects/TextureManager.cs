namespace MauiApp_SproutInAGarden.GameObjects;

using SkiaSharp;

public static class TextureManager
{
    public static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();;
	public static void Init()
	{

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

public struct Texture
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
            // Из папки Resources/Images/ (MauiAsset)
            var assembly = typeof(TextureManager).Assembly;
            var stream = assembly.GetManifestResourceStream($"MauiApp_SproutInAGarden.Resources.Images.{name}.png");

            if (stream != null)
            {
                var bitmap = SKBitmap.Decode(stream);
                if (bitmap != null)
                {
                    img = bitmap;
                    System.Diagnostics.Debug.WriteLine($"Загружена текстура: {name}");
                }
            }
            else
            {
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Не удалось загрузить {name}: {ex.Message}");
        }
    }


}