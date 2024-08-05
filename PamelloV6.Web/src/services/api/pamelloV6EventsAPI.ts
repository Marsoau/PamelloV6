import { PamelloConfig } from "../../config/config";
import { PamelloPlayerState } from "./model/PamelloPlayer";
import { IPamelloSpeaker } from "./model/PamelloSpeaker";
import { PamelloV6API } from "./pamelloV6API.service";

export class PamelloV6EventsAPI {
    private eventSource: EventSource | null;

    public constructor(
        private readonly _api: PamelloV6API
    ) {
        this.eventSource = null;
    }

    public get isConnected(): boolean {
        return this.eventSource != null && this.eventSource.readyState == 1
    }

    public Connect(onsuccess: any = null, onfail: any = null) {
        if (!this._api.token) return;
        if (this.eventSource) this.Disconnect();

        console.log(`Connecting to events`)
        this.eventSource = new EventSource(`${PamelloConfig.BaseUrl}/Events?as=${this._api.token}`);
        this.eventSource.addEventListener("error", (event) => {
            if (this.eventSource?.readyState == 2) return;
            this.Disconnect();

            console.log(`Failed to connect to events`)
            
            if (onfail) onfail();
        });
        this.eventSource.addEventListener("open", () => {
            console.log(`Connected to events successfully`)
            
            this.SubscribeDefault();
            if (onsuccess) onsuccess();
        });
    }
    public Disconnect() {
        this.eventSource?.close();
        this.eventSource = null;
    }

    public AddEventListener(event: string, handler: any) {
        if (!this.eventSource) return;

        this.eventSource.addEventListener(event, (message) => {
            handler(JSON.parse(message.data));
        });

        //console.log(`Added event listener for [${event}] event`);
    }

    private SubscribeDefault() {
        console.log("Subscribe to default events start");

        this.UserPlayerSelected = (playerId: number) => {
            this._api.user!.selectedPlayerId = playerId;
            
            console.log(`Selected player with id ${playerId}`);
            this._api.LoadSelectedPlayer();
        }

        this.PlayerNameUpdated = (newName: string) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.name = newName;
        }
        this.PlayerPauseUpdated = (newState: boolean) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.isPaused = newState;
        }
        this.PlayerStateUpdated = (newState: PamelloPlayerState) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.state = newState;
        }
        this.PlayerCurrentTimeUpdated = (newSeconds: number) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.currentSongTimePassed = newSeconds;
        }
        this.PlayerTotalTimeUpdated = (newSeconds: number) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.currentSongTimeTotal = newSeconds;
        }
        this.PlayerQueueIsRandomUpdated = (newState: boolean) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.queueIsRandom = newState;
        }
        this.PlayerQueueIsReversedUpdated = (newState: boolean) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.queueIsReversed = newState;
        }
        this.PlayerQueueIsNoLeftoversUpdated = (newState: boolean) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.queueIsNoLeftovers = newState;
        }
        this.PlayerQueueListUpdated = (newSongIds: number[]) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.queueSongIds = newSongIds;

            this._api.LoadSelectedPlayerQueueSongs();
        }
        this.PlayerQueuePositionUpdated = (newPosition: number) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.queuePosition = newPosition;
        }
        this.PlayerQueueNextPositionUpdated = (nextPosition: number | null) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.nextPositionRequest = nextPosition;
        }
        this.PlayerQueueSongUpdated = (newCurrentSongId: number) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.currentSongId = newCurrentSongId;

            this._api.LoadSelectedPlayerSong();
        }
        this.PlayerSpeakersUpdated = (newSpeakers: IPamelloSpeaker[]) => {
            if (!this._api.selectedPlayer) return;
            this._api.selectedPlayer.speakers = newSpeakers;
        }
		this.SongEpisodesUpdated = (data: any) => {
			if (!this._api.selectedPlayerSong || this._api.selectedPlayerSong.id != data.songId) return;
			this._api.selectedPlayerSong.episodeIds = data.newEpisodesIds;

			this._api.LoadSelectedPlayerSongEpisodes();
		}

        console.log("Subscribe to default events end");
    }

    public set LogMessage(handler: any) {
        this.AddEventListener("LogMessage", handler);
    }
    public set TokenUpdated(handler: any) {
        this.AddEventListener("TokenUpdated", handler);
    }
    public set PlayerCreated(handler: any) {
        this.AddEventListener("PlayerCreated", handler);
    }
    public set PlayerNameUpdated(handler: any) {
        this.AddEventListener("PlayerNameUpdated", handler);
    }
    public set PlayerPauseUpdated(handler: any) {
        this.AddEventListener("PlayerPauseUpdated", handler);
    }
    public set PlayerStateUpdated(handler: any) {
        this.AddEventListener("PlayerStateUpdated", handler);
    }
    public set PlayerCurrentTimeUpdated(handler: any) {
        this.AddEventListener("PlayerCurrentTimeUpdated", handler);
    }
    public set PlayerTotalTimeUpdated(handler: any) {
        this.AddEventListener("PlayerTotalTimeUpdated", handler);
    }
    public set PlayerSpeakersUpdated(handler: any) {
        this.AddEventListener("PlayerSpeakersUpdated", handler);
    }
    public set PlayerQueuePositionUpdated(handler: any) {
        this.AddEventListener("PlayerQueuePositionUpdated", handler);
    }
    public set PlayerQueueSongUpdated(handler: any) {
        this.AddEventListener("PlayerQueueSongUpdated", handler);
    }
    public set PlayerQueueListUpdated(handler: any) {
        this.AddEventListener("PlayerQueueListUpdated", handler);
    }
    public set PlayerQueueIsRandomUpdated(handler: any) {
        this.AddEventListener("PlayerQueueIsRandomUpdated", handler);
    }
    public set PlayerQueueIsReversedUpdated(handler: any) {
        this.AddEventListener("PlayerQueueIsReversedUpdated", handler);
    }
    public set PlayerQueueIsNoLeftoversUpdated(handler: any) {
        this.AddEventListener("PlayerQueueIsNoLeftoversUpdated", handler);
    }
    public set PlayerQueueNextPositionUpdated(handler: any) {
        this.AddEventListener("PlayerQueueNextPositionUpdated", handler);
    }
    public set EpisodeUpdated(handler: any) {
        this.AddEventListener("EpisodeUpdated", handler);
    }
    public set EpisodeCreated(handler: any) {
        this.AddEventListener("EpisodeCreated", handler);
    }
    public set EpisodeNameUpdated(handler: any) {
        this.AddEventListener("EpisodeNameUpdated", handler);
    }
    public set EpisodeStartUpdated(handler: any) {
        this.AddEventListener("EpisodeStartUpdated", handler);
    }
    public set EpisodeSkipStateUpdated(handler: any) {
        this.AddEventListener("EpisodeSkipStateUpdated", handler);
    }
    public set PlaylistUpdated(handler: any) {
        this.AddEventListener("PlaylistUpdated", handler);
    }
    public set PlaylistCreated(handler: any) {
        this.AddEventListener("PlaylistCreated", handler);
    }
    public set PlaylistDeleted(handler: any) {
        this.AddEventListener("PlaylistDeleted", handler);
    }
    public set PlaylistNameUpdated(handler: any) {
        this.AddEventListener("PlaylistNameUpdated", handler);
    }
    public set PlaylistProtectionUpdated(handler: any) {
        this.AddEventListener("PlaylistProtectionUpdated", handler);
    }
    public set PlaylistSongsUpdated(handler: any) {
        this.AddEventListener("PlaylistSongsUpdated", handler);
    }
    public set SongUpdated(handler: any) {
        this.AddEventListener("SongUpdated", handler);
    }
    public set SongCreated(handler: any) {
        this.AddEventListener("SongCreated", handler);
    }
    public set SongNameUpdated(handler: any) {
        this.AddEventListener("SongNameUpdated", handler);
    }
    public set SongAuthorUpdated(handler: any) {
        this.AddEventListener("SongAuthorUpdated", handler);
    }
    public set SongPlayCountUpdated(handler: any) {
        this.AddEventListener("SongPlayCountUpdated", handler);
    }
    public set SongEpisodesUpdated(handler: any) {
        this.AddEventListener("SongEpisodesUpdated", handler);
    }
    public set SongPlaylistsUpdated(handler: any) {
        this.AddEventListener("SongPlaylistsUpdated", handler);
    }
    public set SongDownloadStarted(handler: any) {
        this.AddEventListener("SongDownloadStarted", handler);
    }
    public set SongDownloadEnded(handler: any) {
        this.AddEventListener("SongDownloadEnded", handler);
    }
    public set UserUpdated(handler: any) {
        this.AddEventListener("UserUpdated", handler);
    }
    public set UserNameUpdated(handler: any) {
        this.AddEventListener("UserNameUpdated", handler);
    }
    public set UserPlayerSelected(handler: any) {
        this.AddEventListener("UserPlayerSelected", handler);
    }
    public set UserAdministratorStateUpdated(handler: any) {
        this.AddEventListener("UserAdministratorStateUpdated", handler);
    }
}
