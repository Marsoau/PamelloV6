import { Component, Input } from '@angular/core';
import { MiniSongComponent } from "../mini-song/mini-song.component";
import { MiniPlaylistComponent } from "../mini-playlist/mini-playlist.component";
import { PageComponent } from "../page/page.component";
import { MultipageComponent } from "../multipage/multipage.component";
import { MiniEpisodeComponent } from "../mini-episode/mini-episode.component";
import { ReorderListComponent } from "../reorder-list/reorder-list.component";
import { ReorderItemComponent } from "../reorder-item/reorder-item.component";
import { FormsModule } from '@angular/forms';
import { IPamelloSong, PamelloSong } from '../../services/api/model/PamelloSong';
import { IPamelloEpisode, PamelloEpisode } from '../../services/api/model/PamelloEpisode';
import { IPamelloPlaylist, PamelloPlaylist } from '../../services/api/model/PamelloPlaylist';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-inspector',
	standalone: true,
	imports: [MiniSongComponent, MiniPlaylistComponent, PageComponent, MultipageComponent, MiniEpisodeComponent, ReorderListComponent, ReorderItemComponent, FormsModule],
	templateUrl: './inspector.component.html',
	styleUrl: './inspector.component.scss'
})
export class InspectorComponent {
	public displayStyle: "Song" | "Playlist";

	public songs: IPamelloSong[];
	public episodes: IPamelloEpisode[];
	public playlists: IPamelloPlaylist[];

	public inspectedSong: IPamelloSong | null;
	public inspectedPlaylist: IPamelloPlaylist | null;

	public newEpisodeNameInput: string;
	public newPlaylistNameInput: string;

	public constructor(
		public readonly api: PamelloV6API
	) {
		this.inspectedSong = null;
		this.inspectedPlaylist = null;

		this.displayStyle = "Song";

		this.songs = [];
		this.episodes = [];
		this.playlists = [];

		this.newEpisodeNameInput = "";
		this.newPlaylistNameInput = "";

		this.InspectSongId(2);
	}

	public async InspectSongId(songId: number) {
		this.InspectSong(await this.api.data.GetSong(songId));
	}
	public InspectSong(song: IPamelloSong | null) {
		this.displayStyle = "Song";
		this.inspectedSong = song;
		
		console.log(`inspecting song "${song?.title}"`);

		this.LoadEpisodes();
		this.LoadPlaylists();
	}

	public async InspectPlaylistId(playlistId: number) {
		this.InspectPlaylist(await this.api.data.GetPlaylist(playlistId));
	}
	public InspectPlaylist(playlist: IPamelloPlaylist | null) {
		this.displayStyle = "Playlist";
		this.inspectedPlaylist = playlist;
		
		console.log(`inspecting playlist "${playlist?.name}"`);

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
				if (playlist) this.playlists[i] = playlist;
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
				if (episode) this.episodes[i] = episode;
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
				if (song) this.songs[i] = song;
			})
		}
	}

	public AddEpisodeToSong() {
		if (!this.inspectedSong) return;

		this.api.commands.EpisodeAdd(this.inspectedSong.id, this.newEpisodeNameInput, 0, false);
	}
	public async AddPlaylistToSong() {
		if (!this.inspectedSong) return;

		let newPlaylistId = await this.api.commands.PlaylistAdd(this.newPlaylistNameInput, false);
		this.api.commands.PlaylistAddSong(newPlaylistId, this.inspectedSong.id);
	}
}
