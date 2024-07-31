import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, lastValueFrom, map } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class PamelloV6API {
    private readonly http: HttpClient;

    authorizedUser: PamelloUser | null;
    authorizedUserToken: string | null;
    
    public readonly data: PamelloV6Data;
    public readonly events: PamelloV6Events;
    public readonly commands: PamelloV6Commands;

    public constructor(http: HttpClient) {
        this.http = http;

        this.data = new PamelloV6Data(http);
        this.events = new PamelloV6Events(this);
        this.commands = new PamelloV6Commands(this, http);

        this.authorizedUser = null;
        this.authorizedUserToken = null;

        this.LoadTokenFromCookies();
        this.AuthorizeUserWithToken();
        this.events.ConnectEvents();

        this.events.UserPlayerSelected = async (playerId: number) => {
            this.authorizedUser!.selectedPlayerId = playerId;
        }
    }

    private LoadTokenFromCookies() {
        let cookies = `; ${document.cookie}`;
        let parts = cookies.split(`; token=`);
        let token;
        if (parts.length === 2) token = parts.pop()?.split(';').shift();
        else token = undefined;

        if (token == undefined || token.length == 0) {
            this.authorizedUserToken = null;
            return;
        }
        
        this.authorizedUserToken = token;
    }

    public async AuthorizeUserWithToken() {
        let obsUser = this.http.get<PamelloUser | null>(`https://188.47.60.95:58631/Data/User?token=${this.authorizedUserToken}`);
        this.authorizedUser = await lastValueFrom(obsUser);

        console.log(this.authorizedUser);
    }

    public async AuthorizeUserWithCode(code: number) {
        let obsToken = this.http.get<string | null>(`https://188.47.60.95:58631/Authorization/GetToken?code=${code}`);
        this.authorizedUserToken = await lastValueFrom(obsToken);

        if (this.authorizedUserToken == null) {
            this.authorizedUser = null;
            return;
        }
        
        document.cookie = `token=${this.authorizedUserToken}`
        this.AuthorizeUserWithToken();
    }

    public UnauthorizeUser() {
        document.cookie = `token=;`;
        this.authorizedUser = null;
        this.authorizedUserToken = null;
    }
}

class PamelloV6Data {
    private readonly http: HttpClient;
    
    test: string = "";

    constructor(http: HttpClient) {
        this.http = http;
    }

    public async GetUser(id: number) {
        let obs = this.http.get<PamelloUser>(`https://188.47.60.95:58631/Data/User?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetSong(id: number) {
        let obs = this.http.get<PamelloSong>(`https://188.47.60.95:58631/Data/Song?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetEpisode(id: number) {
        let obs = this.http.get<PamelloEpisode>(`https://188.47.60.95:58631/Data/Episode?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetPlaylist(id: number) {
        let obs = this.http.get<PamelloPlaylist>(`https://188.47.60.95:58631/Data/Playlist?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetPlayer(id: number) {
        let obs = this.http.get<PamelloPlayer>(`https://188.47.60.95:58631/Data/Player?id=${id}`);
        return await lastValueFrom(obs);
    }

    public async SearchSongs(page: number, count: number, query: string = "") {
        let obs = this.http.get<SearchResult<PamelloSong>>(`https://188.47.60.95:58631/Data/Songs/Search?q=${query}&page=${page}&count=${count}`);
        return await lastValueFrom(obs);
    }
    public async SearchPlaylists(page: number, count: number, query: string = "") {
        let obs = this.http.get<SearchResult<PamelloPlaylist>>(`https://188.47.60.95:58631/Data/Playlists/Search?q=${query}&page=${page}&count=${count}`);
        return await lastValueFrom(obs);
    }
    public async SearchPlayers(page: number, count: number, query: string = "") {
        let obs = this.http.get<SearchResult<PamelloPlayer>>(`https://188.47.60.95:58631/Data/Players/Search?q=${query}&page=${page}&count=${count}`);
        return await lastValueFrom(obs);
    }
}
class PamelloV6Events {
    private readonly api: PamelloV6API;
    private events!: EventSource;
    
    public constructor(api: PamelloV6API) {
        this.api = api;
    }

    public ConnectEvents() {
        this.events = new EventSource(`https://188.47.60.95:58631/Events?as=${this.api.authorizedUserToken}`);
    }

    public set PlayerCreated(handler: any) {
        this.events.addEventListener("PlayerCreated", (message) => {
            handler(JSON.parse(message.data));
        })
    }
    public set PlayerNameUpdated(handler: any) {
            this.events.addEventListener("PlayerNameUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerPauseUpdated(handler: any) {
            this.events.addEventListener("PlayerPauseUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerCurrentTimeUpdated(handler: any) {
            this.events.addEventListener("PlayerCurrentTimeUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerSpeakerConnected(handler: any) {
            this.events.addEventListener("PlayerSpeakerConnected", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerSpeakerDisconnected(handler: any) {
            this.events.addEventListener("PlayerSpeakerDisconnected", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerTotalTimeUpdated(handler: any) {
            this.events.addEventListener("PlayerTotalTimeUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlauerQueuePositionUpdated(handler: any) {
            this.events.addEventListener("PlauerQueuePositionUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlauerQueueSongUpdated(handler: any) {
            this.events.addEventListener("PlauerQueueSongUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlauerQueueListUpdated(handler: any) {
            this.events.addEventListener("PlauerQueueListUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerQueueIsRandomUpdated(handler: any) {
            this.events.addEventListener("PlayerQueueIsRandomUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerQueueIsReversedUpdated(handler: any) {
            this.events.addEventListener("PlayerQueueIsReversedUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerQueueIsNoLeftoversUpdated(handler: any) {
            this.events.addEventListener("PlayerQueueIsNoLeftoversUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlayerQueueNextPositionUpdated(handler: any) {
            this.events.addEventListener("PlayerQueueNextPositionUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set EpisodeUpdated(handler: any) {
            this.events.addEventListener("EpisodeUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set EpisodeCreated(handler: any) {
            this.events.addEventListener("EpisodeCreated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set EpisodeNameUpdated(handler: any) {
            this.events.addEventListener("EpisodeNameUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set EpisodeStartUpdated(handler: any) {
            this.events.addEventListener("EpisodeStartUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set EpisodeSkipStateUpdated(handler: any) {
            this.events.addEventListener("EpisodeSkipStateUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlaylistUpdated(handler: any) {
            this.events.addEventListener("PlaylistUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlaylistCreated(handler: any) {
            this.events.addEventListener("PlaylistCreated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlaylistNameUpdated(handler: any) {
            this.events.addEventListener("PlaylistNameUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlaylistProtectionUpdated(handler: any) {
            this.events.addEventListener("PlaylistProtectionUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set PlaylistSongsUpdated(handler: any) {
            this.events.addEventListener("PlaylistSongsUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongUpdated(handler: any) {
            this.events.addEventListener("SongUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongCreated(handler: any) {
            this.events.addEventListener("SongCreated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongNameUpdated(handler: any) {
            this.events.addEventListener("SongNameUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongAuthorUpdated(handler: any) {
            this.events.addEventListener("SongAuthorUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongPlayCountUpdated(handler: any) {
            this.events.addEventListener("SongPlayCountUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongDownloadStarted(handler: any) {
            this.events.addEventListener("SongDownloadStarted", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set SongDownloadEnded(handler: any) {
            this.events.addEventListener("SongDownloadEnded", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set UserUpdated(handler: any) {
            this.events.addEventListener("UserUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set UserNameUpdated(handler: any) {
            this.events.addEventListener("UserNameUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set UserPlayerSelected(handler: any) {
            this.events.addEventListener("UserPlayerSelected", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set UserAdministratorStateUpdated(handler: any) {
            this.events.addEventListener("UserAdministratorStateUpdated", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    public set (handler: any) {
            this.events.addEventListener("", (message) => {
                    handler(JSON.parse(message.data));
            })
    }
    
}
class PamelloV6Commands {
    private readonly api: PamelloV6API;
    private readonly http: HttpClient;
    
    constructor(api: PamelloV6API, http: HttpClient) {
        this.api = api;
        this.http = http;
    }

    private async InvokeCommand(commandString: string) {
        return await lastValueFrom(
            this.http.get(`https://188.47.60.95:58631/Command?name=${commandString}`, {
                headers: new HttpHeaders({
                    "user-token": this.api.authorizedUserToken ?? ""
                }) 
            })
        )
    }
    public async PlayerCreate(playerName: string): Promise<number> {
        return await this.InvokeCommand(`PlayerCreate&playerName=${playerName}`) as number;
    }
    public async PlayerSelect(playerId: number | null) {
        return await this.InvokeCommand(`PlayerSelect&playerId=${playerId ?? ''}`);
    }
    public async PlayerRename(newName: string) {
        return await this.InvokeCommand(`PlayerRename&newName=${newName}`);
    }
    public async PlayerNext(): Promise<number | null> {
        return await this.InvokeCommand(`PlayerNext`) as number | null;
    }
    public async PlayerPrev(): Promise<number | null> {
        return await this.InvokeCommand(`PlayerPrev`) as number | null;
    }
    public async PlayerSkip(): Promise<number | null> {
        return await this.InvokeCommand(`PlayerSkip`) as number | null;
    }
    public async PlayerGoToSong(songPosition: number, returnBack: boolean): Promise<number | null> {
        return await this.InvokeCommand(`PlayerGoToSong&songPosition=${songPosition}&returnBack=${returnBack}`) as number | null;
    }
    public async PlayerPause() {
        return await this.InvokeCommand(`PlayerPause`);
    }
    public async PlayerResume() {
        return await this.InvokeCommand(`PlayerResume`);
    }
    public async PlayerRewind(seconds: number) {
        return await this.InvokeCommand(`PlayerRewind&seconds=${seconds}`);
    }
    public async PlayerRewindToEpisode(episodePosition: number) {
        return await this.InvokeCommand(`PlayerRewindToEpisode&episodePosition=${episodePosition}`);
    }
    public async PlayerQueueRandom(value: boolean) {
        return await this.InvokeCommand(`PlayerQueueRandom&value=${value}`);
    }
    public async PlayerQueueReversed(value: boolean) {
        return await this.InvokeCommand(`PlayerQueueReversed&value=${value}`);
    }
    public async PlayerQueueNoLeftovers(value: boolean) {
        return await this.InvokeCommand(`PlayerQueueNoLeftovers&value=${value}`);
    }
    public async PlayerQueueClear() {
        return await this.InvokeCommand(`PlayerQueueClear`);
    }
    public async PlayerQueueAddSong(songId: number) {
        return await this.InvokeCommand(`PlayerQueueAddSong&songId=${songId}`);
    }
    public async PlayerQueueInsertSong(queuePosition: number, songId: number) {
        return await this.InvokeCommand(`PlayerQueueInsertSong&queuePosition=${queuePosition}&songId=${songId}`);
    }
    public async PlayerQueueRemoveSong(songPosition: number): Promise<number | null> {
        return await this.InvokeCommand(`PlayerQueueRemoveSong&songPosition=${songPosition}`) as number | null;
    }
    public async PlayerQueueRequestNext(position: number | null) {
        return await this.InvokeCommand(`PlayerQueueRequestNext&position=${position ?? ''}`);
    }
    public async PlayerQueueSwap(fromPosition: number, withPosition: number) {
        return await this.InvokeCommand(`PlayerQueueSwap&inPosition=${fromPosition}&withPosition=${withPosition}`);
    }
    public async PlayerQueueMove(fromPosition: number, toPosition: number) {
        return await this.InvokeCommand(`PlayerQueueMove&fromPosition=${fromPosition}&toPosition=${toPosition}`);
    }
    public async SongAddYoutube(youtubeId: string): Promise<number | null> {
        return await this.InvokeCommand(`SongAddYoutube&youtubeId=${youtubeId}`) as number | null;
    }
    public async SongEditName(songId: number, newName: string): Promise<boolean> {
        return await this.InvokeCommand(`SongEditName&songId=${songId}&newName=${newName}`) as boolean;
    }
    public async SongEditAuthor(songId: number, newAuthor: string): Promise<boolean> {
        return await this.InvokeCommand(`SongEditAuthor&songId=${songId}&newAuthor=${newAuthor}`) as boolean;
    }
    public async PlaylistAdd(playlistName: string, isProtected: boolean): Promise<number> {
        return await this.InvokeCommand(`PlaylistAdd&playlistName=${playlistName}&isProtected=${isProtected}`) as number;
    }
    public async PlaylistRename(playlistId: number, newName: string) {
        return await this.InvokeCommand(`PlaylistRename&playlistId=${playlistId}&newName=${newName}`);
    }
    public async PlaylistChangeProtection(playlistId: number, protection: boolean) {
        return await this.InvokeCommand(`PlaylistChangeProtection&playlistId=${playlistId}&protection=${protection}`);
    }
    public async PlaylistAddSong(playlistId: number, songId: number) {
        return await this.InvokeCommand(`PlaylistAddSong&playlistId=${playlistId}&songId=${songId}`);
    }
    public async PlaylistRemoveSong(playlistId: number, position: number) {
        return await this.InvokeCommand(`PlaylistRemoveSong&playlistId=${playlistId}&position=${position}`);
    }
    public async EpisodeAdd(songId: number, episodeName: string, startSeconds: number, skip: boolean): Promise<number> {
        return await this.InvokeCommand(`EpisodeAdd&songId=${songId}&episodeName=${episodeName}&startSeconds=${startSeconds}&skip=${skip}`) as number;
    }
    public async EpisodeRename(episodeId: number, newName: string) {
        return await this.InvokeCommand(`EpisodeRename&episodeId=${episodeId}&newName=${newName}`);
    }
    public async EpisodeChangeStart(episodeId: number, newStart: number) {
        return await this.InvokeCommand(`EpisodeChangeStart&episodeId=${episodeId}&newStart=${newStart}`);
    }
    public async EpisodeChangeSkipState(episodeId: number, newState: boolean) {
        return await this.InvokeCommand(`EpisodeChangeSkipState&episodeId=${episodeId}&newState=${newState}`);
    }
}


export class PamelloUser {
    public id!: number;
    public name!: string;
    public coverUrl!: string;
    public discordId!: number;
    public selectedPlayerId!: number | null;
    public isAdministrator!: boolean;
    public ownedPlaylistIds!: number[];
}
export class PamelloSong {
    public id!: number;
    public title!: string;
    public author!: string;
    public coverUrl!: string;
    public youtubeId!: string;
    public playCount!: number;
    public isDownloaded!: boolean;
    public episodeIds!: number[];
    public playlistIds!: number[];
}
export class PamelloEpisode {
    public id!: number;
    public songId!: number;
    public name!: string;
    public start!: number;
    public skip!: boolean;
}
export class PamelloPlaylist {
    public id!: number;
    public name!: string;
    public ownerId!: number;
    public isProtected!: boolean;
    public songIds!: number[];
}
export class PamelloPlayer {
    public id!: number;
    public name!: string;
    public isPaused!: boolean;
    public currentSongTimePassed!: number;
    public currentSongTimeTotal!: number;
    public currentSongId!: number | null;
    public queueSongIds!: number[];
    public queuePosition!: number;
    public nextPositionRequest!: number | null;
    public queueIsRandom!: boolean;
    public queueIsReversed!: boolean;
    public queueIsNoLeftovers!: boolean;
}
export class SearchResult<T> {
    page!: number;
    pagesCount!: number;
    results!: T[];
    query!: string;
}
