
import { Node } from './node';
import { Time } from '@angular/common';
import {Event } from './event';

export class EventGroup extends Node {

      Body: string;

      Category: string;

      StartDate: Date;

      StartTime: string;

      EndDate: Date;

      EndTime: string;

      EventUUID: string;

      SessionUUID: string;

      Events: Event[] = [];
}
