import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { catchError, lastValueFrom, throwError } from "rxjs";
import { PamelloConfig } from "../../config/config";
import { PamelloV6API } from "./pamelloV6API.service";

export class PamelloV6Http {
    public constructor (
        private readonly _api: PamelloV6API,
        private readonly _http: HttpClient
    ) {

    }

    public async Get<T>(request: string, onerror: any = null) {
        try {
            return await lastValueFrom(this._http.get<T>(`${PamelloConfig.BaseUrl}/${request}`));
        }
        catch (x) {
            let error = x as HttpErrorResponse;
            console.log(error);

            let errorMessage = typeof error.error == "string" ? error.error : "Failed to connect";

            if (onerror) onerror(errorMessage);
            return null;
        }
    }
    
    public async InvokeCommand(commandString: string) {
		return await lastValueFrom(
            this._http.get(`${PamelloConfig.BaseUrl}/Command?name=${commandString}`, {
                headers: new HttpHeaders({
                    "user-token": this._api.token ?? ""
                }) 
            })
        );
    }
}
