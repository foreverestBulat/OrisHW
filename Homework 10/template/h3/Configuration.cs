using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer;

public class Configuration
{
    public static AppSettingsConfig GetConfig(string fileName)
    {
        if (File.Exists(fileName))
        {
            AppSettingsConfig config;
            using (var file = new FileStream("appsettings.json", FileMode.Open))
            {
                config = System.Text.Json.JsonSerializer.Deserialize<AppSettingsConfig>(file);
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            CheckExistFolderSite(config);
            return config;
        }
        else
        {
            throw new FileNotFoundException(fileName);
        }
    }

    private static void CheckExistFolderSite(AppSettingsConfig config)
    {
        if (!Directory.Exists(config.StaticPathFiles))
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), config.StaticPathFiles));
                Console.WriteLine("Была создана папка Site", config.StaticPathFiles);
            }
            catch
            {
                Console.WriteLine("Невозможно создать папку");
            }
        }
    }
}
