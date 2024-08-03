import { Component } from '@angular/core';
import { StringTimePipe } from '../string-time.pipe';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloPlayer, PamelloPlayerState } from '../../services/api/model/PamelloPlayer';
import { IPamelloSong } from '../../services/api/model/PamelloSong';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-player-controls',
	standalone: true,
	imports: [StringTimePipe, FormsModule],
	templateUrl: './player-controls.component.html',
	styleUrl: './player-controls.component.scss'
})
export class PlayerControlsComponent {
	public readonly api: PamelloV6API;

	public sliderDrag: boolean;
	public sliderDragValue: number;
	public sliderCurrentValue: string;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.sliderDrag = false;
		this.sliderDragValue = 0;
		this.sliderCurrentValue = "0";
	}

	public BackClick() {
		this.api.commands.PlayerPrev();
	}

	public PPClick() {
		console.log("pp:", this.api.selectedPlayer?.state);
		if (this.api.selectedPlayer?.state == PamelloPlayerState.Paused) {
			this.api.commands.PlayerResume();
		}
		else if (this.api.selectedPlayer?.state == PamelloPlayerState.Playing) {
			this.api.commands.PlayerPause();
		}
	}

	public ForwardClick() {
		this.api.commands.PlayerNext();
	}

	public SliderChanged() {
		if (!this.sliderDrag) return;

		this.sliderDrag = false;
		
		this.api.commands.PlayerRewind(this.sliderDragValue);
	}

	public Test() {
		if (!this.sliderDrag) return;
		let slider = document.getElementById("player-time-slider") as HTMLInputElement;

		this.sliderDragValue = parseInt(slider.value);
	}
}
