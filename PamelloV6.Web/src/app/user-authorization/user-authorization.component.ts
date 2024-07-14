import { Component } from '@angular/core';
import { PamelloV6API } from '../../services/pamelloV6API.service';

@Component({
	selector: 'app-user-authorization',
	standalone: true,
	imports: [],
	templateUrl: './user-authorization.component.html',
	styleUrl: './user-authorization.component.scss'
})
export class UserAuthorizationComponent {
	api: PamelloV6API;

	constructor(api: PamelloV6API) {
		this.api = api;
	}
	
	Unauthorize() {
		this.api.UnauthorizeUser();
	}
	Authorize() {
		let authorizationInputElement = document.getElementById("authorization-input") as HTMLInputElement;
		if (authorizationInputElement == null) return;
		
		let code = parseInt(authorizationInputElement.value);
		if (code < 100000 || code > 999999) return;

		this.api.AuthorizeUserWithCode(code);
	}
}
