import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReorderListComponent } from "../reorder-list/reorder-list.component";
import { ReorderEvent, ReorderItemComponent } from "../reorder-item/reorder-item.component";
import { QueueSongComponent } from "../queue-song/queue-song.component";
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-player-queue',
	standalone: true,
	imports: [CommonModule, ReorderListComponent, ReorderItemComponent, QueueSongComponent],
	templateUrl: './player-queue.component.html',
	styleUrl: './player-queue.component.scss'
})
export class PlayerQueueComponent {
	public readonly api: PamelloV6API;

	public dragPosition: number | null;
	public zoneOver: number | null;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.dragPosition = null;
		this.zoneOver = null;
	}

	public SongClick(songPosition: number) {
		if (!this.api.selectedPlayer || songPosition == this.api.selectedPlayer.queuePosition) return;

		this.api.commands.PlayerGoToSong(songPosition, false);
	}
	public NextClick(songPosition: number) {
		if (!this.api.selectedPlayer || songPosition == this.api.selectedPlayer.queuePosition) return;

		if (songPosition == this.api.selectedPlayer.queuePosition) {
			this.api.commands.PlayerQueueRequestNext(null);
		}
		else {
			this.api.commands.PlayerQueueRequestNext(songPosition);
		}
	}
	public RemoveClick(songPosition: number) {
		this.api.commands.PlayerQueueRemoveSong(songPosition);
	}

	public RandomClick() {
		if (!this.api.selectedPlayer) return;
		this.api.commands.PlayerQueueRandom(!this.api.selectedPlayer.queueIsRandom);
	}
	public ReversedClick() {
		if (!this.api.selectedPlayer) return;
		this.api.commands.PlayerQueueReversed(!this.api.selectedPlayer.queueIsReversed);
	}
	public NoLeftoversClick() {
		if (!this.api.selectedPlayer) return;
		this.api.commands.PlayerQueueNoLeftovers(!this.api.selectedPlayer.queueIsNoLeftovers);
	}

	public ShuffleClick() {
		//this.api.commands.PlayerQueueShuffle();
	}
	public ClearClick() {
		this.api.commands.PlayerQueueClear();
	}

	QueueSongReorder(event: ReorderEvent) {
		console.log(event);

		if (event.senderName == "playlist") {
			this.api.commands.PlayerQueueInsertPlaylist(event.targetIndex, event.senderId);
			return;
		}
		else if (event.senderName != "song") return;

		console.log("gt");

		if (event.senderSourceName == "queue") {
			this.api.commands.PlayerQueueMove(event.senderIndex, event.targetIndex);
		}
		else {
			this.api.commands.PlayerQueueInsertSong(event.targetIndex, event.senderId);
		}
	}
}
