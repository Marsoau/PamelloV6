using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;

namespace PamelloV6.API.Model
{
	public class PamelloPlaylist : PamelloEntity
	{
		private protected readonly PlaylistEntity Entity;

		public override int Id {
			get => Entity.Id;
		}

		public string Name {
			get => Entity.Name;
			set {
				Entity.Name = value;
				Save();

                SendUpdate("updatedPlaylist");
                _events.SendToAll("updatedPlaylistName", new {
                    playlistId = Id,
                    newValue = Name
                });
            }
		}
		public PamelloUser Owner {
			get => _users.Get(Entity.Id) ?? throw new Exception("Attempted to get song that doesnt exist");
		}
		public bool IsProtected {
			get => Entity.IsProtected;
			set {
				Entity.IsProtected = value;
				Save();

                SendUpdate("updatedPlaylist");
                _events.SendToAll("updatedPlaylistIsProtected", new {
                    playlistId = Id,
                    newValue = IsProtected
                });
            }
		}

		public List<PamelloSong> Songs {
			get => Entity.Songs.Select(songEntity => _songs.Get(songEntity.Id)
				?? throw new Exception("Attempted to get song that doesnt exist")).ToList();
		}

		public PamelloPlaylist(PlaylistEntity entity,
			IServiceProvider services
		) : base(services) {
			Entity = entity;
		}

		public void AddSong(PamelloSong song) {
			Entity.Songs.Add(song.Entity);
            Save();

            SendUpdate("updatedPlaylist");
            _events.SendToAll("updatedPlaylistSongs", Songs);
        }
		public void AddSongs(IEnumerable<PamelloSong> songs) {
			Entity.Songs.AddRange(songs.Select(song => song.Entity));
			Save();

            SendUpdate("updatedPlaylist");
            _events.SendToAll("updatedPlaylistSongs", Songs);
        }
		public void InsertSong(int position, PamelloSong song) {
			Entity.Songs.Insert(position, song.Entity);
			Save();

            SendUpdate("updatedPlaylist");
            _events.SendToAll("updatedPlaylistSongs", Songs);
        }
		public void RemoveSong(int position) {
			Entity.Songs.RemoveAt(position);
			Save();

            SendUpdate("updatedPlaylist");
            _events.SendToAll("updatedPlaylistSongs", Songs);
        }

		public override object GetDTO() {
			return new PlaylistDTO() {
				Id = Id,
				Name = Name,
				OwnerId = Owner.Id,
				IsProtected = IsProtected,

				SongIds = Entity.Songs.Select(song => song.Id).ToList(),
			};
		}

		public override string ToString() {
			return $"[P: {Id} ({Entity.Songs.Count} Songs)] {Name}";
		}
	}
}
