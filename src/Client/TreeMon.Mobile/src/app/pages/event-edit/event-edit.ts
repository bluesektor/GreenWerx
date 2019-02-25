import { Component, ElementRef, ViewChild, ViewEncapsulation, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../../services/events/event.service';
import { AccountService } from '../../services/user/account.service';
import { Event, EventLocation } from '../../models/index';
import { SessionService} from '../../services/session.service';
import { ServiceResult } from '../../models/serviceresult';
import { Events } from '@ionic/angular';
import {SelectItem} from 'primeng/primeng';
import {GeoLocationComponent } from '../../components/geo/geolocation.component';
import * as moment from 'moment';
import { DateTime } from 'ionic-angular';
import { DateTimeFunctions } from '../../common/date.time.functions';

@Component({
  selector: 'page-event-edit',
  templateUrl: './event-edit.html',
  styleUrls: ['./event-edit.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [EventService]
})
export class EventEditPage implements OnInit  {

  event: Event = new Event();
  newEvent = false;
  eventUUID = '';
  gender = '';
  categories: SelectItem[];
  accounts: SelectItem[];
  hasAccountList = false;
  eventLocationUUID = '';
  category: any;
  eventStartDate: any;
  eventStartTime: any;
  eventEndDate: any = new Date();
  eventEndTime: any;
  currentYear = new Date().getFullYear();

  @ViewChild('geoLocation') geoLocation: GeoLocationComponent;

  constructor(
    public eventService: EventService,
    private route: ActivatedRoute,
    private sessionService: SessionService,
    private accountService: AccountService,
   public messages: Events,
    public router: Router,
  ) {
    this.eventUUID =  this.route.snapshot.paramMap.get('uuid');
    console.log('event-edit.ts ngOnInit UUID:', this.eventUUID);
    this.eventStartDate = moment(new Date()).toISOString();
    this.eventEndDate = moment(new Date()).toISOString();
    this.eventStartTime = moment(new Date()).local().toISOString();
    this.eventEndTime = moment(new Date()).local().toISOString();
  }
  ngOnInit() {

    this.loadCategories();
    this.loadAccounts();

    if ( this.eventUUID === undefined || this.eventUUID === null || this.eventUUID === '') {
      this.newEvent = true;
      return;
    }

    this.loadEvent();
  }

  async loadEvent() {
    await this.eventService.getEvent(this.eventUUID).subscribe((response) => {
      if ( response.hasOwnProperty('Code') ) {
        const data = response as ServiceResult;
        if (data.Code !== 200) {
          this.messages.publish('api:err', data);
          return;
        }
        this.event = data.Result as Event;
        this.eventLocationUUID = this.event.EventLocationUUID;
        console.log('event-edit.ts this.event 1:' , this.event);
        console.log('event-edit.ts this.eventLocationUUID:' , this.eventLocationUUID);
        this.eventStartDate = this.event.StartDate;
        this.eventStartTime = this.event.StartTime;
        this.eventEndDate = this.event.EndDate;
        this.eventEndTime = this.event.EndTime;
      } else { // pulled from local cache.
        this.event = response as Event;
        console.log('event-edit.ts cached this.event:' , this.event);
      }
      this.category = this.event.Category;
   }, (err) => {
      this.messages.publish('service:err', err);
      return;
   });
  }

  async loadCategories() {
    this.categories = [];
    console.log('event-edit.ts loadCategories');
    if (this.eventService.Categories !== undefined && this.eventService.Categories.length > 0) {
      for (let i = 0; i < this.eventService.Categories.length; i++) {
        const tmp = this.eventService.Categories[i];
        this.categories.push({ label: tmp, value: tmp });
      }
      console.log('event-edit.ts loadCategories cached this.categories:', this.categories);
      return;
    }
   // this.loading = true;
   await this.eventService.getEventCategories().subscribe((response) => {
   //   this.loading = false;
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.messages.publish('api:err', data);
        return;
      }
      data.Result.forEach(name => {
        this.eventService.Categories.push(name);
        this.categories.push({ label: name, value: name });
      });
      console.log('event-edit.ts loadCategories this.categories:', this.categories);

    });
  }

  async loadAccounts() {
    this.accounts = [];
    console.log('event-edit.ts loadAccounts');
    if (this.eventService.Accounts !== undefined && this.eventService.Accounts.length > 0) {
      for (let i = 0; i < this.eventService.Accounts.length; i++) {
        const tmp = this.eventService.Accounts[i];
        this.accounts.push({ label: tmp.Name, value: tmp.UUID });
      }
      console.log('event-edit.ts loadAccounts cached this.accounts:', this.accounts);
      return;
    }
   // this.loading = true;
    let isOwnerAdmin = this.sessionService.isUserInRole('OWNER');
    if (isOwnerAdmin === false) {
      isOwnerAdmin =   this.sessionService.isUserInRole('ADMIN');
    }
    let accountThread = null;

    if (isOwnerAdmin === true) {
      console.log('event-edit.ts loadAccounts getAllAccounts');
      accountThread = this.accountService.getAllAccounts();
    } else if ( this.sessionService.isUserInRole('PUBLISHER') === true ) {
      console.log('event-edit.ts loadAccounts getAccounts');
      accountThread = this.accountService.getAccounts();
    } else { // todo user should only be able to create event for an account they own
      console.log('event-edit.ts loadAccounts HostAccountUUID = CurrentSession.AccountUUID. returning..');
      this.hasAccountList = false;
      this.event.HostAccountUUID = this.sessionService.CurrentSession.AccountUUID;
      return;
    }
   await accountThread.subscribe((response) => {
   //   this.loading = false;
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.messages.publish('api:err', data);
        return;
      }
      data.Result.forEach(account => {
        this.eventService.Accounts.push(account);
        this.accounts.push({ label: account.Name, value: account.UUID });
      });
      this.hasAccountList = true;
      console.log('event-edit.ts loadAccounts this.accounts:', this.accounts);
    });
  }


  onCboChangeCategory(event) {
    console.log('onCboChangeCategory:', event.value);
    this.event.Category = event.value;
  }

  onCboChangeHost(event) {

    console.log('onCboChangeHost:', event.value);
    this.event.HostAccountUUID = event.value;
  }

  async saveEvent() {

    console.log('event-edit.ts saveEvent this.eventStartDate:', this.eventStartDate);
    console.log('event-edit.ts saveEvent this.eventStartTime:', this.eventStartTime);
    console.log('event-edit.ts saveEvent this.eventEndDate:', this.eventEndDate);
    console.log('event-edit.ts saveEvent this.eventEndTime:', this.eventEndTime);
    this.event.StartDate = DateTimeFunctions.combine(this.eventStartDate, this.eventStartTime );
    this.event.EndDate = DateTimeFunctions.combine(this.eventEndDate, this.eventEndTime );
    console.log('event-edit.ts saveEvent this.event.StartDate:', this.event.StartDate);
    console.log('event-edit.ts saveEvent this.event.EndDate:', this.event.EndDate);

    this.event.Private = true; // todo defaults to private = true. if only admin/publisher etx can make false.

    console.log('event-edit.ts saveEvent:', this.event);

    let serviceThread;
    if (this.newEvent === true) {
      serviceThread = this.eventService.addEvent(this.event);
    } else {
      serviceThread = this.eventService.updateEvent(this.event);
    }
    await serviceThread.subscribe((response) => {
      //   this.loading = false;
         const data = response as ServiceResult;
         if (data.Code !== 200) {
           this.messages.publish('api:err', data);
  //         this.geoLocation.savingEvent = false;
           return;
         }
         this.eventUUID = data.Result.UUID;
         this.event = data.Result;
  //       this.geoLocation.location.EventUUID =  data.Result.UUID;
         this.saveEventLocation();
         this.newEvent = false;
     }, (err) => {
    //  this.geoLocation.savingEvent  = false;
      this.messages.publish('service:err', err);
      return;
    });
  }

  async saveEventLocation() {

    const eventLocation = new EventLocation();
    await this.eventService.saveEventLocation(this.geoLocation.location).subscribe((response) => {
    this.geoLocation.savingEvent = false;
        const data = response as ServiceResult;
        if (data.Code !== 200) {
          this.messages.publish('api:err', data);
          return;
        }
        this.geoLocation.location  = data.Result;
        if ( this.geoLocation.redirectAfterSave === true) {
          this.geoLocation.redirectAfterSave = false;
          this.router.navigateByUrl('/tabs/home');
          return;
        }
        this.newEvent = true;
        this.event.UUID = '';
        this.eventUUID = '';
        this.geoLocation.UUID = '';
        this.geoLocation.location.UUID = '';
        this.geoLocation.isNew = true;
    });
  }
}
