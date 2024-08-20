import { Component, ViewChild } from '@angular/core';
import { InspectorComponent } from "../inspector/inspector.component";
import { SearchComponent } from "../search/search.component";

@Component({
	selector: 'app-song-addition',
	standalone: true,
	imports: [InspectorComponent, SearchComponent],
	templateUrl: './song-addition.component.html',
	styleUrl: './song-addition.component.scss'
})
export class SongAdditionComponent {
	@ViewChild('inspector') inspector: InspectorComponent | null = null;
}
