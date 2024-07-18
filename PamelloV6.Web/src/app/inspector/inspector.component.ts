import { Component, Input } from '@angular/core';
import { PamelloEpisode, PamelloPlaylist, PamelloSong, PamelloV6API } from '../../services/pamelloV6API.service';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { MiniPlaylistComponent } from "../mini-playlist/mini-playlist.component";

@Component({
	selector: 'app-inspector',
	standalone: true,
	imports: [MiniSongComponent, MiniPlaylistComponent],
	templateUrl: './inspector.component.html',
	styleUrl: './inspector.component.scss'
})
export class InspectorComponent {
	public displayStyle: "Song" | "Playlist";

	public songs: PamelloSong[];
	public episodes: PamelloEpisode[];
	public playlists: PamelloPlaylist[];

	public inspectedSong: PamelloSong | null;
	public inspectedPlaylist: PamelloPlaylist | null;

	public constructor(
		public readonly api: PamelloV6API
	) {
		this.inspectedSong = null;
		this.inspectedPlaylist = null;

		this.displayStyle = "Song";

		this.songs = [];
		this.episodes = [];
		this.playlists = [];

		this.InspectPlaylistId(1);
	}

	public async InspectSongId(songId: number) {
		this.InspectSong(await this.api.data.GetSong(songId));
	}
	public InspectSong(song: PamelloSong) {
		this.displayStyle = "Song";
		this.inspectedSong = song;

		this.LoadEpisodes();
		this.LoadPlaylists();
	}

	public async InspectPlaylistId(playlistId: number) {
		this.InspectPlaylist(await this.api.data.GetPlaylist(playlistId));
	}
	public InspectPlaylist(playlist: PamelloPlaylist) {
		this.displayStyle = "Playlist";
		this.inspectedPlaylist = playlist;
		
		this.LoadSongs();
	}

	public LoadPlaylists() {
		this.playlists = [];
		if (!this.inspectedSong) return;

		let defaultPlaylist = new PamelloPlaylist();
		defaultPlaylist.name = "Loadfing...";
		
		for (let i = 0; i < this.inspectedSong.playlistIds.length; i++) {
			this.playlists.push(defaultPlaylist);
			this.api.data.GetPlaylist(this.inspectedSong.playlistIds[i]).then(playlist => {
				this.playlists[i] = playlist;
			})
		}
	}
	public LoadEpisodes() {
		this.episodes = [];
		if (!this.inspectedSong) return;

		let defaultEpisode = new PamelloEpisode();
		defaultEpisode.name = "Loadfing...";
		
		for (let i = 0; i < this.inspectedSong.episodeIds.length; i++) {
			this.episodes.push(defaultEpisode);
			this.api.data.GetEpisode(this.inspectedSong.episodeIds[i]).then(episode => {
				this.episodes[i] = episode;
			})
		}
	}
	public LoadSongs() {
		this.songs = [];
		if (!this.inspectedPlaylist) return;

		let defaultSong = new PamelloSong();
		defaultSong.title = "Loadfing...";

		for (let i = 0; i < this.inspectedPlaylist.songIds.length; i++) {
			this.songs.push(defaultSong);
			this.api.data.GetSong(this.inspectedPlaylist.songIds[i]).then(song => {
				this.songs[i] = song;
			})
		}
	}
}
