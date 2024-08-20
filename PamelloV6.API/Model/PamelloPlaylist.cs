using AngleSharp.Dom;
using PamelloV6.API.Exceptions;
using PamelloV6.API.Model.Events;
using PamelloV6.Core.DTO;
using PamelloV6.DAL;
using PamelloV6.DAL.Entity;
using PamelloV6.Server.Model;

namespace PamelloV6.API.Model
{
	public class PamelloPlaylist : PamelloEntity
	{
		internal readonly PlaylistEntity Entity;

		public override int Id {
			get => Entity.Id;
		}

		public override string Name {
			get => Entity.Name;
			set {
				Entity.Name = value;
				Save();

                _events.SendToAll(
					PamelloEvent.PlaylistNameUpdated(Id, Name)
				);
            }
		}
		public PamelloUser? Owner {
			get => _users.Get(Entity.Owner.Id);
		}
		public bool IsProtected {
			get => Entity.IsProtected;
			set {
				Entity.IsProtected = value;
				Save();

                _events.SendToAll(
                    PamelloEvent.PlaylistProtectionUpdated(Id, IsProtected)
                );
            }
		}

		public List<PamelloSong> Songs {
			get => Entity.Songs.Select(songEntity => _songs.Get(songEntity.Id)
				?? throw new PamelloException("Attempted to get song that doesnt exist")).ToList();
		}

		public PamelloPlaylist(PlaylistEntity entity,
			IServiceProvider services
		) : base(services) {
			Entity = entity;
		}

		public void AddSong(PamelloSong song) {
			Entity.Songs.Add(song.Entity);
            Save();

			song.SendPlaylistsUpdatedEvent();
			SendSongsUpdatedEvent();
        }
        public void InsertSong(int position, PamelloSong song) {
            Entity.Songs.Insert(position, song.Entity);
            Save();

            song.SendPlaylistsUpdatedEvent();
            SendSongsUpdatedEvent();
        }
        public void AddSongs(IEnumerable<PamelloSong> songs) {
			InsertSongs(Songs.Count, songs);
        }
        public void InsertSongs(int position, IEnumerable<PamelloSong> songs) {
            Entity.Songs.InsertRange(position, songs.Select(song => song.Entity));
            Save();

            foreach (var song in songs) {
                song.SendPlaylistsUpdatedEvent();
            }
            SendSongsUpdatedEvent();
        }
        public void MoveSong(int fromPosition, int toPosition) {
			if (toPosition > fromPosition) toPosition--;

			var song = Entity.Songs[fromPosition];
            Entity.Songs.RemoveAt(fromPosition);
			Entity.Songs.Insert(toPosition, song);

            Save();

            SendSongsUpdatedEvent();
        }
        public void SwapSong(int inPosition, int withPosition) {
			var buffer = Entity.Songs[inPosition];
            Entity.Songs[inPosition] = Entity.Songs[withPosition];
			Entity.Songs[withPosition] = buffer;
			
            Save();

            SendSongsUpdatedEvent();
        }
        public void RemoveSong(PamelloSong song) {
            Entity.Songs.RemoveAll(s => s.Id == song.Id);
            Save();

            song.SendPlaylistsUpdatedEvent();
            SendSongsUpdatedEvent();
        }
        public void RemoveSongAt(int songPosition) {
            var song = Songs[songPosition];

            Entity.Songs.RemoveAt(songPosition);
            Save();

            song.SendPlaylistsUpdatedEvent();
            SendSongsUpdatedEvent();
        }

        public void SendSongsUpdatedEvent() {
            _events.SendToAll(
                PamelloEvent.PlaylistSongsUpdated(Id, Songs.Select(song => song.Id))
            );
        }

		public override object GetDTO() {
			return new PlaylistDTO() {
				Id = Id,
				Name = Name,
				OwnerId = Owner?.Id ?? Entity.Owner.Id,
				IsProtected = IsProtected,

				SongIds = Entity.Songs.Select(song => song.Id).ToList(),
			};
		}

		public override string ToString() {
			return $"[P: {Id} ({Entity.Songs.Count} Songs)] {Name}";
		}
	}
}
