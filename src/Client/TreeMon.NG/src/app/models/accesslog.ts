

import { Node } from '../models/node';

export class AccessLog extends Node {

    AuthenticationDate: Date;

    IPAddress: string;

    UserName: string;

    /// Passed or failed the authentication scheme
    //
    Authenticated: boolean;

    FailType: string;

    // Where is the attempt coming from..
    //
    Vector: string;

}
