using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using static UnityModManagerNet.UnityModManager;

namespace ScalingCantrips.Config
{
  static class ModSettings
  {
    public static ModEntry ModEntry;
    // public static Fixes Fixes;
    //public static AddedContent AddedContent;
    public static Blueprints Blueprints;

    // public static Scaling Scaling;
    public static void LoadAllSettings()
    {
      //  LoadSettings("Fixes.json", ref Fixes);
      //  LoadSettings("AddedContent.json", ref AddedContent);
      LoadSettings("Blueprints.json", ref Blueprints);
      //LoadSettings("Scaling.json", ref Scaling);
    }

    private static void LoadSettings<T>(string fileName, ref T setting) where T : IUpdatableSettings
    {
      Main.Logger.NativeLog($"Loading Settings: {fileName}");

      var assembly = Assembly.GetExecutingAssembly();
      string userConfigFolder = ModEntry.Path + "UserSettings";
      Directory.CreateDirectory(userConfigFolder);
      var resourcePath = $"ScalingCantrips.Config.{fileName}";
      var userPath = $"{userConfigFolder}{Path.DirectorySeparatorChar}{fileName}";

      using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
      using (StreamReader reader = new StreamReader(stream))
      {
        setting = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
      }

      if (File.Exists(userPath))
      {
        Main.Logger.Log($"Creating new settings file for {fileName}");
        using (StreamReader reader = File.OpenText(userPath))
        {
          try
          {
            T userSettings = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            setting.OverrideSettings(userSettings);
          }
          catch
          {
            Main.Error("Failed to load user settings. Settings will be rebuilt.");
            try {
              File.Copy(userPath, userConfigFolder + $"{Path.DirectorySeparatorChar}BROKEN_{fileName}", true);
            }
            catch
            {
              Main.Error("Failed to archive broken settings.");
            }
          }
        }
      }
      else
      {
        Main.Logger.Log($"No settings file found for {fileName}");
      }
      File.WriteAllText(userPath, JsonConvert.SerializeObject(setting, Formatting.Indented));
    }
  }
}
