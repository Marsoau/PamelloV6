using System.Reflection;

namespace PamelloV6.API.Config
{
    public static class PamelloConfig
    {
        public static string BotToken;
        public static string[] SpeakersTokens;
        public static string YoutubeToken;

        public static ulong TestGuildId;

        private static IConfigurationRoot config;

        static PamelloConfig() {
            config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddYamlFile("Config/config.yaml")
                .Build();

            LoadValues();
        }

        public static void LoadValues() {
            var fields = typeof(PamelloConfig).GetFields();
            foreach (var field in fields) {
                if (!field.IsPrivate) {
                    LoadField(field);
                }
            }
        }
        private static void LoadField(FieldInfo field) {
            var section = config.GetSection(field.Name);
            var configValue = section.Get(field.FieldType);
            if (configValue is null) throw new Exception($"Cant find \"{field.Name}\" field in config");

            field.SetValue(null, configValue);
        }
    }
}
