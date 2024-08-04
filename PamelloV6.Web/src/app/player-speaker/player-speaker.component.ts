import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IPamelloSpeaker } from '../../services/api/model/PamelloSpeaker';

@Component({
	selector: 'app-player-speaker',
	standalone: true,
	imports: [],
	templateUrl: './player-speaker.component.html',
	styleUrl: './player-speaker.component.scss'
})
export class PlayerSpeakerComponent {
	@Output() public disconnectClick: EventEmitter<void> = new EventEmitter<void>();

	@Input() public speaker: IPamelloSpeaker | null = null;

	public Disconnect() {
		console.log(this.speaker);
		this.disconnectClick.emit()
	}
}
