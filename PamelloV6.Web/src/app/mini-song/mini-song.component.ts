import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PamelloSong, PamelloV6API } from '../../services/pamelloV6API.service';

@Component({
	selector: 'app-mini-song',
	standalone: true,
	providers: [PamelloV6API],
	imports: [],
	templateUrl: './mini-song.component.html',
	styleUrl: './mini-song.component.scss'
})
export class MiniSongComponent {
	@Output() public selected: EventEmitter<PamelloSong> = new EventEmitter<PamelloSong>();
	@Output() public removeClick: EventEmitter<PamelloSong> = new EventEmitter<PamelloSong>();

	@Input() public song: PamelloSong | null = null;
	@Input() public displayRemoveButton: boolean = false;

	constructor(public readonly api: PamelloV6API) {}

	public Add() {
		if (!this.song) return;
		this.api.commands.PlayerQueueAddSong(this.song?.id);
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
