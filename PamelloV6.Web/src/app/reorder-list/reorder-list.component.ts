import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Output } from '@angular/core';

@Component({
	selector: 'app-reorder-list',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './reorder-list.component.html',
	styleUrl: './reorder-list.component.scss'
})
export class ReorderListComponent {
	@Output() public added = new EventEmitter<string>();

	public additionMode: boolean = false;

	AdditionAreaDragEnter(event: DragEvent) {
		this.additionMode = true;

		event.preventDefault();
	}
	AdditionAreaDragLeave(event: DragEvent) {
		this.additionMode = false;
		console.log("dragleave list");
	}
	AdditionAreaDragDrop(event: DragEvent) {
		this.additionMode = false;
		console.log("dragdrop list");
	}

	DragOverAdd(event: DragEvent) {
		event.preventDefault();
	}
	DragLeaveAdd(event: DragEvent) {
		
	}
	DragDropAdd(event: DragEvent) {
		if (!event.dataTransfer) return;

		let key = event.dataTransfer.getData("reorder-key");
		if (!key) return;

		console.log(`"${key}" added`);

		this.added.emit(key);
	}
}
