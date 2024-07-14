import { Component } from '@angular/core';
import { PamelloPlayer, PamelloV6API } from '../../services/pamelloV6API.service';

@Component({
	selector: 'app-player-selection',
	standalone: true,
	imports: [],
	templateUrl: './player-selection.component.html',
	styleUrl: './player-selection.component.scss'
})
export class PlayerSelectionComponent {
	api: PamelloV6API;

	selectionMode: boolean;
	availablePlayers: PamelloPlayer[];

	constructor(api: PamelloV6API) {
		this.api = api;

		this.selectionMode = true;
		this.availablePlayers = [];

		this.api.events.PlayerCreated = async (newPlayerId: number) => {
			let player = await this.api.data.GetPlayer(newPlayerId);
			if (player == null) return;
	
			this.availablePlayers.push(player);
		};
		this.api.events.UserPlayerSelected = async (selectedPlayerId: number) => {
			this.UpdatePlayerSelectOption();
		}
	}

	ngAfterViewInit() {
		let selectElement = document.getElementsByClassName("player-select")[0] as HTMLSelectElement;
		console.log(`seitusedyuikgj 1: ${selectElement.options.length}`);

		this.api.data.SearchPlayers(0, 100).then((searchResult) => {
			this.availablePlayers = searchResult.results;
			console.log(`seitusedyuikgj 2: ${selectElement.options.length}`);
			this.UpdatePlayerSelectOption();
		});
	}

	public async PlayerCreateClick() {
		let inputElement = document.getElementById("player-creation-name-input") as HTMLInputElement;
		if (inputElement.value.length == 0) return;

		let newPlayerId = await this.api.commands.PlayerCreate(inputElement.value) as number | null;
		console.log(`new player id ${newPlayerId} test`);
		this.selectionMode = true;
		this.api.commands.PlayerSelect(newPlayerId);
	}
	public SelectionChanged() {
		let selectElement = document.getElementsByClassName("player-select")[0] as HTMLSelectElement;
		let selectedValue = selectElement.value;

		this.UpdatePlayerSelectOption();

		let valueNumber;
		if (selectedValue == "none") {
			valueNumber = null;
		}
		else {
			valueNumber = parseInt(selectedValue);
		}
	
		this.api.commands.PlayerSelect(valueNumber);
	}
	public UpdatePlayerSelectOption() {
		this.api.authorizedUser!.selectedPlayerId = this.api.authorizedUser!.selectedPlayerId;
		let selectElement = document.getElementsByClassName("player-select")[0] as HTMLSelectElement;

		if (this.api.authorizedUser?.selectedPlayerId == null) {
			selectElement.value = "none";
		}
		else {
			selectElement.value = this.api.authorizedUser.selectedPlayerId.toString();
		}
	}
}
