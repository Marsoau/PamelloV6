import { Component, Input } from '@angular/core';
import { PamelloSong, PamelloV6API } from '../../services/pamelloV6API.service';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-queue-song',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './queue-song.component.html',
	styleUrl: './queue-song.component.scss'
})
export class QueueSongComponent {
	@Input() public song: PamelloSong | null = null;
	@Input() public position!: number;

	@Input() public isNext: boolean = false;
	@Input() public isCurrent: boolean = false;

	constructor(
		public readonly api: PamelloV6API
	) {

	}

	public SongClick() {
		this.api.commands.PlayerGoToSong(this.position, false);
	}
	public NextClick() {
		this.api.commands.PlayerQueueRequestNext(this.position);
	}
	public RemoveClick() {
		this.api.commands.PlayerQueueRemoveSong(this.position);
	}
}