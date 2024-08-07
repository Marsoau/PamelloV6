import { Component } from '@angular/core';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloPlayer } from '../../services/api/model/PamelloPlayer';
import { SearchResult } from '../../services/api/pamelloV6DataAPI';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-player-selection',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './player-selection.component.html',
	styleUrl: './player-selection.component.scss'
})
export class PlayerSelectionComponent {
	api: PamelloV6API;

	inputValue: string;
	selectionMode: boolean;
	availablePlayers: IPamelloPlayer[];

	displaySelect: boolean;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.inputValue = "";
		this.selectionMode = true;
		this.availablePlayers = [];

		this.displaySelect = false;

		this.api.events.PlayerCreated = async (newPlayerId: number) => {
			this.LoadAwailablePlayers();
		};
		this.api.events.PlayerDeleted = async (newPlayerId: number) => {
			this.LoadAwailablePlayers();
		};
	}

	ngAfterViewInit() {
		this.LoadAwailablePlayers();
	}

	public async LoadAwailablePlayers() {
		let searchResult = await this.api.data.SearchPlayers(0, 100);
		if (!searchResult) return;

		this.availablePlayers = searchResult.results;
	}

	public async SwitchMode() {
		this.selectionMode = !this.selectionMode;
		this.displaySelect = false;
	}

	public async PlayerCreate() {
		if (this.inputValue.length == 0) return;

		let newPlayerId = await this.api.commands.PlayerCreate(this.inputValue) as number | null;
		console.log(`new player id ${newPlayerId} test`);
		this.selectionMode = true;
		this.api.commands.PlayerSelect(newPlayerId);
	}
	public async PlayerDeleteSelected() {
		if (!this.api.selectedPlayer) return;

		this.api.commands.PlayerDeleteSelected();
	}

	public async SelectPlayer(playerId: number | null) {
		this.displaySelect = false;

		if (this.api.selectedPlayer?.id == playerId) {
			await this.api.commands.PlayerSelect(null);
		}
		else {
			await this.api.commands.PlayerSelect(playerId);
		}
	}
}
