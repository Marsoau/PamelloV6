import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
	name: 'stringTime',
	standalone: true
})
export class StringTimePipe implements PipeTransform {
	transform(seconds: number): unknown {
		let h = Math.floor(seconds / 3600);
		let m = Math.floor((seconds - h * 3600) / 60);
		let s = seconds % 60;
		return `${(h) ? (h + ":") : ("")}${(m > 9) ? ("") : ("0")}${(m) ? (m) : ("0")}:${(s > 9) ? ("") : ("0")}${(s) ? (s) : ("0")}`;
	}
}
