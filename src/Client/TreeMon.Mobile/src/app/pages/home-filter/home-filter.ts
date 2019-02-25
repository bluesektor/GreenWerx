import { AfterViewInit, Component, ViewEncapsulation } from '@angular/core';
import { ModalController } from '@ionic/angular';

import { EventService } from '../../services/events/event.service';
import { ServiceResult} from '../../models/serviceresult';
import { Events } from '@ionic/angular';
import { ObjectFunctions } from '../../common/object.functions';
import { SessionService} from '../../services/session.service';
import { AccountService} from '../../services/user/account.service';
import { Filter, Screen } from '../../models/index';
// todo refactor the screens and filters in the services to be in a singular class.
@Component({
  selector: 'page-home-filter',
  templateUrl: 'home-filter.html',
  styleUrls: ['./home-filter.scss'],
  encapsulation: ViewEncapsulation.None
})
export class HomeFilterPage implements AfterViewInit {

  loading = false;

  constructor(
    public modalCtrl: ModalController,
    public eventService: EventService,
    private sessionService: SessionService,
    private accountService: AccountService,
    public messages: Events  ) {   }

    public type: string;

  ionViewDidEnter() {
    console.log('home-filter.ts ionViewDidEnter type:',  this.type);
    switch ( this.type) {
        case 'event':
            this.loadEventCategories();
        break;
        case 'host':
            this.loadAccountCategories();
        break;
        default:

        break;
      }
  }

  initializeView() {
    console.log('home-filter.ts initializeView');

    switch ( this.type) {
        case 'event':
            this.eventService.EventFilter.Screens.forEach(screen => {    // loop through selected screens
            const idx = this.eventService.AvailableScreens.findIndex( asc => asc.Caption === screen.Caption );
            if (idx < 0 ) { return; }
            // find the selected in the available and set the Value field in available to selected
               this.eventService.AvailableScreens[idx].Value =   screen.Value;
           });
        break;
        case 'host':
        this.accountService.AccountFilter.Screens.forEach(screen => {    // loop through selected screens
            const idx = this.accountService.AvailableScreens.findIndex( asc => asc.Caption === screen.Caption );
            if (idx < 0 ) { return; }
            // find the selected in the available and set the Value field in available to selected
               this.accountService.AvailableScreens[idx].Value =   screen.Value;
           });
        break;
        default:

        break;
      }

  }

  isInEventScreens(name: string): boolean {
    for (let i = 0; i < this.eventService.EventFilter.Screens.length; i++ ) {
      if (this.eventService.EventFilter.Screens[i].Field === name) {
        return true;
      }
    }
    return false;
  }

  isInAccountScreens(name: string): boolean {
    for (let i = 0; i < this.accountService.AccountFilter.Screens.length; i++ ) {
      if (this.accountService.AccountFilter.Screens[i].Field === name) {
        return true;
      }
    }
    return false;
  }

  ngAfterViewInit() { }

  resetFilters() {
    switch ( this.type) {
        case 'event':
            // reset all of the toggles to be checked
            this.eventService.AvailableScreens.forEach(prop => {
                // prop.Value = 'false';
                prop.Selected = false;
            });
            this.eventService.EventFilter.Screens = [];
            this.eventService.EventFilter.IncludeDeleted = false;
            this.eventService.EventFilter.IncludePrivate = false;
            // this.eventService.EventFilter.TimeZone = '';
        break;
        case 'host':
            // reset all of the toggles to be checked
            this.accountService.AvailableScreens.forEach(prop => {
                // prop.Value = 'false';
                prop.Selected = false;
            });
            this.accountService.AccountFilter.Screens = [];
            this.accountService.AccountFilter.IncludeDeleted = false;
            this.accountService.AccountFilter.IncludePrivate = false;
            // this.accountService.accountFilter.TimeZone = '';
        break;
        default:

        break;
      }

  }

  applyFilters() {
    switch ( this.type) {
        case 'event':
            this.eventService.EventFilter.StartIndex = 0;
            this.eventService.EventFilter.Screens = this.eventService.AvailableScreens.filter( c => c.Selected === true).map(function(c) {
            console.log('home-filter.ts applyFilters this.eventService.AvailableScreens.filter:', c);
            return c;
            });
            console.log('home-filter.ts applyFilters this.eventService.EventScreens:', this.eventService.EventFilter.Screens);
            // Pass back filter
            this.dismiss(this.eventService.EventFilter);
        break;
        case 'host':
            this.accountService.AccountFilter.StartIndex = 0;
            this.accountService.AccountFilter.Screens =
                this.accountService.AvailableScreens.filter( c => c.Selected === true).map(function(c) {
                console.log('home-filter.ts applyFilters this.accountService.AvailableScreens.filter:', c);
                return c;
            });
            console.log('home-filter.ts applyFilters this.accountService.accountScreens:', this.accountService.AccountFilter.Screens);
            // Pass back filter
            this.dismiss(this.accountService.AccountFilter);
        break;
        default:

        break;
      }
  }

  dismiss(filter?: any) {
    // using the injected ModalController this page
    // can "dismiss" itself and pass back data
    this.modalCtrl.dismiss(filter);
  }

  isUserInrole(roleName: string): boolean {
    return this.sessionService.isUserInRole(roleName);
  }

  async loadEventCategories() {
  // NOTE: Name must match the property name
      // because we're going to create a filter based on the name
      // todo if there are selected screens (EventScreens)
      // then initialize the view to toggle them on..
      //
      if (this.eventService.EventFilter.Screens !== undefined && this.eventService.EventFilter.Screens.length > 0) {
        console.log('home-filter.ts cached screens  this.eventService.AvailableScreens:',  this.eventService.AvailableScreens);

        this.initializeView();
      } else {
        this.loading = true;
        await this.eventService.getEventCategories().subscribe((response) => {
          this.loading = false;
          const data = response as ServiceResult;
          if (data.Code !== 200) {
            this.messages.publish('api:err', data);
            return;
          }
          this.eventService.AvailableScreens = [];
          this.eventService.Categories = [];
          if (ObjectFunctions.isValid(data.Result) === true) {
            data.Result.forEach(name => {
              if (!name) {
                return;
              }

               // Category matches to these screen fields
              const screen = new Screen();
              screen.Command = 'SEARCHBY';
              screen.Field = 'CATEGORY';
              screen.Selected = false;
              screen.Caption = name;
               screen.Value = name; // 'false';
              screen.Type = 'category';
              this.eventService.AvailableScreens.push(screen);
              this.eventService.Categories.push(name); });
          }
        });
      }
  }

  loadAccountCategories() {
  // NOTE: Name must match the property name
      // because we're going to create a filter based on the name
      // todo if there are selected screens (EventScreens)
      // then initialize the view to toggle them on..
      //
      if (this.accountService.AccountFilter.Screens !== undefined && this.accountService.AccountFilter.Screens.length > 0) {
        console.log('home-filter.ts cached screens  this.accountService.AvailableScreens:',  this.accountService.AvailableScreens);

        this.initializeView();
      } else {
        this.loading = true;
        this.accountService.getAccountCategories().subscribe((response) => {
          this.loading = false;
          const data = response as ServiceResult;
          if (data.Code !== 200) {
            this.messages.publish('api:err', data);
            return;
          }
          this.accountService.AvailableScreens = [];
          this.accountService.Categories = [];
          if (ObjectFunctions.isValid(data.Result) === true) {
            data.Result.forEach(name => {
              if (!name) {
                return;
              }

               // Category matches to these screen fields
              const screen = new Screen();
              screen.Command = 'SEARCHBY';
              screen.Field = 'CATEGORY';
              screen.Selected = false;
              screen.Caption = name;
               screen.Value = name; // 'false';
              screen.Type = 'category';
              this.accountService.AvailableScreens.push(screen);
              this.accountService.Categories.push(name); });
          }
        });
      }
    }

}
