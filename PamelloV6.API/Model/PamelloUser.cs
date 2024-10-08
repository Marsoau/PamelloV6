﻿using Discord.WebSocket;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model;
using PamelloV6.API.Model.Audio;
using PamelloV6.API.Model.Events;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;

namespace PamelloV6.Server.Model
{
	public class PamelloUser : PamelloEntity {
		internal readonly UserEntity Entity;
		public readonly SocketUser DiscordUser;

		public override int Id {
			get => Entity.Id;
		}

        public override string Name {
			get => DiscordUser.GlobalName;
			set { }
		}

        public Guid Token {
			get => Entity.Token;
		}

		public bool IsAdministrator {
			get => Entity.IsAdministrator;
			set {
				Entity.IsAdministrator = value;
				Save();

                _events.SendToAll(PamelloEvent.UserAdministratorStateUpdated(Id, IsAdministrator));
            }
		}

		public List<PamelloPlaylist> OwnedPlaylists {
			get => Entity.OwnedPlaylists.Select(playlistEntity => _playlists.Get(playlistEntity.Id)
				?? throw new PamelloException("Attempted to get song that doesnt exist")).ToList();
		}

		private PamelloPlayer? _selectedPlayer;
		public PamelloPlayer? SelectedPlayer {
			get => _selectedPlayer;
			set {
				_selectedPlayer = value;
				_events.SendToOne(Id, PamelloEvent.UserPlayerSelected(SelectedPlayer?.Id));
            }
		}

		public PamelloUser(UserEntity userEntity, SocketUser discordUser,
			IServiceProvider services
		) : base(services) {
			Entity = userEntity;
			DiscordUser = discordUser;
		}

		public PamelloPlaylist CreatePlaylist(string name, bool isProtected = true) {
			return _playlists.Add(name, isProtected, this);
		}

		public override string ToString() {
			return $"[U: {Id} | {DiscordUser.Id}] {DiscordUser.GlobalName}";
		}

		public override object GetDTO() {
			return new UserDTO() {
				Id = Id,
                Name = DiscordUser.GlobalName,
                CoverUrl = DiscordUser.GetAvatarUrl(),
                DiscordId = DiscordUser.Id,
				SelectedPlayerId = SelectedPlayer?.Id,
				IsAdministrator = IsAdministrator,

				OwnedPlaylistIds = OwnedPlaylists?.Select(playlist => playlist.Id) ?? [],
			};
		}
	}
}
