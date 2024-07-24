using Discord;

namespace PamelloV6.API.Model.Interactions.Builders
{
    public static class PamelloEmbedBuilder
    {
        public static Embed Build(string header, string content, uint color) {
            return new EmbedBuilder() {
                Title = header,
                Description = content,
            }
            .WithColor(color)
            .Build();
        }
        public static Embed BuildInfo(string message) {
            return new EmbedBuilder() {
                Title = "Info",
                Description = message,
            }
            .WithColor(0x003030FF)
            .Build();
        }
        public static Embed BuildError(string message) {
            return new EmbedBuilder() {
                Title = "Error",
                Description = message,
            }
            .WithColor(0x00484848)
            .Build();
        }
        public static Embed BuildException(string message) {
            return new EmbedBuilder() {
                Title = "Exception",
                Description = message,
            }
            .WithColor(0x00FF3030)
            .Build();
        }
    }
}
