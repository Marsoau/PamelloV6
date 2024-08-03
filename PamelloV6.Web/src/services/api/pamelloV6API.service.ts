import { Injectable } from "@angular/core";
import { IPamelloUser } from "./model/PamelloUser";
import { PamelloV6DataAPI } from "./pamelloV6DataAPI";
import { PamelloV6EventsAPI } from "./pamelloV6EventsAPI";
import { PamelloV6CommandsAPI } from "./pamelloV6CommandsAPI";
import { HttpClient } from "@angular/common/http";
import { PamelloV6Http } from "./pamelloV6Http";
import { IPamelloPlayer } from "./model/PamelloPlayer";
import { IPamelloSong } from "./model/PamelloSong";

@Injectable({
    providedIn: 'root',
})
export class PamelloV6API {
    public user: IPamelloUser | null;
    public selectedPlayer: IPamelloPlayer | null;
    public selectedPlayerSong: IPamelloSong | null;
    public selectedPlayerQueueSongs: IPamelloSong[];

    public readonly http: PamelloV6Http;
    public readonly data: PamelloV6DataAPI;
    public readonly events: PamelloV6EventsAPI;
    public readonly commands: PamelloV6CommandsAPI;

    public constructor(
        private readonly _http: HttpClient
    ) {
        this.user = null;
        this.selectedPlayer = null;
        this.selectedPlayerSong = null;
        this.selectedPlayerQueueSongs = [];

        this.http = new PamelloV6Http(this, _http);
        this.data = new PamelloV6DataAPI(this);
        this.events = new PamelloV6EventsAPI(this);
        this.commands = new PamelloV6CommandsAPI(this);

        this.Startup();
    }

    public get token() {
        return localStorage.getItem("token");
    }
    public set token(value: string | null) {
        localStorage.setItem("token", value ?? "");
    }

    public get isConnected() {
        return this.events.isConnected;
    }

    private async Startup() {
        console.log("Starting up API service");

        this.Connect((errorMessage: string) => {
            console.log("Starting up fail:", errorMessage);
        });

        console.log("API startup end");
    }

    public async AcquireTokenWithCode(code: number, onerror: any = null) {
        this.token = await this.http.Get<string>(`Authorization/GetToken?code=${code}`, onerror);
        
        if (this.token) return true;
        return false;
    }
    public async Connect(onerror: any = null) {
        if (!this.token) {
            if (onerror) onerror("No token was found");

            this.ResetAuthorizedUserData(true);
            return false;
        }

        let userAuthorizationResult = await this.UpdateAuthorizedUserData();
        if (!userAuthorizationResult) {
            if (onerror) onerror("Failed get user by token");
            
            this.ResetAuthorizedUserData(true);
            return false;
        }
        
        this.events.Connect(() => {}, () => {
            if (onerror) onerror("Failed to connect");
            this.Disonnect();
        });

        return true;
    }
    public Disonnect() {
        this.events.Disconnect();

        this.ResetAuthorizedUserData(true);
    }

    private async UpdateAuthorizedUserData() {
        console.log("Updating authorized user data");
        this.ResetAuthorizedUserData();

        this.user = await this.data.GetAuhorizedUser();
        if (!this.user) return false;
        console.log(`Loaded authorized user "${this.user.name}"`);

        this.LoadSelectedPlayer();

        console.log("Updated authorized user data successfully");
        return true;
    }
    private ResetAuthorizedUserData(resetToken: boolean = false) {
        if (resetToken) {
            this.token = "";
        }
        this.user = null;
        this.selectedPlayer = null;
        this.selectedPlayerSong = null;
        this.selectedPlayerQueueSongs = [];
    }

    public async LoadSelectedPlayer() {
        if (!this.user) return;

        if (!this.user.selectedPlayerId) {
            this.selectedPlayer = null;

            this.LoadSelectedPlayerSong();
            this.LoadSelectedPlayerQueueSongs();

            console.log(`Authorized user doesnt have selected player`);
            return;
        }

        this.selectedPlayer = await this.data.GetPlayer(this.user.selectedPlayerId);
        if (!this.selectedPlayer) {
            console.log(`Failed to load player with id ${this.user.selectedPlayerId}`);
            return;
        }

        console.log(`Loaded selected player "${this.selectedPlayer.name}"`);

        this.LoadSelectedPlayerSong();
        this.LoadSelectedPlayerQueueSongs();
    }

    public async LoadSelectedPlayerQueueSongs() {
        this.selectedPlayerQueueSongs = [];

        if (!this.selectedPlayer?.queueSongIds) return;

        for (let songId of this.selectedPlayer.queueSongIds) {
            let song = await this.data.GetSong(songId);
            if (song) this.selectedPlayerQueueSongs.push(song);
        }
    }

    public async LoadSelectedPlayerSong() {
		if (!this.selectedPlayer?.currentSongId) {
            this.selectedPlayerSong = null;
        }
		else if (this.selectedPlayerSong?.id != this.selectedPlayer.currentSongId) {
			this.selectedPlayerSong = await this.data.GetSong(this.selectedPlayer.currentSongId);
		}
    }
}

