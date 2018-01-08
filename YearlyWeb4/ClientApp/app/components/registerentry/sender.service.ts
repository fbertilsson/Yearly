import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';
import { ISenderInterface } from './sender.interface';
import { RegisterEntry } from './registerentry';

@Injectable()
export class SenderService implements ISenderInterface {

    uri = 'api/RegisterEntry/SubmitRegisterEntry';

    constructor(private http: HttpClient) {
        console.log('SenderService ctor');
    }

    send(
        registerEntry: RegisterEntry): Observable<Object> {

        const s = JSON.stringify(registerEntry);
        const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
        return this.http.post(this.uri, s, { headers: headers }).catch(this.handleError);
    }

    private handleError(error: HttpErrorResponse) {
        // In a real world app, you might use a remote logging infrastructure
        let errMsg: string;
        if (error.error instanceof Error) {
            errMsg = 'A client-side or network error occurred: ${error.error.message}';
        } else {
            errMsg = `Backend returned code ${error.status}, body was: ${error.error}`;
        }
        errMsg = 'SenderService: Error sending entry. Function turned off?: ' + errMsg;
        console.error(errMsg);
        return Observable.throw(errMsg);
    }
}
