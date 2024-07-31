using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PamelloV6.API.Config
{
    public static class PamelloConfig
    {
        public static string BotToken;
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
            var configValue = config[field.Name];
            if (configValue is null) throw new Exception($"Cant find \"{field.Name}\" field in config");

            var value = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(configValue);
            field.SetValue(null, value);
        }
    }
}
