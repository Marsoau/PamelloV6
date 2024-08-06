import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IPamelloSong } from '../../services/api/model/PamelloSong';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-mini-song',
	standalone: true,
	imports: [],
	templateUrl: './mini-song.component.html',
	styleUrl: './mini-song.component.scss'
})
export class MiniSongComponent {
	@Output() public selected: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();
	@Output() public removeClick: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();
	@Output() public addToPlaylistClick: EventEmitter<IPamelloSong> = new EventEmitter<IPamelloSong>();

	@Input() public song: IPamelloSong | null = null;
	@Input() public displayAddToPlaylistButton: boolean = false;
	@Input() public displayRemoveButton: boolean = false;

	constructor(public readonly api: PamelloV6API) {}

	public AddToQueue() {
		if (!this.song) return;
		this.api.commands.PlayerQueueAddSong(this.song.id);
	}
	public AddToPlaylist() {
		if (!this.song) return;
		this.addToPlaylistClick.emit(this.song);
	}
	public Remove() {
		if (!this.song) return;
		this.removeClick.emit(this.song);
	}
	public Select() {
		if (!this.song) return;
		this.selected.emit(this.song);
	}
}
