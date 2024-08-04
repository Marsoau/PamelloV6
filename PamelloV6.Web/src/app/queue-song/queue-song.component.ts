import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloSong } from '../../services/api/model/PamelloSong';

@Component({
	selector: 'app-queue-song',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './queue-song.component.html',
	styleUrl: './queue-song.component.scss'
})
export class QueueSongComponent {
	@Input() public song: IPamelloSong | null = null;
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
		if (this.isNext) {
			this.api.commands.PlayerQueueRequestNext(null);
		}
		else {
			this.api.commands.PlayerQueueRequestNext(this.position);
		}
	}
	public RemoveClick() {
		this.api.commands.PlayerQueueRemoveSong(this.position);
	}
}
