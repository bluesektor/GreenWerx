// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api';
import { Account, EventLocation , Favorite, Filter,  Screen} from '../../models/index';
import { Observable, of as observableOf} from 'rxjs';
import {EventsDashboard} from '../../models/index';

@Injectable({
  providedIn: 'root'
})
export class EventService  {

   public Dashboard: EventsDashboard;

   public Favorites: Favorite[] = [];

   public Categories: string[] = [];

   public Properties: {name: string, caption: string, isChecked: boolean}[] = [];

   public Accounts: Account[] = [];

   // These are selected screens by user in the event-filter.ts
   // NOTE: in the filter dialog this only supports boolean fields i.e. private, active..
   // public EventScreens: Screen[] = [];
    public EventFilter: Filter = new Filter();

   public AvailableScreens:  Screen[] = []; // cache this


    constructor(private api: Api ) {
        this.Properties.push({
            name:  'private',
            caption: 'Include Private',
            isChecked: false
          });
    }

    logOut() {
        this.EventFilter  = new Filter();
        this.Favorites = [];
        this.AvailableScreens = [];
    }

    getEventCategories() {
        return this.api.invokeRequest('GET', 'api/Events/Categories' );
    }

    getDashboard(view: string, filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Apps/Dashboard/' + view , JSON.stringify(filter) );
    }

    addEvent(event) {
      return this.api.invokeRequest('POST', 'api/Events/Add', JSON.stringify(event));
    }

    deleteEvent(eventUUID) {
      return this.api.invokeRequest('DELETE', 'api/Events/Delete/' + eventUUID, ''    );
    }

    getEvents(filter?: Filter):  Observable<Object> {
      return this.api.invokeRequest('POST', 'api/Events'  , JSON.stringify(filter) );
    }

    getHostEvents(accountUUID: string , filter?: Filter):  Observable<Object> {
        return this.api.invokeRequest('POST', 'api/Events/Hosts/' + accountUUID  , JSON.stringify(filter) );
      }

    getEvent(eventId) {
      if (this.Dashboard !== undefined && this.Dashboard.Events !== undefined) {
        for (let i = 0; i < this.Dashboard.Events.length; i++) {
            if ( this.Dashboard.Events[i].UUID === eventId ) {
                return observableOf(this.Dashboard.Events[i] );
            }
        }
      }
      return this.api.invokeRequest('GET', 'api/EventBy/' + eventId, ''    );
        }

    updateEvent(event) {
      return this.api.invokeRequest('PATCH', 'api/Events/Update', event);
    }

    getEventLocation(eventUUID) {
        if (this.Dashboard !== undefined && this.Dashboard.Locations !== undefined) {
            for (let i = 0; i < this.Dashboard.Locations.length; i++) {
                if ( this.Dashboard.Locations[i].EventUUID === eventUUID ) {
                    return observableOf(this.Dashboard.Locations[i] );
                }
            }
        }
        return this.api.invokeRequest('GET', 'api/Events/Locations/' + eventUUID, ''    );
    }

    getPreviousLocations() {
        return this.api.invokeRequest('GET', 'api/Events/Locations/Account' );
    }

    saveEventLocation(eventLocation: EventLocation ) {
       return this.api.invokeRequest('POST', 'api/Events/Location/Save',  JSON.stringify(eventLocation)    );
    }

    // Returns reminders flagged as favorite
    getFavorites(filter: Filter) {
        return this.api.invokeRequest('POST', 'api/Events/Favorites', JSON.stringify(filter) );
    }

    hasFavorite(eventUUID: string): boolean {
        if (this.Favorites.length === 0) {
            console.log('event.service.ts hasFavorite lenght is 0 returing false');
            return false;
        }
        let found = false;
        for (let i = 0; i < this.Favorites.length; i++) {
            console.log('event.service.ts hasFavorite looking at this.Favorites[i].Event.UUID:', this.Favorites[i].Event.UUID);

            console.log('event.service.ts hasFavorite lenght is 0 returing false');
            if (this.Favorites[i].Event.UUID === eventUUID ) {
                console.log('event.service.ts hasFavorite found eventUUID:', eventUUID);
                found = true;
                return true;
            }
        }
        return found;
    }

    addFavorite(eventUUID: string):  Observable<Object> {
        if (this.hasFavorite(eventUUID) === true) {
            return observableOf(null);
        }
        return this.api.invokeRequest('POST', 'api/Events/' + eventUUID + '/Favorite'  );
    }

    removeFavorite(eventUUID: string):  Observable<Object> {
        //  const index = this._favorites.indexOf(sessionName);
        for (let i = 0; i < this.Favorites.length; i++) {
            if (this.Favorites[i].Event.UUID === eventUUID ) {
                this.Favorites.splice(i, 1);
            }
        }
        return this.api.invokeRequest('DELETE', 'api/Events/' + eventUUID + '/Favorite'  );
    }

}
