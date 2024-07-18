import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PamelloPlaylist, PamelloV6API } from '../../services/pamelloV6API.service';

@Component({
	selector: 'app-mini-playlist',
	standalone: true,
	imports: [],
	templateUrl: './mini-playlist.component.html',
	styleUrl: './mini-playlist.component.scss'
})
export class MiniPlaylistComponent {
	@Output() public selected: EventEmitter<PamelloPlaylist> = new EventEmitter<PamelloPlaylist>();
	@Output() public removeClick: EventEmitter<PamelloPlaylist> = new EventEmitter<PamelloPlaylist>();

	@Input() public playlist: PamelloPlaylist | null = null;
	@Input() public displayRemoveButton: boolean = false;

	constructor(public readonly api: PamelloV6API) {}

	public Select() {
		if (!this.playlist) return;
		this.selected.emit(this.playlist);
	}
	public Add() {
		if (!this.playlist) return;
		//this.api.commands.PlayerQueueAddPlaylist(this.playlist.id);
	}
	public Remove() {
		if (!this.playlist) return;
		this.removeClick.emit(this.playlist);
	}
}
