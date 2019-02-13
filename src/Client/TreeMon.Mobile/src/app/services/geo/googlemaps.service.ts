import { Injectable } from '@angular/core';
// import { Platform } from 'ionic-angular';
// import { ConnectionStatus, NetworkService } from '../network.service';
import { EventLocation } from '../../models/location';
import { ObjectFunctions } from '../../common/object.functions';
declare var google: any;

@Injectable({
    providedIn: 'root'
 })

 export class GoogleMapsService  {
    mapElement: any;
    map: any;
    mapInitialised  = false;
    mapLoaded: any;
    mapLoadedObserver: any;
    currentMarker: any;
    apiKey  = 'YOURGOOGLEAPIKEY';

    constructor() { }

    init(mapElement: any ): Promise<any> {
      this.mapElement = mapElement;
      return this.loadGoogleMaps();
    }

    loadGoogleMaps(): Promise<any> {
      console.log('googlemaps.service.ts loadGoogleMaps');
      return new Promise((resolve) => {

        if (typeof google === 'undefined' || typeof google.maps === 'undefined') {

          console.log('Google maps JavaScript needs to be loaded.');
          // this.disableMap();
          // if (this.connectivityService.status === ConnectionStatus.Online ) {

            window['initMap'] = () => {
              console.log('googlemaps.service window[initMap].');
              this.initMap().then(() => {
                console.log('googlemaps.service window[initMap] resolve true.');
                resolve(true);
              });

              // this.enableMap();
            };

            const script = document.createElement('script');
            script.id = 'googleMaps';

            if (this.apiKey) {
              console.log('googlemaps.service.ts load places library');
              script.src = 'http://maps.googleapis.com/maps/api/js?key=' + this.apiKey + '&callback=initMap&libraries=places';
            } else {
              console.log('googlemaps.service.ts NO load places library');
              script.src = 'http://maps.google.com/maps/api/js?callback=initMap';
            }
            console.log('googlemaps.service document.body.appendChild(script).');
            document.body.appendChild(script);

         // }
        } else {

         // if (this.connectivityService.status === ConnectionStatus.Online) {
          console.log('googlemaps.service initMap.');
            this.initMap();
        //    this.enableMap();
        //  } else {
         //   this.disableMap();
        //  }

          resolve(true);

        }
         // this.addConnectivityListeners();
      });
    }

    initMap(): Promise<any> {

      const options = {
        enableHighAccuracy: true,
        timeout: 5000,
        maximumAge: 0
      };

     // this.GoogleMaps = google;
      this.mapInitialised = true;

      return new Promise((resolve) => {
        const latLng = new google.maps.LatLng(33.449372, -112.068949);
        const mapOptions = {
          center: latLng,
          zoom: 15,
          mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        console.log('googlemaps.service initMap a.');
        this.map = new google.maps.Map(this.mapElement, mapOptions);
          resolve(true);
      });
      /*
      this creates an event that updates the map with the current position. i.e. tracks position.
      return new Promise((resolve) => {

        navigator.geolocation.watchPosition((position) => {
          console.log('event-edit.ts. getLocation position:', position);

          const latLng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);

          const mapOptions = {
            center: latLng,
            zoom: 15,
            mapTypeId: google.maps.MapTypeId.ROADMAP
          };

          this.map = new google.maps.Map(this.mapElement, mapOptions);
          resolve(true);
        });
      });
      */
    }

    parseLocation(address_components: any): EventLocation {
      const location = new EventLocation();
      for (let ac = 0; ac < address_components.length; ac++) {
        const component = address_components[ac];
        if ( ObjectFunctions.isNullOrWhitespace(component.long_name) === true || component.long_name === 'undefined') {
          continue;
        }
        switch (component.types[0]) {
            case 'street_number':
              if ( ObjectFunctions.isNullOrWhitespace(location.Address1) === true) {
                location.Address1 = component.long_name.toString() + ' ';
              } else {
                location.Address1 += component.long_name.toString() + ' ';
              }
              break;
            case 'route':
              if ( ObjectFunctions.isNullOrWhitespace(location.Address1) === true) {
                location.Address1 = component.long_name.toString() + ' ';
              } else {
                location.Address1 += component.long_name.toString() + ' ';
              }
              break;
            case 'locality':
              location.City = component.long_name;
              break;
            case 'administrative_area_level_1':
              location.State = component.short_name;
                break;
            case 'postal_code':
              location.Postal = component.short_name;
                break;
            case 'country':
              location.Country = component.long_name;
              break;
        }
      }
      return location;
    }


/*
   disableMap(): void {

     // if (this.pleaseConnect) {
     //   this.pleaseConnect.style.display = 'block';
     // }

    }

    enableMap(): void {

      // if (this.pleaseConnect) {
     //   this.pleaseConnect.style.display = 'none';
     // }

    }

    addConnectivityListeners(): void {

      this.connectivityService.getNetworkStatus().subscribe(() => {

        if (this.connectivityService.status === ConnectionStatus.Online) {
            setTimeout(() => {
                if (typeof google === 'undefined' || typeof google.maps === 'undefined') {
                    this.loadGoogleMaps();
                } else {
                    if (!this.mapInitialised) {
                    this.initMap();
                    }

                    this.enableMap();
                }
            }, 2000);
        } else {
            this.disableMap();
        }
      });
    }

     */
 }
