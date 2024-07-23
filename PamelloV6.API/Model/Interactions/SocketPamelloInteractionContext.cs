using Discord.Interactions;
using Discord.WebSocket;
using PamelloV6.API.Modules;
using PamelloV6.Server.Model;

namespace PamelloV6.API.Model.Interactions
{
    public class SocketPamelloInteractionContext : SocketInteractionContext
    {
        public readonly PamelloUser User;
        public readonly PamelloCommandsModule Commands;

        public readonly IServiceProvider Services;

        public SocketPamelloInteractionContext(
            PamelloUser pamelloUser,
            DiscordSocketClient client,
            SocketInteraction interaction,

            IServiceProvider services
        ) : base(client, interaction) {
            User = pamelloUser;

            Services = services;

            Commands = services.GetRequiredService<PamelloCommandsModule>();
            Commands.User = User;
        }
    }
}
