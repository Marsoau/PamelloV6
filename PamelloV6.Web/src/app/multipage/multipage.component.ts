import { CommonModule } from '@angular/common';
import { Component, ContentChildren, Input, QueryList } from '@angular/core';
import { MiniSongComponent } from '../mini-song/mini-song.component';
import { PageComponent } from '../page/page.component';

@Component({
	selector: 'app-multipage',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './multipage.component.html',
	styleUrl: './multipage.component.scss'
})
export class MultipageComponent {
	@ContentChildren(PageComponent) pages!: QueryList<PageComponent>;

	public selectedPage: number | null = 1;
}
