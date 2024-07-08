using PamelloV6.API.Downloads;
using PamelloV6.API.Model.Audio;
using System.Collections;

namespace PamelloV6.API.Model.Events
{
    public class PamelloEvent
    {
        public string Header { get; private set; }
        public object? Data { get; private set; }

        private PamelloEvent() {
            Header = "";
        }

        public static PamelloEvent LogMessage(string message)
            => new PamelloEvent() {
                Header = "LogMessage",
                Data = message,
            };

        public static PamelloEvent TokenUpdated()
            => new PamelloEvent() {
                Header = "TokenUpdated"
            };

        //Player events
        public static PamelloEvent PlayerNameUpdated(int playerId, string newName)
            => new PamelloEvent() {
                Header = "TokenUpdated",
                Data = new {
                    playerId,
                    newName
                },
            };

        public static PamelloEvent PlayerPauseUpdated(bool newState)
            => new PamelloEvent();
        public static PamelloEvent PlayerCurrentTimeUpdated(int seconds)
            => new PamelloEvent();
        public static PamelloEvent PlayerTotalTimeUpdated(int seconds)
            => new PamelloEvent();

        //Player Queue events
        public static PamelloEvent PlauerQueuePositionUpdated(int newPosition)
            => new PamelloEvent();
        public static PamelloEvent PlauerQueueSongUpdated(int? newSongId)
            => new PamelloEvent();
        public static PamelloEvent PlauerQueueListUpdated(IEnumerable<int> songsIds)
            => new PamelloEvent();
        public static PamelloEvent PlayerQueueIsRandomUpdated(bool newState)
            => new PamelloEvent();
        public static PamelloEvent PlayerQueueIsReversedUpdated(bool newState)
            => new PamelloEvent();
        public static PamelloEvent PlayerQueueIsNoLeftoversUpdated(bool newState)
            => new PamelloEvent();
        public static PamelloEvent PlayerQueueNextPositionUpdated(int? nextPositionRequest)
            => new PamelloEvent();

        //Episode events
        public static PamelloEvent EpisodeUpdated(int episodeId)
            => new PamelloEvent();
        public static PamelloEvent EpisodeNameUpdated(int episodeId, string newName)
            => new PamelloEvent();
        public static PamelloEvent EpisodeStartUpdated(int episodeId, int newSeconds)
            => new PamelloEvent();
        public static PamelloEvent EpisodeSkipStateUpdated(int episodeId, bool newState)
            => new PamelloEvent();

        //Playlist events
        public static PamelloEvent PlaylistUpdated(int playlistId)
            => new PamelloEvent();
        public static PamelloEvent PlaylistNameUpdated(int playlistId, string newName)
            => new PamelloEvent();
        public static PamelloEvent PlaylistProtectionUpdated(int playlistId, bool newState)
            => new PamelloEvent();
        public static PamelloEvent PlaylistSongsUpdated(int playlistId, IEnumerable<int> newSongsIds)
            => new PamelloEvent();

        //Song events
        public static PamelloEvent SongUpdated(int songId)
            => new PamelloEvent();
        public static PamelloEvent SongNameUpdated(int songId, string newName)
            => new PamelloEvent();
        public static PamelloEvent SongAuthorUpdated(int songId, string newAuthor)
            => new PamelloEvent();
        public static PamelloEvent SongPlayCountUpdated(int songId, int newCount)
            => new PamelloEvent();
        public static PamelloEvent SongDownloadStarted(int songId)
            => new PamelloEvent();
        public static PamelloEvent SongDownloadEnded(int songId, DownloadResult result)
            => new PamelloEvent();

        //User events
        public static PamelloEvent UserUpdated(int userId)
            => new PamelloEvent();
        public static PamelloEvent UserNameUpdated(int userId, string newName)
            => new PamelloEvent();
        public static PamelloEvent UserPlayerSelected(int? newPlayerId)
            => new PamelloEvent();
        public static PamelloEvent UserAdministratorStateUpdated(int userId, bool newState)
            => new PamelloEvent();
    }
}
