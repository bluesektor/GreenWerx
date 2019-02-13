import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/map';
import { Api } from '../api/api'; // '../api/api.service';
@Injectable({
    providedIn: 'root' // without this you'll get the 'static injector error'  message.
  })
export class LocationService {

    constructor(public api: Api) { }

    getLocation(locationUUID: string, locationType: string ) {
        return this.api.invokeRequest('GET', 'api/Locations/' + locationUUID + '/Types/' + locationType );
    }

    saveLocation(location: any) {
        return this.api.invokeRequest('POST', 'api/Locations/Save', JSON.stringify(location));
    }

    saveHostLocation(location: any) {
        return this.api.invokeRequest('POST', 'api/Locations/Account/Save', JSON.stringify(location));
    }

    getAreaData(latitude: any, longitude: any, range: any, filter: any) {
        return this.api.invokeRequest('POST', 'api/Locations/InArea/lat/' + latitude + '/lon/' +
            longitude + '/range/' + range, JSON.stringify(filter));
    }
}
