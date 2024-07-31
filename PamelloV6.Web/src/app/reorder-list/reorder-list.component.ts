import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
	selector: 'app-reorder-list',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './reorder-list.component.html',
	styleUrl: './reorder-list.component.scss'
})
export class ReorderListComponent {
	@Output() public added = new EventEmitter<string>();
	public displayAdd: boolean = false;

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
