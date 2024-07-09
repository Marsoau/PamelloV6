import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PamelloV6API } from '../pamelloV6API';

@Component({
	selector: 'app-root',
	standalone: true,
	imports: [RouterOutlet],
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
