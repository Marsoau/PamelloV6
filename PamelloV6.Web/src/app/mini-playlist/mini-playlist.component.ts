import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IPamelloPlaylist } from '../../services/api/model/PamelloPlaylist';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-mini-playlist',
	standalone: true,
	imports: [],
	templateUrl: './mini-playlist.component.html',
	styleUrl: './mini-playlist.component.scss'
})
export class MiniPlaylistComponent {
	@Output() public selected: EventEmitter<IPamelloPlaylist> = new EventEmitter<IPamelloPlaylist>();
	@Output() public removeClick: EventEmitter<IPamelloPlaylist> = new EventEmitter<IPamelloPlaylist>();

	@Input() public playlist: IPamelloPlaylist | null = null;
	@Input() public displayRemoveButton: boolean = false;

	constructor(public readonly api: PamelloV6API) {}

	public Select() {
		if (!this.playlist) return;
		this.selected.emit(this.playlist);
	}
	public AddToQueue() {
		if (!this.playlist) return;
		this.api.commands.PlayerQueueAddPlaylist(this.playlist.id);
	}
	public Remove() {
		console.log(this.playlist)
		if (!this.playlist) return;

		this.removeClick.emit(this.playlist);
	}
}
