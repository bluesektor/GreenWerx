import { AfterViewInit, ChangeDetectorRef, Component, NgZone, OnInit, ViewChild, Input, Output,
  ElementRef, EventEmitter, OnChanges, SimpleChanges  } from '@angular/core';
import {  EventLocation, MapData, MarkerData } from '../../models/index';
import { EventService } from '../../services/events/event.service';
import { ServiceResult } from '../../models/serviceresult';
import { LocationService } from '../../services/geo/locations.service';
import {GoogleMapsService} from '../../services/geo/googlemaps.service';
import { Events } from '@ionic/angular';
import { timer } from 'rxjs';
import {SelectItem} from 'primeng/primeng';
declare var google: any;
import * as _ from 'lodash';
import { ObjectFunctions } from '../../common/object.functions';

@Component({
selector: 'geolocation-component',
templateUrl: 'geolocation.component.html',
styleUrls: ['./geolocation.component.scss'],
providers: [LocationService]
})

export class GeoLocationComponent implements  OnInit, AfterViewInit, OnChanges {
showLoadingMap = false;
@Input() ShowSaveButton = true;
@Input() ShowChangeLocationButton = true;
@Input() ShowPreviousLocationsCBO = true;
@Input() mapOnly = false;
@Input() ShowSaveAddButton = true;
@Input() UUID?: string;
@Input() UUIDType?: string; // event, location
@Input() category: string; // cruise, resort..
@Input() isNew: boolean; // is the event new or edit
@Input() eventUUID: string;
@Output() eventSaveData = new EventEmitter<string>();
isValidLocation = false;
canGetLocation = true;
changeLocation = false;
savingEvent = false;
@ViewChild('mapCanvas') mapElement: ElementRef;

// ******** Search Location
latitude: number;
longitude: number;
autocompleteService: any;
placesService: any;
query  = '';
places: any = [];
filteredPlaces: string[];
selectedPlace = '';
searchDisabled: boolean;
saveDisabled: boolean;
map: any;
showSearchResults = false;
searching = false;
location: EventLocation = new EventLocation();
btnChangeLocationTitle = 'Change Location';
redirectAfterSave = false;
previousLocations: SelectItem[];
locations: EventLocation[] = [];
processing = false;


constructor(  public mapsService: GoogleMapsService,
         public eventService: EventService,
         public locationService: LocationService,
         public messages: Events,
         private cdr: ChangeDetectorRef,
         private ngZone: NgZone  ) {
console.log('geolocation.component.ts constructor UUID:', this.UUID);
if (this.isNew === true) {
this.btnChangeLocationTitle = 'Set Location';
}
}

ngOnChanges(changes: SimpleChanges) {
  console.log('geolocation.component.ts ngOnChanges UUID:', this.UUID);
  if (this.isNew === true) {
    this.btnChangeLocationTitle = 'Set Location';
  }
  for (const propName of Object.keys(changes)) {
    if ( propName === 'UUID' ) {
      this.loadEventLocation();
    return;
  }
  /*
  const change = changes[propName];
  const curVal  = JSON.stringify(change.currentValue);
  const prevVal = JSON.stringify(change.previousValue);
  console.log('geolocation.component.ts ngOnChanges propName:' , propName);
  console.log('geolocation.component.ts ngOnChanges change:' , change);
  console.log('geolocation.component.ts ngOnChanges curVal:' , curVal);
  console.log('geolocation.component.ts ngOnChanges prevVal:' , prevVal);
  */
  }

}

ngOnInit() {
  if ( this.ShowPreviousLocationsCBO === true ) {
    this.loadPreviousLocations();
  }
  this.searchDisabled = true;
  this.saveDisabled = true;
  this.canGetLocation = false;
  console.log('geolocation.component.ts ngOnInit UUID:', this.UUID);
  if (navigator.geolocation) {
  this.canGetLocation = true;
  }
}

// NOTE: google maps has to run in AfterViewInit
ngAfterViewInit(): void {

  console.log('geolocation.component.ts ngAfterViewInit UUID:', this.UUID);
  const mapLoaded = this.mapsService.init(this.mapElement.nativeElement).then(() => {
  console.log('geolocation.component.ts ngOnInit this.mapsService.init');
  if ( google.maps.places !== undefined) {
  this.autocompleteService = new google.maps.places.AutocompleteService();
  this.placesService = new google.maps.places.PlacesService(this.mapsService.map);
  }
  this.searchDisabled = false;

  });

  console.log('geolocation.component.ts ngOnInit UUID:', this.UUID);
  if ( this.UUID === undefined || this.UUID === null || this.UUID === '') { return; }

  if (!this.mapsService.mapInitialised) {
  const source = timer(1000, 3000);
  const subscribe = source.subscribe((val) => {
    console.log(val);
    console.log('geolocation.component.ts ngOnInit this.mapsService.mapInitialised:', this.mapsService.mapInitialised);
    if ( this.mapsService.mapInitialised === true) {
      subscribe.unsubscribe();
      this.loadEventLocation();
    }
  });
  return;
  }
  console.log('geolocation.component.ts ngOnInit this.mapsService.mapInitialised:', this.mapsService.mapInitialised);

  this.loadEventLocation();
}

onCboChangePreviousLocation(event) {
console.log('onCboChangePreviousLocation:', event.value);

for ( let i = 0; i < this.locations.length; i++) {

// If it's new no need to see if it was attached to the event before..
if (this.isNew === true && this.locations[i].UUID === event.value ) {
 this.location.UUID = ''; // we wan't to create a new location record to match each event. this way
                         // we can log the changes.
 this.UUID = '';
 this.location = this.locations[i];
 this.location.UUID = '';
 console.log('geolocation.component.ts onCboChangePreviousLocation found location this.location :', this.location );
 this.showEventLocation(this.location);
 break;
}

const usedLocation = this.locationUsedForEvent(event.value, this.eventUUID);
if (usedLocation !== null) {
 this.location = usedLocation;
 break;
}

if (this.locations[i].UUID === event.value) {
 this.location = this.locations[i];
 this.location.UUID = ''; // we wan't to create a new location record to match each event. this way
 // we can log the changes.
 this.UUID = '';
 console.log('geolocation.component.ts onCboChangePreviousLocation found location this.isNew:', this.isNew );
 break;
}
}
}

locationUsedForEvent(locationUUID: string, eventUUID: string): EventLocation {
  for ( let i = 0; i < this.locations.length; i++) {
  if (this.locations[i].UUID === eventUUID &&
  this.locations[i].EventUUID === this.eventUUID ) {
    return this.locations[i];
  }
  }
  return null;
}

async loadPreviousLocations() {
  await this.eventService.getPreviousLocations().subscribe((response) => {
  const data = response as ServiceResult;
  if (data.Code !== 200) {
    this.messages.publish('api:err', data);
    return;
  }
  this.previousLocations = [];
  if (ObjectFunctions.isValid(data.Result) === false) {
    return;
  }
  data.Result.forEach(location => {
    this.locations.push(location);
    this.previousLocations.push({ label: location.Name, value: location.UUID });
  });
  }, (err) => {
  this.messages.publish('service:err', err);
  return;
  });

}

saveEvent(redirect: boolean) {
this.savingEvent = true;
this.redirectAfterSave = redirect;
this.eventSaveData.emit('saveEvent');
}

// Queries the EventLocations table by eventLocation uuid for coordinates.
//
async loadEventLocation() {

  if (this.UUID === '' || this.UUID === undefined) {
  console.log('geolocation.component.ts getEventLocation UUID not set');
  return;
  }
  await this.eventService.getEventLocation(this.UUID).subscribe((response) => {
  console.log('geolocation.component.ts getEventLocation response:', response);
  if ( response.hasOwnProperty('Code') ) {
  const data = response as ServiceResult;
  if (data.Code !== 200) {
    this.messages.publish('api:err', data);
    this.showLoadingMap = false;
    return;
  }
  if (data.Result === null) {
    this.showLoadingMap = false;
    return;
  }
  this.location  =  data.Result  as EventLocation;
  console.log('geolocation.component.ts Location XXX:',  this.location);
  }
  }, (err) => {
  this.showLoadingMap =  false;
  this.messages.publish('service:err', err);
  return;
  });

}

showEventLocation(eLocation: EventLocation) {
console.log('geolocation.component.ts showEventLocation:',  eLocation);
const mapData = new MapData();
const markerData = new MarkerData();
markerData.name = eLocation.Name;
markerData.title = this.location.Name;
markerData.center = true;
markerData.lat = parseFloat( eLocation.Latitude );
markerData.lng = parseFloat( eLocation.Longitude );
if ( markerData.lat === undefined || markerData.lng === undefined ||
markerData.lat === NaN ||  markerData.lng === NaN ) {
console.log('geolocation.component.ts invalid coordinates Location:',  eLocation);
console.log('geolocation.component.ts this.isValidLocation:', this.isValidLocation);
this.isValidLocation = false;
this.showLoadingMap = false;
return;
}
this.isValidLocation = true;
console.log('geolocation.component.ts this.isValidLocation:', this.isValidLocation);
mapData.markers.push(markerData);
this.showMapMarkers(mapData);
}

showMapMarkers(mapData: any) {
if (!google) {
console.log('geolocation.component.ts google not set returning');
return;
}
console.log('geolocation.component.ts showMapMarkers');
if (this.mapElement === null || this.mapElement === undefined) {
console.log('geolocation.component.ts showMapMarkers this.mapElement is null or undefined');
this.showLoadingMap =  false;
return;
}
const mapEle = this.mapElement.nativeElement;

const map = new google.maps.Map(mapEle, {
center: mapData.markers.find((d: any) => d.center),
zoom: 16
});

mapData.markers.forEach((markerData: any) => {
console.log('geolocation.component.ts showMapMarkers markerData:', markerData);

const infoWindow = new google.maps.InfoWindow({
 content: `<h5>${markerData.name}</h5>`
});


const marker = new google.maps.Marker({
 position: markerData,
 map: map,
 title: markerData.name
});

const tmp = new google.maps.LatLng( markerData.lat, markerData.lng);
console.log('geolocation.component.ts showMapMarkers google.maps.LatLng:', tmp);
console.log('geolocation.component.ts showMapMarkers markerData:', markerData);

this.mapsService.map.panTo( {lat: markerData.lat, lng: markerData.lng});
marker.addListener('click', () => { infoWindow.open(map, marker); });
});

google.maps.event.addListenerOnce(map, 'idle', () => {
mapEle.classList.add('show-map');
setTimeout(() => {
   google.maps.event.trigger(map, 'resize');
   console.log('geolocation.component.ts showMapMarkers  google.maps.event.trigger');
 }, 250);

});
/*   google.maps.event.addListenerOnce(map, 'tilesloaded', function() {
google.maps.event.addListenerOnce(map, 'tilesloaded', function() {
   google.maps.event.trigger(map, 'resize');
});
});*/
this.showLoadingMap = false;
}

toggleSelectLocationView() {
this.changeLocation = !this.changeLocation;
this.cdr.detectChanges();
}

// ==================================================== Button Get and show location
getCurrentLocation() {
if (this.canGetLocation === false || this.processing === true ) { return; }

this.processing = true;
navigator.geolocation.watchPosition((position) => {
console.log('event-edit.ts. getCurrentLocation position:', position);

const mapData = new MapData();
const markerData = new MarkerData();
markerData.name = this.location.Name;
markerData.title = this.location.Name;
markerData.center = true;
markerData.lat = position.coords.latitude;
markerData.lng = position.coords.longitude;
this.location.Latitude = position.coords.latitude.toString();
this.location.Longitude = position.coords.longitude.toString();
this.isValidLocation = true;
console.log('geolocation.component.ts this.isValidLocation:', this.isValidLocation);
mapData.markers.push(markerData);
this.showMapMarkers(mapData);
this.getAddress( position.coords.latitude , position.coords.longitude);
});
}

getAddress(latitude: number, longitude: number) {
const latlng = {lat: latitude, lng: longitude};
console.log('geolocation.component.ts getAddress latlng:', latlng);
const geocoder = new google.maps.Geocoder;
const self = this;
geocoder.geocode({'location': latlng}, function(results, status) {
console.log('geolocation.component.ts getAddress geocode status:', status);
if (status === 'OK') {
 if (results[0]) {
   console.log('geolocation.component.ts getAddress result results[0]:', results[0]);
   self.location.Address1 = '';

   for (let ac = 0; ac < results[0].address_components.length; ac++) {
     const component = results[0].address_components[ac];
     switch (component.types[0]) {
         case 'street_number':
         self.location.Address1 += component.long_name.toString() + ' ';
           break;
         case 'route':
         self.location.Address1 += component.long_name;
         self.location.Address2 = component.long_name;
         break;
         case 'locality':
         self.location.City = component.long_name;
           break;
         case 'administrative_area_level_1':
         self.location.State = component.short_name;
             break;
         case 'postal_code':
         self.location.Postal = component.short_name;
             break;
         case 'country':
         self.location.Country = component.long_name;
           break;
         // Type: number;
         // Category: string;
     }
   }
   self.processing = false;
   console.log('geolocation.component.ts getAddress result this.location:', this.location);
 } else {
   self.processing = false;
   console.log('geolocation.component.ts getAddress Geocoder No results found');
 }
} else {
 self.processing = false;
 console.log('geolocation.component.ts getAddress Geocoder failed due to: ' + status);
}
});
}

// ***************************************** Search Location
searchPlace(event) {
this.searching = true;
console.log('geolocation.component.ts searchPlace event:', event);
this.filteredPlaces = [];
this.places = [];
this.saveDisabled = true;

console.log('geolocation.component.ts searchPlace this.searchDisabled:', this.searchDisabled );

if (this.query.length === 0 ||  this.searchDisabled === true) {
console.log('geolocation.component.ts searchPlace returning');
this.searching = false;
return;
}

const config = {
types: ['geocode'],
input: this.query
};
const self = this;
this.showSearchResults = true;
this.autocompleteService.getPlacePredictions(config, (predictions, status) => {

 if (status === google.maps.places.PlacesServiceStatus.OK && predictions ) {
   console.log('geolocation.component.ts searchPlace google.maps.places.PlacesServiceStatus.OK' );

     predictions.forEach((prediction) => {
         self.places.push(prediction);
     });
     self.filteredPlaces = self.places;
     self.searching = false;
     this.cdr.detectChanges();
     console.log('geolocation.component.ts searchPlace self.places', self.places);
 } else {
   self.searching = false;
   this.cdr.detectChanges();
 }
});
}

selectPlace ( place ) {
console.log('geolocation.component.ts selectPlace place:', place);
if (place === null || place === undefined) { return; }

this.location.UUParentID = place.id;
this.location.UUParentIDType = 'place';
this.places = [];
this.placesService.getDetails({placeId: place.place_id}, (details) => {
 this.ngZone.run(() => {
   console.log('geolocation.component.ts getDetails details:', details);
     this.location = this.mapsService.parseLocation(details.address_components);
     this.location.Latitude =  details.geometry.location.lat();
     this.location.Longitude = details.geometry.location.lng();
     this.saveDisabled = false;
     this.query = place.description;

      const mapData = new MapData();
     const markerData = new MarkerData();
     markerData.name = this.location.Name;
     markerData.title = this.location.Name;
     markerData.center = true;
     markerData.lat = details.geometry.location.lat();
     markerData.lng = details.geometry.location.lng();
     this.isValidLocation = true;
     console.log('geolocation.component.ts this.isValidLocation:', this.isValidLocation);
     mapData.markers.push(markerData);
     this.showMapMarkers(mapData);
     console.log('geolocation.component.ts selectPlace this.placesService.getDetails this.selectedLocation:', this.location);
 });
});
}
}
