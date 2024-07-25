import { Component, Input } from '@angular/core';
import { PamelloEpisode, PamelloV6API } from '../../services/pamelloV6API.service';
import { StringTimePipe } from '../string-time.pipe';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-mini-episode',
	standalone: true,
	imports: [CommonModule, StringTimePipe, FormsModule],
	templateUrl: './mini-episode.component.html',
	styleUrl: './mini-episode.component.scss'
})
export class MiniEpisodeComponent {
	@Input() public episode: PamelloEpisode | null = null;
	@Input() public displayRemoveButton: boolean = false;

	public nameInputValue: string = "";
	public startInputValue: string = "";

	public isNameEdit: boolean = false;
	public isStartEdit: boolean = false;

	public constructor(
		private readonly api: PamelloV6API
	) {
		this.api.events.EpisodeNameUpdated = (data: any) => {
			if (this.episode && data.episodeId == this.episode?.id) {
				this.episode.name = data.newName;
			}
		}
		this.api.events.EpisodeStartUpdated = (data: any) => {
			if (this.episode && data.episodeId == this.episode?.id) {
				this.episode.start = data.newSeconds;
			}
		}
		this.api.events.EpisodeSkipStateUpdated = (data: any) => {
			if (this.episode && data.episodeId == this.episode?.id) {
				this.episode.skip = data.newState;
			}
		}
	}

	public NameEdit() {
		if (this.isNameEdit) this.SaveName();

		this.nameInputValue = this.episode?.name ?? "";

		this.isNameEdit = !this.isNameEdit;
		this.isStartEdit = false;
	}
	public StartEdit() {
		if (this.isStartEdit) this.SaveStart();

		this.startInputValue = String(this.episode?.start ?? '0');

		this.isNameEdit = false;
		this.isStartEdit = !this.isStartEdit;
	}

	public SaveName() {
		if (!this.episode) return;
		if (!this.nameInputValue) return;

		this.api.commands.EpisodeRename(this.episode.id, this.nameInputValue);
	}
	public SaveStart() {
		if (!this.episode) return;

		let startSeconds = parseInt(this.startInputValue ?? '0');

		this.api.commands.EpisodeChangeStart(this.episode.id, startSeconds);
	}
	public SwitchSkip() {
		if (!this.episode) return;

		this.api.commands.EpisodeChangeSkipState(this.episode.id, !this.episode.skip);
	}
}
