import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PamelloV6API } from '../services/pamelloV6API.service';
import { PlayerSelectionComponent } from "./player-selection/player-selection.component";
import { PlayerControlsComponent } from "./player-controls/player-controls.component";
import { MultipageComponent } from "./multipage/multipage.component";
import { UserAuthorizationComponent } from "./user-authorization/user-authorization.component";
import { PlayerQueueComponent } from "./player-queue/player-queue.component";
import { ReqireAuthorizationComponent } from "./reqire-authorization/reqire-authorization.component";
import { SongAdditionComponent } from "./song-addition/song-addition.component";

@Component({
	selector: 'app-root',
	standalone: true,
	imports: [RouterOutlet, PlayerSelectionComponent, PlayerControlsComponent, MultipageComponent, UserAuthorizationComponent, PlayerQueueComponent, ReqireAuthorizationComponent, SongAdditionComponent],
	templateUrl: './app.component.html',
	styleUrl: './app.component.scss'
})
export class AppComponent {
	title = 'PamelloV6.Web';
	api: PamelloV6API;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.api.events.PlayerCreated = async (playerId: number) => {
			console.log(`created player: ${(await this.api.data.GetPlayer(playerId)).name}`);
		}
	}

	public async ButtonClick() {
		let p = document.getElementById("p-test");
		if (p == null) return;

		p.innerHTML = this.api.authorizedUser!.name;
	}
}
