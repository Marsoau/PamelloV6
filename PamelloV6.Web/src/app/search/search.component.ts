import { Component, EventEmitter, Output } from '@angular/core';
import { PamelloPlaylist, PamelloSong, PamelloV6API, SearchResult } from '../../services/pamelloV6API.service';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { CommonModule } from '@angular/common';
import { MiniPlaylistComponent } from "../mini-playlist/mini-playlist.component";

@Component({
	selector: 'app-search',
	standalone: true,
	providers: [PamelloV6API],
	imports: [MiniSongComponent, CommonModule, MiniPlaylistComponent],
	templateUrl: './search.component.html',
	styleUrl: './search.component.scss'
})
export class SearchComponent {
	private readonly api: PamelloV6API;

	@Output() public selectedSongChanged: EventEmitter<PamelloSong> = new EventEmitter<PamelloSong>();
	@Output() public selectedPlaylistChanged: EventEmitter<PamelloPlaylist> = new EventEmitter<PamelloPlaylist>();

	public currentCategoryLabel: "Songs" | "Playlists" | "Youtube";

	public songsResults: Search<PamelloSong>;
	public playlistResults: Search<PamelloPlaylist>;
	public youtubeResults: Search<any>;

	public currentResults: Search<any>;

	public q: string = "";

	public constructor(api: PamelloV6API) {
		this.api = api;

		this.songsResults = new Search<PamelloSong>();
		this.playlistResults = new Search<PamelloPlaylist>();
		this.youtubeResults = new Search<any>();

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
		if (this.currentResults.page >= this.currentResults.pageCount - 1) return;

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

		this.songsResults.page = result.page;
		this.songsResults.pageCount = result.pagesCount;
		this.songsResults.results = result.results;
		this.songsResults.query = result.query;
	}
	public async SearchPlaylists(page: number, q: string | null) {
		let result = await this.api.data.SearchPlaylists(page, 30, q ?? "");

		this.playlistResults.page = result.page;
		this.playlistResults.pageCount = result.pagesCount;
		this.playlistResults.results = result.results;
		this.playlistResults.query = result.query;
	}
}

class Search<T> {
	public page: number = 0;
	public pageCount: number = 0;
	public results: T[] = [];
	public query: string = "";
}
