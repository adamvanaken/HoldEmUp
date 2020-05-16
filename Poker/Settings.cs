using Newtonsoft.Json;
using System.IO;

namespace Solitare
{
    public class Settings
    {
        private const string PATH = ".settings.json";

        public PlayerRotationState Rotation { get; set; }

        public string LastUsedName { get; set; }

        public string LastUsedServer { get; set; }

        public static Settings Load()
        {
            try
            {
                if (File.Exists(PATH))
                {
                    var state = File.ReadAllText(PATH);
                    var settings = JsonConvert.DeserializeObject<Settings>(state);
                    return settings;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void Save(Settings settings)
        {
            try
            {
                if (settings != null)
                {
                    var state = JsonConvert.SerializeObject(settings);
                    File.WriteAllText(PATH, state);
                }
            }
            catch
            {

            }
        }
    }

    public enum PlayerRotationState
    {
        All = 0,
        Sides = 1,
        None = 2
    }
}
