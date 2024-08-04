import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PlayerSelectionComponent } from "./player-selection/player-selection.component";
import { PlayerControlsComponent } from "./player-controls/player-controls.component";
import { MultipageComponent } from "./multipage/multipage.component";
import { UserAuthorizationComponent } from "./user-authorization/user-authorization.component";
import { PlayerQueueComponent } from "./player-queue/player-queue.component";
import { ReqireAuthorizationComponent } from "./reqire-authorization/reqire-authorization.component";
import { SongAdditionComponent } from "./song-addition/song-addition.component";
import { MiniSongComponent } from "./mini-song/mini-song.component";
import { PageComponent } from "./page/page.component";
import { PamelloV6API } from '../services/api/pamelloV6API.service';
import { PlayerEpisodesComponent } from "./player-episodes/player-episodes.component";
import { PlayerSpeakersControlsComponent } from "./player-speakers-controls/player-speakers-controls.component";

@Component({
	selector: 'app-root',
	standalone: true,
	imports: [RouterOutlet, PlayerSelectionComponent, PlayerControlsComponent, MultipageComponent, UserAuthorizationComponent, PlayerQueueComponent, ReqireAuthorizationComponent, SongAdditionComponent, MiniSongComponent, PageComponent, PlayerEpisodesComponent, PlayerSpeakersControlsComponent],
	templateUrl: './app.component.html',
	styleUrl: './app.component.scss'
})
export class AppComponent {
	constructor(
		public readonly api: PamelloV6API
	) {
		
	}
}
