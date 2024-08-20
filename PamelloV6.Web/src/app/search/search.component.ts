import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { CommonModule } from '@angular/common';
import { MiniPlaylistComponent } from "../mini-playlist/mini-playlist.component";
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloSong } from '../../services/api/model/PamelloSong';
import { IPamelloPlaylist } from '../../services/api/model/PamelloPlaylist';
import { ReorderItemComponent } from "../reorder-item/reorder-item.component";
import { ISearchResult } from '../../services/api/model/search/SearchResult';
import { IYoutubeSearchResult } from '../../services/api/model/search/YoutubeSearchResult';
import { IYoutubeSearchVideoInfo } from '../../services/api/model/search/YoutubeSearchVideoInfo';
import { MiniYoutubeVideoComponent } from "../mini-youtube-video/mini-youtube-video.component";

@Component({
	selector: 'app-search',
	standalone: true,
	imports: [MiniSongComponent, CommonModule, MiniPlaylistComponent, ReorderItemComponent, MiniYoutubeVideoComponent],
	templateUrl: './search.component.html',
	styleUrl: './search.component.scss'
})
export class SearchComponent {
	private readonly api: PamelloV6API;

	@Output() public selectedSongChanged: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();
	@Output() public selectedPlaylistChanged: EventEmitter<IPamelloPlaylist> = new EventEmitter<IPamelloPlaylist>();
	@Output() public addSongToPlaylistClick: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();
	@Output() public addYoutubeSongToPlaylistClick: EventEmitter<string> = new EventEmitter<string>();

	@Input() public displayAddToPlaylistButton: boolean = false;

	public currentCategoryLabel: "Songs" | "Playlists" | "Youtube";

	public songsResults: ISearchResult<IPamelloSong>;
	public playlistResults: ISearchResult<IPamelloPlaylist>;
	public youtubeResults: IYoutubeSearchResult;

	public q: string = "";

	public pageSize: number = 25;

	public constructor(api: PamelloV6API) {
		this.api = api;

		this.songsResults = new SearchResultObject<IPamelloSong>();
		this.playlistResults = new SearchResultObject<IPamelloPlaylist>();
		this.youtubeResults = new YoutubeSearchResultObject();

		this.currentCategoryLabel = "Songs";

		this.SubscribeToEvents();

		this.SearchSongs(0, "");
		this.SearchPlaylists(0, "");
	}

	public SubscribeToEvents() {
		this.api.events.SongCreated = async (songId: number) => {
			this.SearchSongs(this.songsResults.page, this.songsResults.query);

			let newSong = await this.api.data.GetSong(songId);

			for (let video of this.youtubeResults.youtubeVideos) {
				if (video.id == newSong?.youtubeId) {
					let videoIndex = this.youtubeResults.youtubeVideos.indexOf(video);
					if (!videoIndex || Number.isNaN(videoIndex)) return;
					this.youtubeResults.youtubeVideos.splice(videoIndex, 1)

					this.youtubeResults.pamelloSongs.push(newSong);

					return;
				}
			}
		}
		this.api.events.PlaylistCreated = () => {
			this.SearchPlaylists(this.playlistResults.page, this.playlistResults.query);
		}
		this.api.events.PlaylistDeleted = () => {
			this.SearchPlaylists(this.playlistResults.page, this.playlistResults.query);
		}
	}

	public SwitchCategory(category: "Songs" | "Playlists" | "Youtube" | null = null) {
		if (category) {
			this.currentCategoryLabel = category;
		}
		else if (this.currentCategoryLabel == "Songs") {
			this.currentCategoryLabel = "Playlists";
		}
		else if (this.currentCategoryLabel == "Playlists") {
			this.currentCategoryLabel = "Youtube";
		}
		else if (this.currentCategoryLabel == "Youtube") {
			this.currentCategoryLabel = "Songs";
		}
	}

	public setQ(q: Event) {
		this.q = (q.target as HTMLInputElement).value;
	}

	public async PrevPage() {
		if (this.currentCategoryLabel == "Songs" && this.songsResults.page != 0) {
			await this.SearchSongs(this.songsResults.page - 1, this.songsResults.query);
		}
		else if (this.currentCategoryLabel == "Playlists" && this.playlistResults.page != 0) {
			await this.SearchSongs(this.playlistResults.page - 1, this.playlistResults.query);
		}
	}
	public async NextPage() {
		if (this.currentCategoryLabel == "Songs" && this.songsResults.page != this.songsResults.pagesCount - 1) {
			await this.SearchSongs(this.songsResults.page + 1, this.songsResults.query);
		}
		else if (this.currentCategoryLabel == "Playlists" && this.playlistResults.page != this.playlistResults.pagesCount - 1) {
			await this.SearchSongs(this.playlistResults.page + 1, this.playlistResults.query);
		}
	}

	public async Search() {
		if (this.currentCategoryLabel == "Songs") {
			await this.SearchSongs(0, this.q);
		}
		else if (this.currentCategoryLabel == "Playlists") {
			await this.SearchPlaylists(0, this.q);
		}
		else if (this.currentCategoryLabel == "Youtube") {
			await this.SearchYoutube(this.q);
		}
	}

	public async SearchSongs(page: number, q: string | null) {
		let result = await this.api.data.SearchSongs(page, this.pageSize, q ?? "");
		if (result == null) {
			this.songsResults = new SearchResultObject<IPamelloSong>();
			return;
		}

		this.songsResults.page = result.page;
		this.songsResults.pagesCount = result.pagesCount;
		this.songsResults.results = result.results;
		this.songsResults.query = result.query;
	}
	public async SearchPlaylists(page: number, q: string | null) {
		let result = await this.api.data.SearchPlaylists(page, this.pageSize, q ?? "");
		if (result == null) {
			this.playlistResults = new SearchResultObject<IPamelloPlaylist>();
			return;
		}

		this.playlistResults.page = result.page;
		this.playlistResults.pagesCount = result.pagesCount;
		this.playlistResults.results = result.results;
		this.playlistResults.query = result.query;
	}
	public async SearchYoutube(q: string | null) {
		let result = await this.api.data.SearchYoutubeSongs(this.pageSize, q ?? "");
		if (result == null) {
			this.youtubeResults = new YoutubeSearchResultObject();
			return;
		}

		console.log(result);

		this.youtubeResults.resultsCount = result.resultsCount;
		this.youtubeResults.pamelloSongs = result.pamelloSongs;
		this.youtubeResults.youtubeVideos = result.youtubeVideos;
	}

	public RemovePlaylist(playlist: IPamelloPlaylist) {
		if (confirm(`Delete playlist "${playlist.name}" from the database?`)) {
			this.api.commands.PlaylistDelete(playlist.id);
		}
	}
}

class SearchResultObject<T> implements ISearchResult<T> {
	page: number;
	pagesCount: number;
	results: T[];
	query: string;

	constructor() {
		this.page = 0;
		this.pagesCount = 0;
		this.results = [];
		this.query = "";
	}
}

class YoutubeSearchResultObject implements IYoutubeSearchResult {
    resultsCount: number;
    pamelloSongs: IPamelloSong[];
    youtubeVideos: IYoutubeSearchVideoInfo[];
	query: string;

	constructor() {
		this.resultsCount = 0;
		this.pamelloSongs = [];
		this.youtubeVideos = [];
		this.query = "";
	}
}
