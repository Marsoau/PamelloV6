import { Component } from '@angular/core';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { MiniEpisodeComponent } from "../mini-episode/mini-episode.component";

@Component({
	selector: 'app-player-episodes',
	standalone: true,
	imports: [MiniEpisodeComponent],
	templateUrl: './player-episodes.component.html',
	styleUrl: './player-episodes.component.scss'
})
export class PlayerEpisodesComponent {
	public constructor(
		public readonly api: PamelloV6API
	) {

	}

	public EpisodeSelected(episodeIndex: number) {
		this.api.commands.PlayerRewindToEpisode(episodeIndex);
	}
}
