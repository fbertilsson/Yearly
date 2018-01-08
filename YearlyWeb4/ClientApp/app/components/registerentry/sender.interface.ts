import { Observable } from 'rxjs/Observable';
import { RegisterEntry } from './registerentry';

export interface ISenderInterface {
    send(
        registerEntry: RegisterEntry
    ): Observable<Object>;
}
