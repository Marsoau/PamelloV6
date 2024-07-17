import { Component, EventEmitter, Output } from '@angular/core';
import { PamelloPlaylist, PamelloSong, PamelloV6API, SearchResult } from '../../services/pamelloV6API.service';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-search',
	standalone: true,
	providers: [PamelloV6API],
	imports: [MiniSongComponent, CommonModule],
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

		this.currentCategoryLabel = "Songs";

		this.songsResults = new Search<PamelloSong>();
		this.playlistResults = new Search<PamelloPlaylist>();
		this.youtubeResults = new Search<any>();

		this.currentResults = this.songsResults;

		this.SearchSongs(0, "");
	}

	public SwitchCategory() {
		if (this.currentCategoryLabel == "Songs") {
			this.currentCategoryLabel = "Playlists";
			this.currentResults = this.playlistResults;
		}
		else if (this.currentCategoryLabel == "Playlists") {
			this.currentCategoryLabel = "Youtube";
			this.currentResults = this.youtubeResults;
		}
		else if (this.currentCategoryLabel == "Youtube") {
			this.currentCategoryLabel = "Songs";
			this.currentResults = this.songsResults;
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
	}
	public async NextPage() {
		if (this.currentResults.page >= this.currentResults.pageCount - 1) return;

		if (this.currentCategoryLabel == "Songs") {
			await this.SearchSongs(this.songsResults.page + 1, this.songsResults.query);
		}
	}

	public async Search() {
		await this.SearchSongs(0, this.q);
	}

	public async SearchSongs(page: number, q: string | null) {
		let result = await this.api.data.SearchSongs(page, 30, q ?? "");

		this.songsResults.page = result.page;
		this.songsResults.pageCount = result.pagesCount;
		this.songsResults.results = result.results;
		this.songsResults.query = result.query;
	}
}

class Search<T> {
	public page: number = 0;
	public pageCount: number = 0;
	public results: T[] = [];
	public query: string = "";
}
