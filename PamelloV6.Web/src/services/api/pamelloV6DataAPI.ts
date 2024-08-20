import { PamelloV6API } from "./pamelloV6API.service";
import { IPamelloUser } from "./model/PamelloUser";
import { IPamelloSong } from "./model/PamelloSong";
import { IPamelloEpisode } from "./model/PamelloEpisode";
import { IPamelloPlaylist } from "./model/PamelloPlaylist";
import { IPamelloPlayer } from "./model/PamelloPlayer";
import { ISearchResult } from "./model/search/SearchResult";
import { IYoutubeSearchResult } from "./model/search/YoutubeSearchResult";

export class PamelloV6DataAPI {
    public constructor(
        private readonly _api: PamelloV6API
    ) {

    }

    public async GetAuhorizedUser(onerror: any = null) {
        if (!this._api.token) {
            if (onerror) onerror("");
            return null;
        }
        return await this._api.http.Get<IPamelloUser>(`Data/User?token=${this._api.token}`, onerror);
    }

    public async GetUser(id: number, onerror: any = null) {
        return await this._api.http.Get<IPamelloUser>(`Data/User?id=${id}`, onerror);
    }
    public async GetSong(id: number, onerror: any = null) {
        return await this._api.http.Get<IPamelloSong>(`Data/Song?id=${id}`, onerror);
    }
    public async GetEpisode(id: number, onerror: any = null) {
        return await this._api.http.Get<IPamelloEpisode>(`Data/Episode?id=${id}`, onerror);
    }
    public async GetPlaylist(id: number, onerror: any = null) {
        return await this._api.http.Get<IPamelloPlaylist>(`Data/Playlist?id=${id}`, onerror);
    }
    public async GetPlayer(id: number, onerror: any = null) {
        return await this._api.http.Get<IPamelloPlayer>(`Data/Player?id=${id}`, onerror);
    }

    public async SearchUsers(page: number, count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<ISearchResult<IPamelloUser>>(`Data/Users/Search?q=${query}&page=${page}&count=${count}`, onerror);
    }
    public async SearchSongs(page: number, count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<ISearchResult<IPamelloSong>>(`Data/Songs/Search?q=${query}&page=${page}&count=${count}`, onerror);
    }
    public async SearchEpisodes(page: number, count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<ISearchResult<IPamelloEpisode>>(`Data/Episodes/Search?q=${query}&page=${page}&count=${count}`, onerror);
    }
    public async SearchPlaylists(page: number, count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<ISearchResult<IPamelloPlaylist>>(`Data/Playlists/Search?q=${query}&page=${page}&count=${count}`, onerror);
    }
    public async SearchPlayers(page: number, count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<ISearchResult<IPamelloPlayer>>(`Data/Players/Search?q=${query}&page=${page}&count=${count}`, onerror);
    }

    public async SearchYoutubeSongs(count: number, query: string = "", onerror: any = null) {
        return await this._api.http.Get<IYoutubeSearchResult>(`Data/Songs/SearchYoutube?q=${query}&count=${count}`, onerror);
    }
}
