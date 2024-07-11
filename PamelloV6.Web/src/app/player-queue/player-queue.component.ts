import { Component } from '@angular/core';
import { PamelloSong, PamelloV6API } from '../../pamelloV6API';

@Component({
	selector: 'app-player-queue',
	standalone: true,
	imports: [],
	templateUrl: './player-queue.component.html',
	styleUrl: './player-queue.component.scss'
})
export class PlayerQueueComponent {
	private readonly api: PamelloV6API;

	public songs: PamelloSong[];
	public position: number;
	public nextPosition: number | null;

	public isRandom: boolean;
	public isReversed: boolean;
	public isNoLeftovers: boolean;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.songs = [];
		this.position = 0;
		this.nextPosition = null;

		this.isRandom = false;
		this.isReversed = false;
		this.isNoLeftovers = false;

		this.SubscribeToEvents();
		this.Update();
	}

	private SubscribeToEvents() {
		this.api.events.UserPlayerSelected = (newPlayerId: number) => {
			this.Update();
		}
		this.api.events.PlauerQueueListUpdated = (newSongsIds: number[]) => {
			this.LoadSongsFromIds(newSongsIds);
		}
		this.api.events.PlauerQueuePositionUpdated = (newPosition: number) => {
			this.position = newPosition;
		}
		this.api.events.PlayerQueueNextPositionUpdated = (newNextPosition: number | null) => {
			this.nextPosition = newNextPosition;
		}

		this.api.events.PlayerQueueIsRandomUpdated = (newState: boolean) => {
			this.isRandom = newState;
		}
		this.api.events.PlayerQueueIsReversedUpdated = (newState: boolean) => {
			this.isReversed = newState;
		}
		this.api.events.PlayerQueueIsNoLeftoversUpdated = (newState: boolean) => {
			this.isNoLeftovers = newState;
		}
	}

	private Update() {
		if (this.api.authorizedUser!.selectedPlayerId != null) {
			this.api.data.GetPlayer(this.api.authorizedUser!.selectedPlayerId).then(player => {
				this.position = player.queuePosition;
				this.nextPosition = player.nextPositionRequest;
				this.LoadSongsFromIds(player.queueSongIds);

				this.isRandom = player.queueIsRandom;
				this.isReversed = player.queueIsReversed;
				this.isNoLeftovers = player.queueIsNoLeftovers;
			})
		}
		else {
			this.songs = [];
			this.position = 0;
		}
	}

	private LoadSongsFromIds(songsIds: number[]) {
		this.songs = [];

		let defaultSong = new PamelloSong();
		defaultSong.title = "Loadfing...";
		for (let i = 0; i < songsIds.length; i++) {
			this.songs.push(defaultSong);
			this.api.data.GetSong(songsIds[i]).then(song => {
				this.songs[i] = song;
			})
		}
	}

	public SongClick(songPosition: number) {
		if (songPosition == this.position) return;

		this.api.commands.PlayerGoToSong(songPosition, false);
	}
	public NextClick(songPosition: number) {
		if (songPosition == this.position) return;

		if (songPosition == this.nextPosition) {
			this.api.commands.PlayerQueueRequestNext(null);
		}
		else {
			console.log("asd");
			this.api.commands.PlayerQueueRequestNext(songPosition);
		}
	}
	public RemoveClick(songPosition: number) {
		this.api.commands.PlayerQueueRemoveSong(songPosition);
	}

	public RandomClick() {
		this.api.commands.PlayerQueueRandom(!this.isRandom);
	}
	public ReversedClick() {
		this.api.commands.PlayerQueueReversed(!this.isReversed);
	}
	public NoLeftoversClick() {
		this.api.commands.PlayerQueueNoLeftovers(!this.isNoLeftovers);
	}

	public ShuffleClick() {
		this.api.commands.PlayerQueueShuffle();
	}
	public ClearClick() {
		this.api.commands.PlayerQueueClear();
	}
}
