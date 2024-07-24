import { Component } from '@angular/core';
import { PamelloSong, PamelloV6API } from '../../services/pamelloV6API.service';
import { CommonModule } from '@angular/common';
import { ReorderListComponent } from "../reorder-list/reorder-list.component";
import { ReorderEvent, ReorderItemComponent } from "../reorder-item/reorder-item.component";
import { QueueSongComponent } from "../queue-song/queue-song.component";

@Component({
	selector: 'app-player-queue',
	standalone: true,
	imports: [CommonModule, ReorderListComponent, ReorderItemComponent, QueueSongComponent],
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

	public dragPosition: number | null;

	public zoneOver: number | null;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.songs = [];
		this.position = 0;
		this.nextPosition = null;

		this.isRandom = false;
		this.isReversed = false;
		this.isNoLeftovers = false;

		this.dragPosition = null;

		this.zoneOver = null;

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
		//this.api.commands.PlayerQueueShuffle();
	}
	public ClearClick() {
		this.api.commands.PlayerQueueClear();
	}

	songAdded(event: string) {

	}
	songMoved(event: ReorderEvent) {
		let fromPosition = parseInt(event.firstKey);
		let toPosition = parseInt(event.secondKey);

		this.api.commands.PlayerQueueMove(fromPosition, toPosition);
	}
	songSwapped(event: ReorderEvent) {
		let fromPosition = parseInt(event.firstKey);
		let withPosition = parseInt(event.secondKey);

		this.api.commands.PlayerQueueSwap(fromPosition, withPosition);
	}
}
