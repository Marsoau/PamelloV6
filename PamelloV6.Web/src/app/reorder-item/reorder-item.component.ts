import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

@Component({
	selector: 'app-reorder-item',
	standalone: true,
	imports: [CommonModule],
	templateUrl: './reorder-item.component.html',
	styleUrl: './reorder-item.component.scss'
})
export class ReorderItemComponent {
	@Input() public isDragable: boolean = false;
	@Input() public isDropableOn: boolean = false;
	@Input() public doDisplayDropZoneLines: boolean = false;

	@Input() public itemSourceName: string = "default";
	@Input() public itemSourceId: number = 0;
	@Input() public itemName: string = "default";
	@Input() public itemIndex: number = 0;
	@Input() public itemId: number = 0;

	@Input() public dropZoneDistance: number = 2;

	public inAfterZone: boolean = false;
	public displayTopDropLine: boolean = false;
	public displayBottomDropLine: boolean = false;

	@Output() public reorder = new EventEmitter<ReorderEvent>();

	@HostListener("dragover", ['$event']) HostDragEnter(event: DragEvent) {
		if (!this.isDropableOn) return;

		let targetElement = event.currentTarget as HTMLElement;
		if (!targetElement) return;

		this.inAfterZone = event.offsetY > (targetElement.offsetHeight / 3);

		this.displayTopDropLine = !this.inAfterZone;
		this.displayBottomDropLine = this.inAfterZone;

		console.log(targetElement.scrollTop);

		event.preventDefault();
	}
	@HostListener("dragleave") HostDragLeave(event: DragEvent) {
		this.displayTopDropLine = false;
		this.displayBottomDropLine = false;
	}
	@HostListener("drop", ['$event']) HostDragDrop(event: DragEvent) {
		this.displayTopDropLine = false;
		this.displayBottomDropLine = false;

		if (!event.dataTransfer) return;

		let senderSourceName = event.dataTransfer.getData("reorder-item-source-name");
		let senderSourceIdValue = event.dataTransfer.getData("reorder-item-source-id");
		let senderName = event.dataTransfer.getData("reorder-item-name");
		let senderIndexValue = event.dataTransfer.getData("reorder-item-index");
		let senderIdValue = event.dataTransfer.getData("reorder-item-id");
		
		let senderSourceId = parseInt(senderSourceIdValue);
		let senderIndex = parseInt(senderIndexValue);
		let senderId = parseInt(senderIdValue);

		if (Number.isNaN(senderIndex) || Number.isNaN(senderId) || Number.isNaN(senderSourceId)) {
			console.log("wrong transfer data");
			return;
		}

		console.log(`move {${senderSourceId}:${senderSourceName}}${senderId}:${senderName} (${senderIndex}) to {${this.itemSourceId}:${this.itemSourceName}}${this.itemId}:${this.itemName} (${this.itemIndex}) in the ${(this.inAfterZone ? "Bottom" : "Top")}`);

		this.reorder.emit(new ReorderEvent(
			senderSourceName,
			senderSourceId,
			senderName,
			senderIndex,
			senderId,
			this.itemIndex + (this.doDisplayDropZoneLines && this.inAfterZone ? 1 : 0),
			this.itemId,

			this.doDisplayDropZoneLines && this.inAfterZone
		));
	}

	public DragStart(event: DragEvent) {
		if (!event.dataTransfer) return;

		event.dataTransfer.effectAllowed = "all";
		event.dataTransfer.setData("reorder-item-source-name", `${this.itemSourceName}`);
		event.dataTransfer.setData("reorder-item-source-id", `${this.itemSourceId}`);
		event.dataTransfer.setData("reorder-item-name", `${this.itemName}`);
		event.dataTransfer.setData("reorder-item-index", `${this.itemIndex}`);
		event.dataTransfer.setData("reorder-item-id", `${this.itemId}`);
	}
}

export class ReorderEvent {
	public senderSourceName: string;
	public senderSourceId: number;
	public senderName: string;
	public senderIndex: number;
	public senderId: number;
	public targetIndex: number;
	public targetId: number;

	public droppedAfter: boolean;

	public constructor(
		senderSourceName: string,
		senderSourceId: number,
		senderName: string,
		senderIndex: number,
		senderId: number,
		targetIndex: number,
		targetId: number,

		droppedAfter: boolean
	) {
		this.senderSourceName = senderSourceName;
		this.senderSourceId = senderSourceId;
		this.senderName = senderName;
		this.senderIndex = senderIndex;
		this.senderId = senderId;
		this.targetIndex = targetIndex;
		this.targetId = targetId;

		this.droppedAfter = droppedAfter;
	}
}
