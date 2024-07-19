import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, contentChild, contentChildren, ContentChildren, ElementRef, Input, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { MiniSongComponent } from '../mini-song/mini-song.component';
import { PageComponent } from '../page/page.component';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
	selector: 'app-multipage',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './multipage.component.html',
	styleUrl: './multipage.component.scss'
})
export class MultipageComponent {
	@Input() pages!: string[];

	@ContentChildren("page") children!: QueryList<any>;
	@ViewChild("test") testChild!: ElementRef<any>;

	public selectedPage: number | null = null;

	public constructor(
		private _sanitizer: DomSanitizer
	) {

	}

	safeHtml(html: string): SafeHtml {
		return this._sanitizer.bypassSecurityTrustHtml(html);
	}
}
