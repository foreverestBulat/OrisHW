namespace MyHttpServer.tempOfExtensions;


public static class DictContentType
{
    public static Dictionary<string, string> dictContentType = new()
    {
        [".css"] = "text/css",
        [".html"] = "text/html",
        [".jpg"] = "image/jpeg",
        [".svg"] = "image/svg+xml",
        [".png"] = "image/png",
        [".js"] = "text/javascript",
        [".webp"] = "image/png",
        [".ico"] = "image/ico",
        [".jpeg"] = "image/jpeg",
    };

    public static string GetImageContentType(string imagePath)
    {
        string extension = Path.GetExtension(imagePath).ToLower();
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";
            case ".png":
                return "image/png";
            case ".svg":
                return "image/svg+xml";
            default:
                return "application/octet-stream";
        }
    }
}
