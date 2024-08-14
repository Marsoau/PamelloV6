import { Component } from '@angular/core';
import { PamelloV6API } from '../../services/api/pamelloV6API.service';
import { FormsModule } from '@angular/forms';

@Component({
	selector: 'app-user-authorization',
	standalone: true,
	imports: [FormsModule],
	templateUrl: './user-authorization.component.html',
	styleUrl: './user-authorization.component.scss'
})
export class UserAuthorizationComponent {
	api: PamelloV6API;

	inputValue: string;
	errorMessage: string;

	constructor(api: PamelloV6API) {
		this.api = api;

		this.inputValue = "";
		this.errorMessage = "";
	}
	
	Unauthorize() {
		this.api.events.Unauthorize();
	}
	async Authorize() {
		this.errorMessage = "";

		let code = parseInt(this.inputValue);
		if (!code || code < 100000 || code > 999999) {
			this.errorMessage = "Code must be a 6 digit number";
			return;
		}

		let acquireTokenResult = await this.api.events.TryAuthorizeWithCode(code, (errorMessage: string) => {
			this.errorMessage = errorMessage;
		});
	}
}
