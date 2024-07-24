import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
	selector: 'app-reorder-item',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './reorder-item.component.html',
	styleUrl: './reorder-item.component.scss'
})
export class ReorderItemComponent {
	@Input() public key: string = "default";
	@Input() public dropZoneHeight: number = 4;

	public isOverDropZone: boolean = false;

	@Output() public moved = new EventEmitter<ReorderEvent>();
	@Output() public swapped = new EventEmitter<ReorderEvent>();

	public DragStart(event: DragEvent) {
		if (!event.dataTransfer) return;

		event.dataTransfer.effectAllowed = "all";
		event.dataTransfer.setData("header", "ReorderItem");
		event.dataTransfer.setData("reorder-key", `${this.key}`);
	}
	public DragOver(event: DragEvent) {
		event.preventDefault();
	}
	public DragDrop(event: DragEvent) {
		if (!event.dataTransfer) return;

		let firstKey = event.dataTransfer.getData("reorder-key");
		if (!firstKey) return;

		console.log(`"${firstKey}" swapped with "${this.key}"`);

		this.swapped.emit(new ReorderEvent(firstKey, this.key));
	}

	public DragZoneOver(event: DragEvent) {
		this.isOverDropZone = true;
		event.preventDefault();
	}
	public DragZoneLeave(event: DragEvent) {
		this.isOverDropZone = false;
	}
	public DragZoneDrop(event: DragEvent) {
		this.isOverDropZone = false;

		if (!event.dataTransfer) return;

		let firstKey = event.dataTransfer.getData("reorder-key");
		if (!firstKey) return;

		console.log(`"${firstKey}" moved to "${this.key}"`);

		this.moved.emit(new ReorderEvent(firstKey, this.key));
	}
}

export class ReorderEvent {
	public firstKey: string;
	public secondKey: string;

	public constructor(
		firstKey: string,
		secondKey: string
	) {
		this.firstKey = firstKey;
		this.secondKey = secondKey;
	}
}
