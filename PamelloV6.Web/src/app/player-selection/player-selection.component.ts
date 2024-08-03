import { Component } from '@angular/core';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { IPamelloPlayer } from '../../services/api/model/PamelloPlayer';
import { SearchResult } from '../../services/api/pamelloV6DataAPI';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-player-selection',
	standalone: true,
	imports: [FormsModule],
	templateUrl: './player-selection.component.html',
	styleUrl: './player-selection.component.scss'
})
export class PlayerSelectionComponent {
	api: PamelloV6API;

	inputValue: string;
	selectValue: string;
	selectionMode: boolean;
	availablePlayers: IPamelloPlayer[];

	constructor(api: PamelloV6API) {
		this.api = api;

		this.inputValue = "";
		this.selectValue = "";
		this.selectionMode = true;
		this.availablePlayers = [];

		this.api.events.PlayerCreated = async (newPlayerId: number) => {
			let player = await this.api.data.GetPlayer(newPlayerId);
			if (player == null) return;
	
			this.availablePlayers.push(player);
		};
	}

	ngAfterViewInit() {
		this.LoadAwailablePlayers();
	}

	public GetSelectValue() {
		if (this.selectValue != (this.api.selectedPlayer?.id ?? "none").toString()) {
			this.selectValue = (this.api.selectedPlayer?.id ?? "none").toString();
		}
		return this.selectValue;
	}

	public async LoadAwailablePlayers() {
		let searchResult = await this.api.data.SearchPlayers(0, 100);
		if (!searchResult) return;

		this.availablePlayers = searchResult.results;
		this.UpdatePlayerSelectOption();
	}

	public async PlayerCreateClick() {
		console.log(this.inputValue);
		return;
		if (this.inputValue.length == 0) return;

		let newPlayerId = await this.api.commands.PlayerCreate(this.inputValue) as number | null;
		console.log(`new player id ${newPlayerId} test`);
		this.selectionMode = true;
		this.api.commands.PlayerSelect(newPlayerId);
	}
	public SelectionChanged() {
		let valueNumber;
		if (this.selectValue == "none") {
			valueNumber = null;
		}
		else {
			valueNumber = parseInt(this.selectValue);
		}

		this.UpdatePlayerSelectOption();
	
		this.api.commands.PlayerSelect(valueNumber);
	}
	public UpdatePlayerSelectOption() {
		if (!this.api.selectedPlayer) {
			this.selectValue = "none";
		}
		else {
			this.selectValue = this.api.selectedPlayer.id.toString();
		}
	}
}
