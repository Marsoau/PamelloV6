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

	public initializationProgress: number;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.sliderDrag = false;
		this.sliderDragValue = 0;
		this.sliderCurrentValue = "0";

		this.initializationProgress = 0;

		this.api.events.PlayerInitializationProgress = (progress: number) => {
			this.initializationProgress = progress;
		}
	}

	public BackClick() {
		this.api.commands.PlayerPrev();
	}

	public PPClick() {
		if (!this.api.selectedPlayer) return;

		if (this.api.selectedPlayer.isPaused) {
			this.api.commands.PlayerResume();
		}
		else {
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
