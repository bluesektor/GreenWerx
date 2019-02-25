import { Node } from './node';

export class Profile extends Node {

    constructor() {
        super();
    }
    Location: string;

    LocationType: string;

    Theme: string;

    View: string;

    UserUUID: string;
}
