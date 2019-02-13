import { Node } from './node';

export class  Event extends Node {

    Body: string;

    Category: string;

    EventDateTime: Date;

        // how many times to remind
    RepeatCount: number;

        // if this is set ignore RepeatCount
    RepeatForever: boolean;

        // daily, bi-weekly, monthly..
    Frequency: string;

    StartDate: Date;

    StartTime: string;

    EndDate: Date;

    EndTime: string;

    EventLocationUUID: string;

    // who's hosting the event
    HostAccountUUID: string;

    FavoritedByUserUUID: string;

    FavoritedByAccountUUID: string;

}
