using Discord.Audio;
using Discord.Interactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Expressions;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Modules;
using PamelloV6.API.Repositories;
using PamelloV6.Server.Handlers;
using PamelloV6.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PamelloV6.Server.Modules
{
	public class InteractionModule : InteractionModuleBase<SocketPamelloInteractionContext>
	{
		private readonly UserAuthorizationService _authtorization;

		protected readonly PamelloUserRepository _users;
		protected readonly PamelloSongRepository _songs;
		protected readonly PamelloEpisodeRepository _episodes;
		protected readonly PamelloPlaylistRepository _playlists;

		public InteractionModule(
			UserAuthorizationService authtorization,

			PamelloUserRepository users,
			PamelloSongRepository songs,
			PamelloEpisodeRepository episodes,
			PamelloPlaylistRepository playlists
		) {
			_authtorization = authtorization;

			_users = users;
			_songs = songs;
			_episodes = episodes;
			_playlists = playlists;
		}

		[SlashCommand("ping", "Check if bot is alive")]
		public async Task Ping() {
			await RespondAsync("Pong", ephemeral: true);
		}

		[SlashCommand("get-code", "Get authorisation code")]
		public async Task GetCode() {
			await RespondAsync($"Authrozation Code: {_authtorization.GetCode(Context.User.DiscordUser.Id)}", ephemeral: true);
		}

		[SlashCommand("player-select", "Select player")]
		public async Task PlayerSelect(
			[Summary("player", "Name or id of player to select")] string playerValue
		) {

		}

		[SlashCommand("add", "Add (new) song to bot and queue if any player selected", runMode: RunMode.Async)]
		public async Task Add(
			[Summary("song", "(Name)/Id/Youtube Url of song to add to a selected player queue")] string songValue
		) {
			await RespondAsync("gugugaga", ephemeral: true);

			if (!int.TryParse(songValue, out var songId)) {
				throw new Exception("Cant parse value to int");
			}

			var song = _songs.GetRequired(songId);
			var songAudio = new PamelloAudio(song);

			var guild = Context.Guild;
			var voice = guild.GetUser(Context.User.DiscordUser.Id).VoiceChannel;

			var ac = await voice.ConnectAsync();

			var outputStream = ac.CreatePCMStream(AudioApplication.Mixed);

			byte[]? audioBytes;
			if (await songAudio.TryInitialize()) {
				while ((audioBytes = songAudio.NextBytes()) is not null) {
					outputStream.Write(audioBytes);
				}
			}
		}

		private async Task AddByUrl(Uri url) {

		}

		private async Task AddByName(string name) {

		}

		private async Task AddById(int id) {

		}
	}
}
