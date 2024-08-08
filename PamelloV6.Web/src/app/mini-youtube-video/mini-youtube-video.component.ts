import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IYoutubeSearchVideoInfo } from '../../services/api/model/search/YoutubeSearchVideoInfo';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-mini-youtube-video',
	standalone: true,
	imports: [],
	templateUrl: './mini-youtube-video.component.html',
	styleUrl: './mini-youtube-video.component.scss'
})
export class MiniYoutubeVideoComponent {
	@Input() public video: IYoutubeSearchVideoInfo | null = null;

	@Input() public displayAddToPlaylistButton: boolean = true;

	@Output() public addToPlaylistClick: EventEmitter<string> = new EventEmitter<string>();

	public constructor(
		public readonly api: PamelloV6API
	) {

	}

	AddToQueue() {
		if (!this.video?.id) return;

		this.api.commands.PlayerQueueAddYoutubeSong(this.video.id);
	}

	AddToPlaylist() {
		if (!this.video?.id) return;

		this.addToPlaylistClick.emit(this.video.id);
	}
}
