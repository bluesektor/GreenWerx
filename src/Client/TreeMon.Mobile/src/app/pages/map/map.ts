import { Component, ElementRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { Platform } from '@ionic/angular';
import { LocationService } from '../../services/geo/locations.service';
import { Filter, Screen } from '../../models/index';
import { ServiceResult} from '../../models/serviceresult';
import {  MapData, MarkerData } from '../../models/index';
import {GoogleMapsService} from '../../services/geo/googlemaps.service';
declare var google: any;
import { Events } from '@ionic/angular';


@Component({
  selector: 'page-map',
  templateUrl: 'map.html',
  styleUrls: ['./map.scss'],
  providers: [LocationService],
  encapsulation: ViewEncapsulation.None
})
export class MapPage {
  apiKey  = 'AIzaSyC6LsWGcWdG923U2z-ss72T3mZPMtkeQg0';
  @ViewChild('mapCanvas') mapElement: ElementRef;
  canGetLocation = false;
  mapData = new MapData();
  loading = false;

  constructor(  public locationService: LocationService,
    public messages: Events,
    public mapsService: GoogleMapsService,
                public platform: Platform  ) {  }

  ionViewDidEnter() {
    if (navigator.geolocation) {
      this.canGetLocation = true;
    }
    this.loading = true;
    console.log('map.ts ionViewDidEnter navigator.geolocation:', navigator.geolocation);

    navigator.geolocation.getCurrentPosition((position) => {   // .watchPosition((position) => {
      console.log('map.ts. ionViewDidEnter position:', position);
      this.mapData.markers = [];
      const markerData = new MarkerData();
      markerData.name = 'You are here.';
      markerData.center = true;
      markerData.lat = position.coords.latitude;
      markerData.lng = position.coords.longitude;
      this.mapData.markers.push(markerData);
      this.getAreaData(markerData, 10);
    });

  }

  getAreaData(markerData: MarkerData, distance: number) {

    this.locationService.getAreaData(markerData.lat, markerData.lng, 10, null ).subscribe(response => {
      const data = response as ServiceResult;
        console.log('map.ts getAreaData response:', response);

        if (data.Code !== 200) {
          this.loading = false;
          this.messages.publish('api:err', data);
          return false;
        }

        for (let i = 0; i < data.Result.Distances.length; i++) {
          const marker = new MarkerData();
          marker.name =  data.Result.Distances[i].Name;
          marker.title = data.Result.Distances[i].Name;

          // LocationType = s.LocationType, // todo change the icon based on type
          marker.center = false;
          marker.lat = data.Result.Distances[i].Latitude;
          marker.lng = data.Result.Distances[i].Longitude;
          this.mapData.markers.push(marker);
        }
        this.showMapMarkers( this.mapData);
    }, err => {
        this.loading = false;
        this.messages.publish('service:err', err.statusText, 4);

    });
  }

  showMapMarkers( mapInfo: any) {
    if (!google) {
      this.loading = false;
      console.log('geolocation.component.ts google not set returning');
      return;
    }
    console.log('geolocation.component.ts showMapMarkers');
    if (this.mapElement === null || this.mapElement === undefined) {
      this.loading = false;
      console.log('geolocation.component.ts showMapMarkers this.mapElement is null or undefined');
      return;
    }
    const mapEle = this.mapElement.nativeElement;

    const map = new google.maps.Map(mapEle, {
      center: mapInfo.markers.find((d: any) => d.center),
      zoom: 16
    });

    for (let i = 0; i < mapInfo.markers.length; i++ ) {
      const markerData  = this.mapData.markers[i];

      console.log('geolocation.component.ts showMapMarkers markerData:', markerData);
      let marker  = null;

      if (markerData.center === true) {
        marker = new google.maps.Marker({
          map: map,
          title: markerData.name,
          clickable: true,
          position: new google.maps.LatLng(markerData.lat, markerData.lng)
      });
      } else {
          marker = new google.maps.Marker({
            map: map,
            icon: {
              path: 'M416 208H272V64c0-17.67-14.33-32-32-32h-32c-17.67 0-32 14.33-32 32v144H32c-17.67 0-32 14.33-32 32v32c0 17.67 14.33 3' +
              '2 32 32h144v144c0 17.67 14.33 32 32 32h32c17.67 0 32-14.33 32-32V304h144c17.67 0 32-14.33 32-32v-32c0-17.67-14.33-32-32-32z',
                scale: 0.05,
                strokeWeight: 1,
                strokeColor: 'black',
                strokeOpacity: 1,
                fillColor: 'green',
                fillOpacity: 0.7,
            },
          title: markerData.name,
            clickable: true,
            position: new google.maps.LatLng(markerData.lat, markerData.lng)
        });
      }
    const infoWindow = new google.maps.InfoWindow({
      content: `<h5>${markerData.name}</h5>`
    });

      marker.addListener('click', () => { infoWindow.open(map, marker); });
    }

    google.maps.event.addListenerOnce(map, 'idle', () => {
      mapEle.classList.add('show-map');
       setTimeout(() => {
        google.maps.event.trigger(map, 'resize');
        console.log('map.ts showMapMarkers  google.maps.event.trigger');
      }, 250);
    });

    this.loading = false;
  }

}
