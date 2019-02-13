import {Event } from './event';
import { Account } from './account';
export class  Favorite {
    Event: Event = new Event();
    Account: Account = new Account();
    ReminderUUID: string;
    CreatedBy: string;
    AccountUUID: string;
}
