using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PamelloV6.API.Model.Interactions;

namespace PamelloV6.API.Extensions
{
    public static class InteracionExtensions
    {
        public static async Task RespondWithEmbedAsync(this SocketInteraction intecration, Embed embed) {
            await intecration.ModifyOriginalResponseAsync(message => message.Embed = embed);
        }
    }
}
