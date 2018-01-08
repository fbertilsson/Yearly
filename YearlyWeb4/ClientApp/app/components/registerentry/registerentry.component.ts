import { Component } from '@angular/core';
import { RegisterEntry } from './registerentry';
import { SenderService } from './sender.service';

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

    constructor(
        private sender: SenderService) {}

    add(registerEntry: RegisterEntry) {
        console.log('Add ' + registerEntry.dateString + ' ' + registerEntry.registerValue);
        this.sender.send(
                registerEntry)
            .subscribe(x => console.log(x));
    }
}