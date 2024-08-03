using PamelloV6.API.Downloads;
using PamelloV6.API.Model.Audio;
using PamelloV6.Core.DTO;
using PamelloV6.Core.Enumerators;
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
        public static PamelloEvent PlayerCreated(int playerId)
            => new PamelloEvent() {
                Header = "PlayerCreated",
                Data = playerId
            };

        public static PamelloEvent PlayerNameUpdated(int playerId, string newName)
            => new PamelloEvent() {
                Header = "PlayerNameUpdated",
                Data = new {
                    playerId,
                    newName
                },
            };

        public static PamelloEvent PlayerStateUpdated(PamelloPlayerState newState)
            => new PamelloEvent() {
                Header = "PlayerStateUpdated",
                Data = newState
            };
        public static PamelloEvent PlayerCurrentTimeUpdated(int newSeconds)
            => new PamelloEvent() {
                Header = "PlayerCurrentTimeUpdated",
                Data = newSeconds
            };
        public static PamelloEvent PlayerTotalTimeUpdated(int newSeconds)
            => new PamelloEvent() {
                Header = "PlayerTotalTimeUpdated",
                Data = newSeconds
            };
        public static PamelloEvent PlayerSpeakersUpdated(IEnumerable<SpeakerDTO> newSpeakers)
            => new PamelloEvent() {
                Header = "PlayerSpeakersUpdated",
                Data = newSpeakers
            };

        //Player Queue events
        public static PamelloEvent PlayerQueuePositionUpdated(int newPosition)
            => new PamelloEvent() {
                Header = "PlayerQueuePositionUpdated",
                Data = newPosition
            };
        public static PamelloEvent PlayerQueueSongUpdated(int? newSongId)
            => new PamelloEvent() {
                Header = "PlayerQueueSongUpdated",
                Data = newSongId
            };
        public static PamelloEvent PlayerQueueListUpdated(IEnumerable<int> songsIds)
            => new PamelloEvent() {
                Header = "PlayerQueueListUpdated",
                Data = songsIds
            };
        public static PamelloEvent PlayerQueueIsRandomUpdated(bool newState)
            => new PamelloEvent() {
                Header = "PlayerQueueIsRandomUpdated",
                Data = newState
            };
        public static PamelloEvent PlayerQueueIsReversedUpdated(bool newState)
            => new PamelloEvent() {
                Header = "PlayerQueueIsReversedUpdated",
                Data = newState
            };
        public static PamelloEvent PlayerQueueIsNoLeftoversUpdated(bool newState)
            => new PamelloEvent() {
                Header = "PlayerQueueIsNoLeftoversUpdated",
                Data = newState
            };
        public static PamelloEvent PlayerQueueNextPositionUpdated(int? nextPositionRequest)
            => new PamelloEvent() {
                Header = "PlayerQueueNextPositionUpdated",
                Data = nextPositionRequest
            };

        //Episode events
        public static PamelloEvent EpisodeUpdated(int episodeId)
            => new PamelloEvent() {
                Header = "EpisodeUpdated",
                Data = episodeId
            };
        public static PamelloEvent EpisodeCreated(int episodeId)
            => new PamelloEvent() {
                Header = "EpisodeCreated",
                Data = episodeId
            };
        public static PamelloEvent EpisodeNameUpdated(int episodeId, string newName)
            => new PamelloEvent() {
                Header = "EpisodeNameUpdated",
                Data = new {
                    episodeId,
                    newName
                }
            };
        public static PamelloEvent EpisodeStartUpdated(int episodeId, int newSeconds)
            => new PamelloEvent() {
                Header = "EpisodeStartUpdated",
                Data = new {
                    episodeId,
                    newSeconds
                }
            };
        public static PamelloEvent EpisodeSkipStateUpdated(int episodeId, bool newState)
            => new PamelloEvent() {
                Header = "EpisodeSkipStateUpdated",
                Data = new {
                    episodeId,
                    newState
                }
            };

        //Playlist events
        public static PamelloEvent PlaylistUpdated(int playlistId)
            => new PamelloEvent() {
                Header = "PlaylistUpdated",
                Data = playlistId
            };
        public static PamelloEvent PlaylistCreated(int playlistId)
            => new PamelloEvent() {
                Header = "PlaylistCreated",
                Data = playlistId
            };
        public static PamelloEvent PlaylistNameUpdated(int playlistId, string newName)
            => new PamelloEvent() {
                Header = "PlaylistNameUpdated",
                Data = new {
                    playlistId,
                    newName
                }
            };
        public static PamelloEvent PlaylistProtectionUpdated(int playlistId, bool newState)
            => new PamelloEvent() {
                Header = "PlaylistProtectionUpdated",
                Data = new {
                    playlistId,
                    newState
                }
            };
        public static PamelloEvent PlaylistSongsUpdated(int playlistId, IEnumerable<int> newSongsIds)
            => new PamelloEvent() {
                Header = "PlaylistSongsUpdated",
                Data = new {
                    playlistId,
                    newSongsIds
                }
            };

        //Song events
        public static PamelloEvent SongUpdated(int songId)
            => new PamelloEvent() {
                Header = "SongUpdated",
                Data = songId
            };
        public static PamelloEvent SongCreated(int songId)
            => new PamelloEvent() {
                Header = "SongCreated",
                Data = songId
            };
        public static PamelloEvent SongNameUpdated(int songId, string newName)
            => new PamelloEvent() {
                Header = "SongNameUpdated",
                Data = new {
                    songId,
                    newName
                }
            };
        public static PamelloEvent SongAuthorUpdated(int songId, string newAuthor)
            => new PamelloEvent() {
                Header = "SongAuthorUpdated",
                Data = new {
                    songId,
                    newAuthor
                }
            };
        public static PamelloEvent SongPlayCountUpdated(int songId, int newCount)
            => new PamelloEvent() {
                Header = "SongPlayCountUpdated",
                Data = new {
                    songId,
                    newCount
                }
            };
        public static PamelloEvent SongEpisodesUpdated(int songId, IEnumerable<int> newEpisodesIds)
            => new PamelloEvent() {
                Header = "SongEpisodesUpdated",
                Data = new {
                    songId,
                    newEpisodesIds
                }
            };
        public static PamelloEvent SongDownloadStarted(int songId)
            => new PamelloEvent() {
                Header = "SongDownloadStarted",
                Data = songId
            };
        public static PamelloEvent SongDownloadEnded(int songId, DownloadResult result)
            => new PamelloEvent() {
                Header = "SongDownloadEnded",
                Data = new {
                    songId,
                    result
                }
            };

        //User events
        public static PamelloEvent UserUpdated(int userId)
            => new PamelloEvent() {
                Header = "UserUpdated",
                Data = userId
            };
        public static PamelloEvent UserNameUpdated(int userId, string newName)
            => new PamelloEvent() {
                Header = "UserNameUpdated",
                Data = new {
                    userId,
                    newName
                }
            };
        public static PamelloEvent UserPlayerSelected(int? newPlayerId)
            => new PamelloEvent() {
                Header = "UserPlayerSelected",
                Data = newPlayerId
            };
        public static PamelloEvent UserAdministratorStateUpdated(int userId, bool newState)
            => new PamelloEvent() {
                Header = "UserAdministratorStateUpdated",
                Data = new {
                    userId,
                    newState
                }
            };


        public override string ToString() => Header;
    }
}
