import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { lastValueFrom, map } from "rxjs";

@Injectable({
    providedIn: 'root',
})
export class PamelloV6API {
    private readonly http: HttpClient;

    authorizedUser: PamelloUser | null;
    authorizedUserToken: string | null;
    selectedPlayer: PamelloPlayer | null;
    
    public readonly data: PamelloV6Data;
    public readonly events: PamelloV6Events;
    public readonly commands: PamelloV6Commands;

    public constructor(http: HttpClient) {
        this.http = http;

        this.data = new PamelloV6Data(http);
        this.events = new PamelloV6Events();
        this.commands = new PamelloV6Commands(http);

        this.authorizedUser = null;
        this.selectedPlayer = null;
        this.authorizedUserToken = null;

        this.LoadTokenFromCookies();
        this.AuthorizeUserWithToken();

        this.events.UserPlayerSelected = async (playerId: number) => {
            this.selectedPlayer = await this.data.GetPlayer(playerId);
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
        let obsUser = this.http.get<PamelloUser | null>(`https://localhost:58631/Data/User?token=${this.authorizedUserToken}`);
        this.authorizedUser = await lastValueFrom(obsUser);

        console.log(this.authorizedUser);
    }

    public async AuthorizeUserWithCode(code: number) {
        let obsToken = this.http.get<string | null>(`https://localhost:58631/Authorization/GetToken?code=${code}`);
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
        let obs = this.http.get<PamelloUser>(`https://localhost:58631/Data/User?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetSong(id: number) {
        let obs = this.http.get<PamelloSong>(`https://localhost:58631/Data/Song?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetEpisode(id: number) {
        let obs = this.http.get<PamelloEpisode>(`https://localhost:58631/Data/Episode?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetPlaylist(id: number) {
        let obs = this.http.get<PamelloPlaylist>(`https://localhost:58631/Data/Playlist?id=${id}`);
        return await lastValueFrom(obs);
    }
    public async GetPlayer(id: number) {
        let obs = this.http.get<PamelloPlayer>(`https://localhost:58631/Data/Player?id=${id}`);
        return await lastValueFrom(obs);
    }
}
class PamelloV6Events {
    private readonly events: EventSource;
    
    public constructor() {
        this.events = new EventSource(`https://localhost:58631/Events?as=690D0DDB-57D6-4265-9816-EA5C05FFE8D0`);
    }

    public set PlayerCreated(handler: any) {
        this.events.addEventListener("PlayerCreated", (message) => {
            handler(JSON.parse(message.data));
        })
    }
    public set UserPlayerSelected(handler: any) {
        this.events.addEventListener("UserPlayerSelected", (message) => {
            handler(JSON.parse(message.data));
        })
    }
}
class PamelloV6Commands {
    private readonly http: HttpClient;
    
    constructor(http: HttpClient) {
        this.http = http;
    }
}


class PamelloUser {
    public id!: number;
    public name!: string;
    public coverUrl!: string;
    public discordId!: number;
    public selectedPlayerId!: number | null;
    public isAdministrator!: boolean;
    public ownedPlaylistIds!: number[];
}
class PamelloSong {
    public id!: number;
    public title!: string;
    public author!: string;
    public coverUrl!: string;
    public sourceUrl!: string;
    public playCount!: number;
    public isDownloaded!: boolean;
    public episodeIds!: number[];
    public playlistIds!: number[];
}
class PamelloEpisode {
    public id!: number;
    public songId!: number;
    public name!: string;
    public start!: number;
    public skip!: boolean;
}
class PamelloPlaylist {
    public id!: number;
    public name!: string;
    public ownerId!: number;
    public isProtected!: boolean;
    public songIds!: number;
}
class PamelloPlayer {
    public id!: number;
    public name!: string;
    public isPaused!: boolean;
    public currentSongTimePassed!: number;
    public currentSongTimeTotal!: number;
    public queueSongIds!: number[];
    public queuePosition!: number;
    public nextPositionRequest!: number | null;
    public queueIsRandom!: boolean;
    public queueIsReversed!: boolean;
    public queueIsNoLeftovers!: boolean;
}
