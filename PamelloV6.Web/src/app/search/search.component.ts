import { Component, EventEmitter, Output } from '@angular/core';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { CommonModule } from '@angular/common';
import { MiniPlaylistComponent } from "../mini-playlist/mini-playlist.component";
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloSong } from '../../services/api/model/PamelloSong';
import { IPamelloPlaylist } from '../../services/api/model/PamelloPlaylist';
import { SearchResult } from '../../services/api/pamelloV6DataAPI';

@Component({
	selector: 'app-search',
	standalone: true,
	imports: [MiniSongComponent, CommonModule, MiniPlaylistComponent],
	templateUrl: './search.component.html',
	styleUrl: './search.component.scss'
})
export class SearchComponent {
	private readonly api: PamelloV6API;

	@Output() public selectedSongChanged: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();
	@Output() public selectedPlaylistChanged: EventEmitter<IPamelloPlaylist> = new EventEmitter<IPamelloPlaylist>();

	public currentCategoryLabel: "Songs" | "Playlists" | "Youtube";

	public songsResults: SearchResult<IPamelloSong>;
	public playlistResults: SearchResult<IPamelloPlaylist>;
	public youtubeResults: SearchResult<any>;

	public currentResults: SearchResult<any>;

	public q: string = "";

	public constructor(api: PamelloV6API) {
		this.api = api;

		this.songsResults = new SearchResultObject<IPamelloSong>();
		this.playlistResults = new SearchResultObject<IPamelloPlaylist>();
		this.youtubeResults = new SearchResultObject<any>();

		this.currentCategoryLabel = "Songs";
		this.currentResults = this.songsResults;

		this.SearchSongs(0, "");
		this.SearchPlaylists(0, "");

		this.SwitchCategory("Playlists");
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

		if (this.currentCategoryLabel == "Songs") {
			this.currentResults = this.songsResults;
		}
		else if (this.currentCategoryLabel == "Playlists") {
			this.currentResults = this.playlistResults;
		}
		else if (this.currentCategoryLabel == "Youtube") {
			this.currentResults = this.youtubeResults;
		}
	}

	public setQ(q: Event) {
		this.q = (q.target as HTMLInputElement).value;
	}

	public async PrevPage() {
		if (this.currentResults.page <= 0) return;

		if (this.currentCategoryLabel == "Songs") {
			await this.SearchSongs(this.songsResults.page - 1, this.songsResults.query);
		}
		else if (this.currentCategoryLabel == "Playlists") {
			await this.SearchSongs(this.playlistResults.page - 1, this.playlistResults.query);
		}
	}
	public async NextPage() {
		if (this.currentResults.page >= this.currentResults.pagesCount - 1) return;

		if (this.currentCategoryLabel == "Songs") {
			await this.SearchSongs(this.songsResults.page + 1, this.songsResults.query);
		}
		else if (this.currentCategoryLabel == "Playlists") {
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
	}

	public async SearchSongs(page: number, q: string | null) {
		let result = await this.api.data.SearchSongs(page, 30, q ?? "");
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
		let result = await this.api.data.SearchPlaylists(page, 30, q ?? "");
		if (result == null) {
			this.playlistResults = new SearchResultObject<IPamelloPlaylist>();
			return;
		}

		this.playlistResults.page = result.page;
		this.playlistResults.pagesCount = result.pagesCount;
		this.playlistResults.results = result.results;
		this.playlistResults.query = result.query;
	}
}

class SearchResultObject<T> implements SearchResult<T> {
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
