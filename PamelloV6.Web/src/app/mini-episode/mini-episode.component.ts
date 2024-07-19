import { Component, Input } from '@angular/core';
import { PamelloEpisode } from '../../services/pamelloV6API.service';
import { StringTimePipe } from '../string-time.pipe';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-mini-episode',
	standalone: true,
	imports: [CommonModule, StringTimePipe],
	templateUrl: './mini-episode.component.html',
	styleUrl: './mini-episode.component.scss'
})
export class MiniEpisodeComponent {
	@Input() public episode: PamelloEpisode | null = null;
	@Input() public displayRemoveButton: boolean = false;
}
