import { Component } from '@angular/core';
import { PlayerSpeakerComponent } from "../player-speaker/player-speaker.component";
import { PamelloV6API } from '../../services/api/pamelloV6API.service';

@Component({
	selector: 'app-player-speakers-controls',
	standalone: true,
	imports: [PlayerSpeakerComponent],
	templateUrl: './player-speakers-controls.component.html',
	styleUrl: './player-speakers-controls.component.scss'
})
export class PlayerSpeakersControlsComponent {
	public constructor(
		public readonly api: PamelloV6API
	) {

	}

	public SpeakerDisconnect(speakerPosition: number) {
		this.api.commands.PlayerDisconnect(speakerPosition);
	}
}
