
import { Node } from './node';

export class Account extends Node {
    LocationUUID: string;
    LocationType: string;

    BillingAddress: string;
    Description: string;
    Email: string;
    BillingPostalCode;

    FavoritedByUserUUID: string;

    FavoritedByAccountUUID: string;
}


