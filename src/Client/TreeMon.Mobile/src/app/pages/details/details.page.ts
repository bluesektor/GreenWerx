import { ChangeDetectorRef, Component, OnInit, ViewEncapsulation, ViewChild, Input, Host } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService} from '../../services/index';
import { EventService } from '../../services/events/event.service';
import { LocationService } from '../../services/geo/locations.service';
import { SessionService } from '../../services/session.service';
import {InventoryService } from '../../services/store/inventory.service';
import { Events } from '@ionic/angular';
import { ServiceResult } from '../../models/serviceresult';
import {Account, Event, FileEx, EventLocation, Filter, Screen, EventGroup} from '../../models/index';
import {GeoLocationComponent } from '../../components/geo/geolocation.component';
import { ObjectFunctions } from '../../common/object.functions';
import * as moment from 'moment';
import * as momentTimezone from 'moment-timezone';


@Component({
  selector: 'app-details',
  templateUrl: './details.page.html',
  styleUrls: ['./details.page.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class DetailsPage implements OnInit {

  uuid: string;
  type: string;
  item: any;
  isNew = false;
  loading = false;
  editing = false;
  processingRequest  = false;

  eventLocationUUID: string;
  @ViewChild('geoLocation') geoLocation: GeoLocationComponent;
  @ViewChild('eventsList') eventsList: any;
  events: Event[] = [];
  eventCount = 0;
   // groups: EventGroup[] = [];

  // Image upload
  images:  Array<FileEx> = [];
  @Input() maxFileUpload = 1;
  picsReadOnly = false;
  imageFormData = new FormData();
  updateImage = false;

  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    public accountService: AccountService,
    public sessionService: SessionService,
    public inventoryService: InventoryService,
    public eventService: EventService,
    public messages: Events,
    public locationService: LocationService
  ) { }

  ngOnInit() {
    this.uuid =  this.route.snapshot.paramMap.get('uuid');
    this.type =  this.route.snapshot.paramMap.get('type');
    if (this.type !== '' || this.type !== null ) {
      this.type = this.type.toUpperCase();
    }
    console.log('details.page.ts ngOnInit uuid', this.uuid);
    console.log('details.page.ts ngOnInit type', this.type);
  }

    ionViewWillEnter() {
    this.geoLocation.changeLocation = false;

    switch (this.type) {
      case 'ACCOUNT':
        console.log('details.page.ts ionViewWillEnter switch type', this.type);
        this.item = new Account(); // not sure if this is valid or matters..
        const canCreateHost  = this.sessionService.isUserInRole('OWNER');
        if ( this.uuid === 'newHost'  ) {
          if ( canCreateHost === true ) {
            this.isNew = true;
            this.item.AccountUUID = this.sessionService.CurrentSession.AccountUUID;
            this.item.CreatedBy = this.sessionService.CurrentSession.UserUUID;
            this.editHost();
            return;
          }
          return; // can't load an id as 'new', it's not valid.
        }
        this.loading = true;
        this.getAccount(this.uuid);
        const filter = new Filter();
        this.loadEvents(this.uuid, filter, true);
        break;
      case 'EVENT':
        console.log('details.page.ts ionViewWillEnter switch type', this.type);
        this.item = new Event();
        this.loadEvent();
        break;
    }
 }

 async loadEvent() {
  await this.eventService.getEvent(this.uuid).subscribe((response) => {
    if ( response.hasOwnProperty('Code') ) {
      this.loading = false;
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.messages.publish('api:err', data);
        return;
      }
      this.item = data.Result as Event;
      if (this.item.Image === '' || this.item.Image === null ) {
        this.item.Image = '/assets/img/blankprofile.png';
      }
      this.eventLocationUUID = this.item.EventLocationUUID;
      console.log('event-edit.ts this.event 1:' , this.item);
      console.log('event-edit.ts this.eventLocationUUID:' , this.eventLocationUUID);

    } else { // pulled from local cache.
      this.item = response as Event;
      console.log('event-edit.ts cached this.event:' , this.item);
    }
 }, (err) => {
    this.loading = false;
    this.messages.publish('service:err', err);
    return;
 });
}

  editHost() {
    this.editing = true;
     this.geoLocation.changeLocation = true;
  }

  cancelEditHost() {
    this.editing = false;
    this.geoLocation.changeLocation = false;
  }

  canEditHost(): boolean {
    let canEdit = false;

    if ( this.sessionService === undefined ||
         this.sessionService.UserRoles === undefined) {
      return canEdit;
    }
    for ( let i = 0; i <  this.sessionService.UserRoles.length; i++) {
      const roleName = this.sessionService.UserRoles[i].Name.toUpperCase();

      if ( this.item.CreatedBy === this.sessionService.CurrentSession.UserUUID || roleName === 'OWNER' ) {
        canEdit = true;
        break;
      }
    }
    return canEdit;
  }

  goToSessionDetail(session: any) {
    this.router.navigateByUrl(`app/tabs/schedule:session/${session.id}`);
  }

  getAccount(accountUUID: string) {
    console.log('host-detail.ts getAccount accountUUID:', accountUUID);

    this.accountService.getAccount(accountUUID).subscribe((response) => {
        console.log('host-detail.ts getAccount response:', response);
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.loading = false;
        this.messages.publish('api:err', data);
        return;
      }
      this.item = data.Result;
      this.getLocation(this.item.LocationUUID, this.item.LocationType);

      if (this.item.Image === '' || this.item.Image === null ) {
        this.item.Image = '/assets/img/blankprofile.png';
      }
    });
  }

  getLocation(locationUUID: string, locationType: string) {

    console.log('host-detail.ts getLocation locationUUID:', locationUUID, locationType);

    this.locationService.getLocation(locationUUID, locationType ).subscribe((response) => {
      console.log('host-detail.ts getLocation response:', response);
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.loading = false;
        this.messages.publish('api:err', data);
        return;
      }

      if ( ObjectFunctions.isValid(   data.Result ) === false ) {
        this.loading = false;
        console.log('host-detail.ts getLocation no location returned');
        return;
      }
      const location = data.Result  as EventLocation;
      this.eventLocationUUID = location.UUID;
      this.geoLocation.location = location;
      this.geoLocation.showEventLocation(location);
      this.loading = false;
      this.cdr.detectChanges();

    });
  }

  saveHost() {
    let svcRequest = null;
    this.geoLocation.location.isDefault = true;
    if (ObjectFunctions.isNullOrWhitespace(this.geoLocation.location.UUIDType) === true) {
      this.geoLocation.location.UUIDType =  'EventLocation';
    }

    this.eventService.saveEventLocation( this.geoLocation.location ).subscribe((locRepsonse) => {
      const locData = locRepsonse as ServiceResult;
      if (locData.Code !== 200) {
        this.messages.publish('api:err', locData);
        return;
      }
      console.log('host-detail.ts saveHost locationService.saveHostLocation locData.Result:', locData.Result);
      this.geoLocation.location = locData.Result;
      this.item.LocationType = locData.Result.UUIDType;
      this.item.LocationUUID = locData.Result.UUID;

      if ( this.isNew === true ) {
        svcRequest = this.accountService.addAccount(this.item);
      } else {
        svcRequest =  this.accountService.updateAccount(this.item);
      }

      svcRequest.subscribe((response) => {
        const data = response as ServiceResult;
        this.cancelEditHost();
        if (data.Code !== 200) {
          this.messages.publish('api:err', data);
          return;
        }

        console.log('host-detail.ts saveHost accountService.updateAccount data.Result:', data.Result);
        this.item = data.Result;

        if (this.updateImage === true ) {
        // upload image
          console.log('host-detail.ts saveHost updateImage :' );
          const res = this.inventoryService.uploadFormEx(this.imageFormData, this.item.UUID, 'Account' );
          res.subscribe(imageResponse => {
            console.log('host-detail.ts saveHost updateImage imageResponse:', imageResponse);
            const imgData = imageResponse as ServiceResult;
            this.processingRequest = false;
              if (imgData.Code !== 200) {
                this.messages.publish('service:err', imgData.Message);
                  return false;
              }
              console.log('host-detail.ts saveHost updateImage image upload response:',  imgData.Result);
              this.images.push(imgData.Result);
              this.item.Image = imgData.Result.ImageThumb;
              this.updateImage = false;
              if (this.item.Image === '' || this.item.Image === null ) {
                this.item.Image = '/assets/img/blankprofile.png';
              }
            }, err => {
              this.processingRequest = false;
              this.messages.publish('service:err', err.statusText, 4);

              if (err.status === 401) {

                // setTimeout(() => {    this.navCtrl.push(LoginPage);}, 3000);
              }
          });
        } // END updateImage ===
      }); // END SAVE HOST =======
    }); // SAVE LOCATION ========================================================================

    console.log('host-detail.ts saveHost.');
    this.editing = false;
  }

  changeImageEvent(imageEvent, index) {
    console.log('Clicked to update picture');

    if ( this.images.length === this.maxFileUpload ) {
      this.messages.publish('service:err', 'Maximum number of images is ' + this.maxFileUpload);
      return;
    }

    if (!imageEvent.target.files || imageEvent.target.files.length === 0) {
      this.messages.publish('service:err', 'You must select a file to upload.');
      return;
    }

    this.processingRequest = true;
    const files =  imageEvent.target.files;

    for ( let i = 0; i < files.length; i++) {
      console.log('processing file:', i);
      const file = files[i];

       // Only pics
      if (!file.type.match('image')) {
        this.messages.publish('service:err', 'file type is not an image!');
        continue;
      }

      console.log('appending form data:', file);

      if (this.picsReadOnly === true) {
          console.log('gallery is read only, uploads are not allowed.');
          return;
      }
      // if it's new or the default image is empty then set the first image to the default image.
      //
      if (   this.isDefaultSet() === false ) {
          this.imageFormData.append('defaultImage', file);
          console.log('setting defaultImage');

      } else {

        this.imageFormData.append('settingImage', file);
          console.log('setting settingImage');
      }


      // show preview..
      if (imageEvent.target.files && imageEvent.target.files[0]) {
        const previewFile = imageEvent.target.files[0];

        const reader = new FileReader();
        reader.onload = e => this.item.Image = reader.result as string;

        reader.readAsDataURL(previewFile);
    }
    this.updateImage = true;
  }
}

  openHostSite(host: any) {
    if (host.WebSite === null || host.WebSite === '' ) {
      const data = new ServiceResult();
      data.Message = 'Host does not have a url for the website.';
      this.messages.publish('api:err', data);
      console.log('host-lists.ts openHostSite url is null or empty host:', host);
      return;
    }
    console.log('host-lists.ts openHostSite host:', host);
//    this.inAppBrowser.create(      `${host.WebSite}`,      '_blank'    );
  }

  loadEvents(accountUUID: string, filter: Filter, includeTimezone: boolean ) {
    console.log('host-detail.ts loadEvents accountUUID:', accountUUID);
    // Close any open sliding items when the events updates
    if (this.eventsList) {
      this.eventsList.closeSlidingItems();
    }
    this.loading = true;
 //   this.groups = [];
    this.events = [];
    this.eventCount = 0;
    // Screen by clients time and timezone.
    // If no timezone all event will be returned.
    if (includeTimezone === true) {
      filter.TimeZone =  momentTimezone.tz.guess();
    }

    // NOTE: in the service set the DefaultEvent value in web.config to empty to pull main events.
     this.eventService.getHostEvents(accountUUID, filter).subscribe((response) => {

      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.loading = false;
        this.messages.publish('api:err', data);
        return;
      }

      this.events = data.Result;
      this.eventCount = this.events.length;
      console.log('EVENTS.TS getEvents   this.eventCount:', this.eventCount);
//      this.initializeEvents();

    }, (err) => {
       this.messages.publish('service:err', err);
    });
  }

 saveEvent() {
    console.log('host-detail.ts saveEvent. really saves host info');
    this.saveHost();
  }

getCategoryIcon(category: string): string {
  if (ObjectFunctions.isNullOrWhitespace(category) === true) {
    return category;
  }
    console.log('events.ts getCategoryIcon category:', category);
    category = category.toUpperCase();
    let icon = '';
    switch (category) {
      case 'FOOD':
        icon = '<i class="fa fa-utensils"></i>';
        break;

    }

    return icon;
  }

  formatDate(eventDate: any, format: string): string {
    const date =  moment( eventDate).local();
    return date.format(format);
  }

  goBack() {
    this.router.navigateByUrl(`tabs/home`);
  }

  isNullOrWhitespace(val: string): boolean {
    return ObjectFunctions.isNullOrWhitespace(val);
  }

  isDefaultSet(): boolean {
    if ( !this.images || this.images.length === 0 ) {
      return false;
    }

    for (let i = 0; i < this.images.length; i++) {
      if (this.images[i].Default === true) {
        return true;
      }
    }
    return false;
  }
}
