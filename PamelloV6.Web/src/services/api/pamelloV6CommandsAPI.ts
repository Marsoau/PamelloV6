import { HttpHeaders } from "@angular/common/http";
import { PamelloV6API } from "./pamelloV6API.service";

export class PamelloV6CommandsAPI {
    public constructor(
        private readonly _api: PamelloV6API
    ) {

    }

	public async PlayerCreate(playerName: string) {
		return await this._api.http.InvokeCommand(`PlayerCreate&playerName=${playerName}`) as number;
	}
	public async PlayerDeleteSelected() {
		return await this._api.http.InvokeCommand(`PlayerDeleteSelected`);
	}
	public async PlayerSelect(playerId: number | null) {
		return await this._api.http.InvokeCommand(`PlayerSelect&playerId=${playerId ?? ""}`);
	}
	public async PlayerConnect() {
		return await this._api.http.InvokeCommand(`PlayerConnect`);
	}
	public async PlayerDisconnect(speakerPosition: number) {
		return await this._api.http.InvokeCommand(`PlayerDisconnect&speakerPosition=${speakerPosition}`);
	}
	public async PlayerRename(newName: string) {
		return await this._api.http.InvokeCommand(`PlayerRename&newName=${newName}`);
	}
	public async PlayerNext() {
		return await this._api.http.InvokeCommand(`PlayerNext`) as number;
	}
	public async PlayerPrev() {
		return await this._api.http.InvokeCommand(`PlayerPrev`) as number;
	}
	public async PlayerSkip() {
		return await this._api.http.InvokeCommand(`PlayerSkip`) as number | null;
	}
	public async PlayerGoToSong(songPosition: number, returnBack: boolean) {
		return await this._api.http.InvokeCommand(`PlayerGoToSong&songPosition=${songPosition}&returnBack=${returnBack}`) as number;
	}
	public async PlayerPause() {
		return await this._api.http.InvokeCommand(`PlayerPause`);
	}
	public async PlayerResume() {
		return await this._api.http.InvokeCommand(`PlayerResume`);
	}
	public async PlayerRewind(seconds: number) {
		return await this._api.http.InvokeCommand(`PlayerRewind&seconds=${seconds}`);
	}
	public async PlayerRewindToEpisode(episodePosition: number) {
		return await this._api.http.InvokeCommand(`PlayerRewindToEpisode&episodePosition=${episodePosition}`);
	}
	public async PlayerQueueRandom(value: boolean) {
		return await this._api.http.InvokeCommand(`PlayerQueueRandom&value=${value}`);
	}
	public async PlayerQueueReversed(value: boolean) {
		return await this._api.http.InvokeCommand(`PlayerQueueReversed&value=${value}`);
	}
	public async PlayerQueueNoLeftovers(value: boolean) {
		return await this._api.http.InvokeCommand(`PlayerQueueNoLeftovers&value=${value}`);
	}
	public async PlayerQueueClear() {
		return await this._api.http.InvokeCommand(`PlayerQueueClear`);
	}
	public async PlayerQueueAddSong(songId: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueAddSong&songId=${songId}`);
	}
	public async PlayerQueueInsertSong(queuePosition: number, songId: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueInsertSong&queuePosition=${queuePosition}&songId=${songId}`);
	}
	public async PlayerQueueAddYoutubeSong(youtubeId: string) {
		return await this._api.http.InvokeCommand(`PlayerQueueAddYoutubeSong&youtubeId=${youtubeId}`);
	}
	public async PlayerQueueInsertYoutubeSong(queuePosition: number, youtubeId: string) {
		return await this._api.http.InvokeCommand(`PlayerQueueInsertYoutubeSong&queuePosition=${queuePosition}&youtubeId=${youtubeId}`);
	}
	public async PlayerQueueAddPlaylist(playlistId: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueAddPlaylist&playlistId=${playlistId}`);
	}
	public async PlayerQueueInsertPlaylist(queuePosition: number, playlistId: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueInsertPlaylist&queuePosition=${queuePosition}&playlistId=${playlistId}`);
	}
	public async PlayerQueueRemoveSong(songPosition: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueRemoveSong&songPosition=${songPosition}`) as number;
	}
	public async PlayerQueueRequestNext(position: number | null) {
		return await this._api.http.InvokeCommand(`PlayerQueueRequestNext&position=${position ?? ''}`);
	}
	public async PlayerQueueSwap(inPosition: number, withPosition: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueSwap&inPosition=${inPosition}&withPosition=${withPosition}`);
	}
	public async PlayerQueueMove(fromPosition: number, toPosition: number) {
		return await this._api.http.InvokeCommand(`PlayerQueueMove&fromPosition=${fromPosition}&toPosition=${toPosition}`);
	}
	public async SongAddYoutube(youtubeId: string) {
		return await this._api.http.InvokeCommand(`SongAddYoutube&youtubeId=${youtubeId}`) as number;
	}
	public async SongEditName(songId: number, newName: string) {
		return await this._api.http.InvokeCommand(`SongEditName&songId=${songId}&newName=${newName}`);
	}
	public async SongEditAuthor(songId: number, newAuthor: string) {
		return await this._api.http.InvokeCommand(`SongEditAuthor&songId=${songId}&newAuthor=${newAuthor}`);
	}
	public async SongMoveEpisode(songId: number, fromPosition: number, toPosition: number) {
		return await this._api.http.InvokeCommand(`SongMoveEpisode&songId=${songId}&fromPosition=${fromPosition}&toPosition=${toPosition}`);
	}
	public async SongSwapEpisode(songId: number, fromPosition: number, withPosition: number) {
		return await this._api.http.InvokeCommand(`SongSwapEpisode&songId=${songId}&fromPosition=${fromPosition}&withPosition=${withPosition}`);
	}
	public async PlaylistAdd(playlistName: string, isProtected: boolean) {
		return await this._api.http.InvokeCommand(`PlaylistAdd&playlistName=${playlistName}&isProtected=${isProtected}`) as number;
	}
	public async PlaylistRename(playlistId: number, newName: string) {
		return await this._api.http.InvokeCommand(`PlaylistRename&playlistId=${playlistId}&newName=${newName}`);
	}
	public async PlaylistChangeProtection(playlistId: number, protection: boolean) {
		return await this._api.http.InvokeCommand(`PlaylistChangeProtection&playlistId=${playlistId}&protection=${protection}`);
	}
	public async PlaylistDelete(playlistId: number) {
		return await this._api.http.InvokeCommand(`PlaylistDelete&playlistId=${playlistId}`);
	}
	public async PlaylistAddSong(playlistId: number, songId: number) {
		return await this._api.http.InvokeCommand(`PlaylistAddSong&playlistId=${playlistId}&songId=${songId}`);
	}
	public async PlaylistInsertSong(playlistId: number, position: number, songId: number) {
		return await this._api.http.InvokeCommand(`PlaylistInsertSong&playlistId=${playlistId}&position=${position}&songId=${songId}`);
	}
	public async PlaylistAddYoutubeSong(playlistId: number, youtubeId: string) {
		return await this._api.http.InvokeCommand(`PlaylistAddYoutubeSong&playlistId=${playlistId}&youtubeId=${youtubeId}`);
	}
	public async PlaylistInsertYoutubeSong(playlistId: number, position: number, youtubeId: string) {
		return await this._api.http.InvokeCommand(`PlaylistInsertYoutubeSong&playlistId=${playlistId}&position=${position}&youtubeId=${youtubeId}`);
	}
	public async PlaylistAddPlaylistSongs(toPlaylistId: number, fromPlaylistId: number) {
		return await this._api.http.InvokeCommand(`PlaylistAddPlaylistSongs&toPlaylistId=${toPlaylistId}&fromPlaylistId=${fromPlaylistId}`);
	}
	public async PlaylistInsertPlaylistSongs(toPlaylistId: number, position: number, fromPlaylistId: number) {
		return await this._api.http.InvokeCommand(`PlaylistInsertPlaylistSongs&toPlaylistId=${toPlaylistId}&position=${position}&fromPlaylistId=${fromPlaylistId}`);
	}
	public async PlaylistMoveSong(playlistId: number, fromPosition: number, toPosition: number) {
		return await this._api.http.InvokeCommand(`PlaylistMoveSong&playlistId=${playlistId}&fromPosition=${fromPosition}&toPosition=${toPosition}`);
	}
	public async PlaylistSwapSong(playlistId: number, inPosition: number, withPosition: number) {
		return await this._api.http.InvokeCommand(`PlaylistSwapSong&playlistId=${playlistId}&fromPosition=${inPosition}&withPosition=${withPosition}`);
	}
	public async PlaylistRemoveSong(playlistId: number, songId: number) {
		return await this._api.http.InvokeCommand(`PlaylistRemoveSong&playlistId=${playlistId}&songId=${songId}`);
	}
	public async PlaylistRemoveSongAt(playlistId: number, songPosition: number) {
		return await this._api.http.InvokeCommand(`PlaylistRemoveSongAt&playlistId=${playlistId}&songPosition=${songPosition}`);
	}
	public async EpisodeAdd(songId: number, episodeName: string, startSeconds: number, skip: boolean) {
		return await this._api.http.InvokeCommand(`EpisodeAdd&songId=${songId}&episodeName=${episodeName}&startSeconds=${startSeconds}&skip=${skip}`) as number;
	}
	public async EpisodeRename(episodeId: number, newName: string) {
		return await this._api.http.InvokeCommand(`EpisodeRename&episodeId=${episodeId}&newName=${newName}`);
	}
	public async EpisodeChangeStart(episodeId: number, newStart: number) {
		return await this._api.http.InvokeCommand(`EpisodeChangeStart&episodeId=${episodeId}&newStart=${newStart}`);
	}
	public async EpisodeChangeSkipState(episodeId: number, newState: boolean) {
		return await this._api.http.InvokeCommand(`EpisodeChangeSkipState&episodeId=${episodeId}&newState=${newState}`);
	}
	public async EpisodeDelete(episodeId: number) {
		return await this._api.http.InvokeCommand(`EpisodeDelete&episodeId=${episodeId}`);
	}
}
