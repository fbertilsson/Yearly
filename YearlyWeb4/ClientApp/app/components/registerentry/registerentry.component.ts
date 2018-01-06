import { Component } from '@angular/core';
import { RegisterEntry } from './registerentry';

@Component({
    selector: 'reg-entry',
    templateUrl: './registerentry.component.html',
    styleUrls: ['./registerentry.component.css']
})
export class RegisterEntryComponent {
    registerEntry: RegisterEntry = {
        dateString: '2018-01-05 17:00',
        registerValue: ''
    }

    constructor() {}

    add(registerEntry: RegisterEntry) {
        console.log('Add ' + registerEntry.dateString + ' ' + registerEntry.registerValue);
    }
}