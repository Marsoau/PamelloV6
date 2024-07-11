import { Component } from '@angular/core';
import { PamelloPlayer, PamelloSong, PamelloV6API } from '../../pamelloV6API';

@Component({
	selector: 'app-player-controls',
	standalone: true,
	imports: [],
	templateUrl: './player-controls.component.html',
	styleUrl: './player-controls.component.scss'
})
export class PlayerControlsComponent {
	private readonly api: PamelloV6API;
	public selectedPlayer: PamelloPlayer | null;
	public currentSong: PamelloSong | null;

	public sliderDrag: boolean;
	public sliderDragValue: number;

	constructor(api: PamelloV6API) {
		this.api = api;
		this.selectedPlayer = null;
		this.currentSong = null;

		this.sliderDrag = false;
		this.sliderDragValue = 0;

		this.SubscribeToEvents();
		this.UpdateSelectedPlayer();
	}

	private SubscribeToEvents() {
		this.api.events.UserPlayerSelected = (newPlayerId: number) => {
			this.UpdateSelectedPlayer();
		}
		
		this.api.events.PlayerCurrentTimeUpdated = (newSeconds: number) => {
			this.selectedPlayer!.currentSongTimePassed = newSeconds;
		}
		this.api.events.PlayerTotalTimeUpdated = (newSeconds: number) => {
			this.selectedPlayer!.currentSongTimeTotal = newSeconds;
		}
		this.api.events.PlayerPauseUpdated = (newState: boolean) => {
			this.selectedPlayer!.isPaused = newState;
		}
		this.api.events.PlauerQueueSongUpdated = (newSongId: number) => {
			this.selectedPlayer!.currentSongId = newSongId;
			this.UpdateCurrentSong();
		}
	}

	private UpdateSelectedPlayer() {
		if (this.api.authorizedUser!.selectedPlayerId != null) {
			this.api.data.GetPlayer(this.api.authorizedUser!.selectedPlayerId).then(player => {
				this.selectedPlayer = player;
				this.UpdateCurrentSong();
			})
		}
		else {
			this.selectedPlayer = null;
			this.UpdateCurrentSong();
		}
	}

	private UpdateCurrentSong() {
		if (this.selectedPlayer != null && this.selectedPlayer.currentSongId != null) {
			console.log("f");
			this.api.data.GetSong(this.selectedPlayer!.currentSongId).then(song => {
				this.currentSong = song;
			})
		}
		else {
			console.log("s");
			this.currentSong = null;
		}
	}

	public BackClick() {
		this.api.commands.PlayerPrev();
	}

	public PPClick() {
		if (this.selectedPlayer?.isPaused) {
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

	ToStringTime(seconds: number) {
		let h = Math.floor(seconds / 3600);
		let m = Math.floor((seconds - h * 3600) / 60);
		let s = seconds % 60;
		return `${(h) ? (h + ":") : ("")}${(m > 9) ? ("") : ("0")}${(m) ? (m) : ("0")}:${(s > 9) ? ("") : ("0")}${(s) ? (s) : ("0")}`;
	}
}
