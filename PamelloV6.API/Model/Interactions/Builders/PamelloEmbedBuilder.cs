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
        public static Embed BuildInfo(string header, string message) {
            return Info(header, message).Build();
        }
        public static Embed BuildError(string message) {
            return Error(message).Build();
        }
        public static Embed BuildException(string message) {
            return Exception(message).Build();
        }
        public static Embed BuildWait() {
            return Wait().Build();
        }
        public static EmbedBuilder Info(string header, string message) {
            return new EmbedBuilder() {
                Title = header,
                Description = message,
            }
            .WithColor(0x007272FF);
        }
        public static EmbedBuilder Error(string message) {
            return new EmbedBuilder() {
                Title = "Error",
                Description = message,
            }
            .WithColor(0x00484848);
        }
        public static EmbedBuilder Exception(string message) {
            return new EmbedBuilder() {
                Title = "Exception",
                Description = message,
            }
            .WithColor(0x00FF3030);
        }
        public static EmbedBuilder Wait() {
            return new EmbedBuilder() {
                Title = "Wait",
                Description = "Processing...",
            }
            .WithColor(0x00303030);
        }
    }
}
