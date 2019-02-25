import { Component, OnInit, ViewChild,  ViewEncapsulation } from '@angular/core';
import { AlertController, LoadingController, ModalController } from '@ionic/angular';
import { Refresher } from 'ionic-angular';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/events/event.service';
import { AccountService} from '../../services/user/account.service';
import { ServiceResult} from '../../models/serviceresult';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Events} from '@ionic/angular';
import {LocalSettings } from '../../services/settings/local.settings';
import { HomeFilterPage } from '../home-filter/home-filter';
import {ObjectFunctions} from '../../common/object.functions';
import * as moment from 'moment';
import * as momentTimezone from 'moment-timezone';
import { Filter, Screen, Event, EventGroup } from '../../models/index';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  encapsulation: ViewEncapsulation.None
})
export class HomePage implements OnInit {

  filter: Filter = new Filter();
  listItems: any[] = [];
  loadingControl: any;

  @ViewChild('refresherRef') refresherRef: Refresher;
  @ViewChild('ctlList') listControl: any;

  itemCount = 0; // total # of items in query.
  currentIndex = 0;
  pageSize = 25; // number of items to load
  loadMoreIndex = 75; // initially load 100 records (if we have them)
                              // then adjust accordingly.
                              // If the total items is less than 100 then we
                              // won't need to use the loadMoreIndex (all available items loaded).
  viewPortLoaded = false;
  indexesLoaded: number[] = [];
  queryText = '';
  segment = 'all';
  fabIcon = 'calendar';
  viewType = 'event';
  // distance = 25;


  constructor(
    public alertCtrl: AlertController,
    public accountService: AccountService,
    public eventService: EventService,
    public loadingController: LoadingController,
    public sessionService: SessionService,
    public router: Router,
    public route: ActivatedRoute,
    public modalCtrl: ModalController,
    public messages: Events,
    private localSettings: LocalSettings
    ) { }

  ngOnInit() {
    let tab = document.getElementById('lblHome');
    console.log('home.page.ts loadTypes tab:', tab); // update tab label
    tab.innerHTML = 'Events';
    let icon =  document.getElementById('icoHome');
    icon.setAttribute('name', this.fabIcon);
    // Update the app.component view..
    tab = document.getElementById('lblCompHome'); // update component side bar
    tab.innerHTML = 'Events';
    icon =  document.getElementById('icoCompHome');
    icon.setAttribute('name', this.fabIcon);

     // Screen by clients time and timezone.
    // If no timezone all event will be returned.
    this.filter.TimeZone =  momentTimezone.tz.guess();

    this.localSettings.getValue(LocalSettings.ViewType, 'event').then(res => {
      console.log('home.page.ts localSettings res:', res);
      this.viewType = res;
      this.updateList();
    });

  }

   deleteItem(slidingItem: HTMLIonItemSlidingElement, item: any, title: string) {
     console.log('home.page.ts deleteItem');
    /*
    const alert = await this.alertCtrl.create({
      header: title,
      message: 'Would you like to remove this session from your favorites?',
      buttons: [
        {
          text: 'Cancel',
          handler: () => {
            // they clicked the cancel button, do not remove the session
            // close the sliding item and hide the option buttons
            slidingItem.close();
          }
        },
        {
          text: 'Remove',
          handler: () => {

            if ( ObjectFunctions.isValid( this.eventService.Dashboard  ) === true ) {
              for (let i = 0; i < this.eventService.Dashboard.Events.length; i++) {
                console.log('home.page.ts remove favorite searching x:', this.eventService.Dashboard.Events[i]);
                if ( this.eventService.Dashboard.Events[i].UUID === item.UUID) {
                  this.eventService.Dashboard.Events.splice( i, 1);

                  break;
                }
              }
            }
            */


            // close the sliding item
            slidingItem.close();
            /*
            this.eventService.deleteEvent(item.UUID).subscribe((response) => {
              const data = response as ServiceResult;
              if (data.Code !== 200) {
                this.messages.publish('api:err', data);
                return;
              }
              const filter = new Filter();
              this.updateEvents(filter, true );
              this.messages.publish('api:ok', 'Event Removed');
            }, (err) => {
              this.messages.publish('service:err', err);
           });

          }
        }
      ]
    });
    // now present the alert on top of all other content
    await alert.present();
    */
  }

  async updateList() {
     // Close any open sliding items when the listItems updates
     if (this.listControl) {
      this.listControl.closeSlidingItems();
    }
    console.log('homepage.TS updateEvents  ');
    this.loadingControl = await this.loadingController.create({
      message: 'Loading...'
    });
    await this.loadingControl.present();
     // this.listItems = [];

    switch ( this.viewType) {
      case 'event':
        if (this.segment === 'favorites') {
          await this.loadFavorites();
          return;
        } else {
          await this.loadEvents();
        }
      break;
      case 'host':
        if (this.segment === 'favorites') {
          await this.loadFavorites();
          return;
        } else {
          await this.loadHosts();
        }
      break;
      default:
        this.loadingControl.dismiss();
        if ( ObjectFunctions.isValid(this.refresherRef) === true) {
          this.refresherRef.complete();
        }
      break;
    }
  }

  getCategoryIcon(category: string): string {
    if (ObjectFunctions.isNullOrWhitespace(category) === true) {
      return category;
    }
   // console.log('home.page.ts getCategoryIcon category:', category);
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

  async searchEvents() {

    if (!this.queryText || this.queryText.length < 2) {
      return;
    }
     console.log('home.page.ts searchEvents  this.queryText:',  this.queryText);
      this.filter = new Filter();
      const screen = new Screen();
      screen.Field = 'NAME';
      screen.Command = 'SearchBy';
      screen.Operator = 'CONTAINS';
      screen.Value = this.queryText;
      this.filter.Screens.push(screen);

      const screenDescription = new Screen();
      screenDescription.Field = 'BODY';
      screenDescription.Command = 'SearchBy';
      screenDescription.Operator = 'CONTAINS';
      screenDescription.Value = this.queryText;
      this.filter.Screens.push(screenDescription);
      await this.updateList();
  }

  segmentClicked() {
    console.log('this.segment:', this.segment);
    this.resetParameters();
    this.updateList();

  }

  async presentFilter() {

    const modal = await this.modalCtrl.create({
      component: HomeFilterPage,
      componentProps: {
        type: this.viewType
      }
    });
    await modal.present();
    console.log('home.page.ts filter before onWillDismiss');
    const { data } = await modal.onWillDismiss();
    this.resetParameters();
    const eventFilter = data;
    console.log('home.page.ts filter after onWillDismiss eventFilter:', eventFilter);
    this.filter = eventFilter;
    if (this.filter === undefined) {
      this.filter = new Filter();
    }

    this.filter.TimeZone = momentTimezone.tz.guess();
    this.updateList();
  }

  async loadEvents() {
    await this.eventService.getEvents(this.filter)
      .subscribe(response => {

      this.loadingControl.dismiss();
      if (ObjectFunctions.isValid(this.refresherRef) === true) {
        this.refresherRef.complete();
      }

      console.log('home.page.ts updateEvents   response:', response);
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.messages.publish('api:err', data);
        return;
      }
     for (let i = 0; i < data.Result.length; i++) {
        this.listItems.push(data.Result[i]);
     }
      this.itemCount = data.TotalRecordCount;
      console.log('homepage.TS updateEvents   this.listItems:', this.listItems);

      this.indexesLoaded.push( this.loadMoreIndex );
      const pctFromTop = this.listItems.length - Math.round( data.Result.length * .25 );
      this.loadMoreIndex = pctFromTop;
      if (this.loadMoreIndex < 1 ) {
        this.loadMoreIndex =  this.listItems.length - 1;
      }
      console.log('listItems.ts updateEvents segment:', this.segment);

  }, (err) => {
    console.log('homepage.TS updateEvents  err:', err);
    this.loadingControl.dismiss();
    if ( ObjectFunctions.isValid(this.refresherRef) === true) {
      this.refresherRef.complete();
    }
      this.messages.publish('service:err', err);
    });
  }

  async loadFavorites() {
    console.log('home.page.ts onLogin loadFavorites :');
    this.listItems = null;
    this.listItems = [];

    switch ( this.viewType) {
      case 'event':
          await this.eventService.getFavorites(null).subscribe(sessionResponse => {
            this.loadingControl.dismiss();
            if (ObjectFunctions.isValid(this.refresherRef) === true) {
              this.refresherRef.complete();
            }
            const data = sessionResponse as ServiceResult;
            console.log('home.page.ts onLogin loadFavorites.data :', data);
            if (data.Code !== 200) {
              this.messages.publish('api:err', data);
                return false;
            }
            this.itemCount = data.TotalRecordCount;
            this.eventService.Favorites =  data.Result;
            console.log('home.page.ts loadFavorites this.eventService.Favorites:' , this.eventService.Favorites);
            for (let j = 0; j < this.eventService.Favorites.length; j++) {
              if (
                this.eventService.Favorites[j] === undefined
                || this.eventService.Favorites[j].Event === null
                || this.eventService.Favorites[j].Event === undefined
                ) {
                continue;
              }
              const evt =   this.eventService.Favorites[j].Event;
              evt.FavoritedByAccountUUID =  this.eventService.Favorites[j].AccountUUID;
              evt.FavoritedByUserUUID =   this.eventService.Favorites[j].CreatedBy;
              this.listItems.push(evt);
          }
            console.log('home.page.ts loadFavorites this.events:' , this.listItems);
        }, (err) => {
          this.loadingControl.dismiss();
          this.messages.publish('service:err', err);
        });
      break;
      case 'host':
          await this.accountService.getFavorites(null).subscribe(sessionResponse => {
            this.loadingControl.dismiss();
            if (ObjectFunctions.isValid(this.refresherRef) === true) {
              this.refresherRef.complete();
            }
            const data = sessionResponse as ServiceResult;
            console.log('home.page.ts onLogin loadFavorites.data :', data);
            if (data.Code !== 200) {
              this.messages.publish('api:err', data);
                return false;
            }
            this.itemCount = data.TotalRecordCount;
            this.accountService.Favorites =  data.Result;
            console.log('home.page.ts updateaccounts this.accountService.Favorites:' , this.accountService.Favorites);
            for (let i = 0; i < this.accountService.Favorites.length; i++) {
              console.log('i:', i);
              if (this.accountService.Favorites[i] === undefined
                || this.accountService.Favorites[i].Account === undefined
                ) {
                continue;
              }
              const evt =   this.accountService.Favorites[i].Account;
              evt.FavoritedByAccountUUID = this.accountService.Favorites[i].AccountUUID;
              evt.FavoritedByUserUUID = this.accountService.Favorites[i].CreatedBy;
              this.listItems.push(evt);
          }
            console.log('home.page.ts updateaccounts this.accounts:' , this.listItems);
        }, (err) => {
          this.loadingControl.dismiss();
          this.messages.publish('service:err', err);
        });
      break;
      default:
      break;
    }
  }

  goToItemDetail(item: any) {
    console.log('goToItemDetail item:', item);
    this.router.navigateByUrl(`tabs/details/${item.UUID}/${item.UUIDType}`); //
  }

  editItem(slidingItem: HTMLIonItemSlidingElement, item: any) {
    console.log('editEvent item:', item);
    this.router.navigateByUrl(`tabs/edit/${item.UUID}`);  // cant load angular/core
    // this.router.navigateByUrl(`./tabs/edit/${item.UUID}`); //can't match route
  }

  async addFavoriteItem(slidingItem: HTMLIonItemSlidingElement, item: any) {
    switch ( this.viewType) {
      case 'event':
          console.log('home.page.ts addFavoriteEvent item:', item);
          slidingItem.close(); // close the sliding item
          const svc = this.eventService.addFavorite(item.UUID);
          if (svc === null) {
            return;
          }
          await svc.subscribe((response) => {
            const data = response as ServiceResult;
            if ( data === null || data.hasOwnProperty('Code') === false ) {
              console.log('already added.');
              return;
            }
            if (data.Code !== 200) {
              this.messages.publish('api:err', data);
              return;
            }
            this.messages.publish('api:ok', 'Favorite Added');
          }, (err) => {
            this.messages.publish('service:err', err);
        });
      break;
      case 'host':
          console.log('home.page.ts addFavoriteAccount item:', item);
          slidingItem.close(); // close the sliding item
          const acctSvc = this.accountService.addFavorite(item.UUID);
          if (acctSvc === null) {
            return;
          }
          await acctSvc.subscribe((response) => {
            const data = response as ServiceResult;
            if ( data === null || data.hasOwnProperty('Code') === false ) {
              console.log('already added.');
              return;
            }
            if (data.Code !== 200) {
              this.messages.publish('api:err', data);
              return;
            }
            this.messages.publish('api:ok', 'Favorite Added');
          }, (err) => {
            this.messages.publish('service:err', err);
        });
      break;
      default:
      break;
    }
  }

  async removeFavoriteItem(slidingItem: HTMLIonItemSlidingElement, item: any, title: string) {

    const alert = await this.alertCtrl.create({
      header: title,
      message: 'Would you like to remove this item from your favorites?',
      buttons: [
        {
          text: 'Cancel',
          handler: () => {
            // they clicked the cancel button, do not remove the item
            // close the sliding item and hide the option buttons
            slidingItem.close();
          }
        },
        {
          text: 'Remove',
          handler: () => {
                // close the sliding item
                slidingItem.close();
                this.eventService.removeFavorite(item.UUID).subscribe((response) => {
                  const data = response as ServiceResult;
                  if (data.Code !== 200) {
                    this.messages.publish('api:err', data);
                    return;
                  }

                  this.updateList();
                  this.messages.publish('api:ok', 'Favorite Removed');
                }, (err) => {
                  this.messages.publish('service:err', err, '');
              });
          }
        }
      ]
    });
    // now present the alert on top of all other content
    await alert.present();
  }

  doRefresh(event: Refresher) {
    this.updateList();
  }

  loadMoreInViewPort(event, itemIndex: any) {

    console.log('home.page.ts loadMoreInViewPort   this.viewPortLoaded:' , this.viewPortLoaded   );

    if ( this.viewPortLoaded === false ) {
        this.viewPortLoaded = true;
        return;
    }

    console.log('home.page.ts loadMoreInViewPort  event:', event   );

    if (  event.visible === false) { return; }

    console.log('home.page.ts loadMoreInViewPort   this.currentIndex:' , this.currentIndex   );
    console.log('home.page.ts loadMoreInViewPort   this.loadMoreIndex:' , this.loadMoreIndex   );


    if (this.currentIndex === this.loadMoreIndex) {
      return;
    }
    console.log('home.page.ts loadMoreInViewPort   itemIndex:' , itemIndex   );

    if (itemIndex === this.loadMoreIndex) {

        console.log('home.page.ts loadMoreInViewPort   this.indexesLoaded:' , this.indexesLoaded   );
        console.log('home.page.ts loadMoreInViewPort   this.indexesLoaded.indexOf(itemIndex):' ,
        this.indexesLoaded.indexOf(itemIndex)  );

        // make an array of loaded indexes. if item index is in the array we don't need to loadmore
        if (this.indexesLoaded.length > 0 && this.indexesLoaded.indexOf(itemIndex) > 0) {
            return;
        }
        this.currentIndex = itemIndex;

        if (this.filter === undefined) {
          this.filter = new Filter();
        }
        this.filter.PageSize = this.pageSize;
        this.filter.StartIndex = this.listItems.length;
        this.filter.PageResults = true;
        this.updateList();
      }
  }

  toggleList(fabButton: HTMLIonFabButtonElement, fabList: HTMLIonFabListElement) {
    fabButton.activated = !fabButton.activated;
    fabList.activated = !fabList.activated;
  }

  loadTypes(type: string, fab: HTMLIonFabElement) {
    let title = 'Events';

    fab.close();
    switch ( type ) {
      case 'event':
        this.fabIcon = 'calendar';
        break;
      case 'host':
        title = 'Hosts';
        this.fabIcon = 'contacts';
        break;
    }
    this.viewType = type;
    this.resetParameters();
    this.updateList();

    let tab = document.getElementById('lblHome');
    console.log('home.page.ts loadTypes tab:', tab); // update tab label
    tab.innerHTML = title;
    let icon =  document.getElementById('icoHome');
    icon.setAttribute('name', this.fabIcon);
    // Update the app.component view..
    tab = document.getElementById('lblCompHome'); // update component side bar
    tab.innerHTML = title;
    icon =  document.getElementById('icoCompHome');
    icon.setAttribute('name', this.fabIcon);

  }

  loadHosts( ) {
    console.log('host-lists.ts loadHosts ');

    this.accountService.getAllAccounts(this.filter).subscribe((response) => {
        if (ObjectFunctions.isValid(this.refresherRef) === true) {
          this.refresherRef.complete();
        }
        this.loadingControl.dismiss();
        const data = response as ServiceResult;
        if (data.Code !== 200) {
          this.messages.publish('api:err', data);
          return;
        }
        this.eventService.Accounts = [];

        for (let j = 0; j < data.Result.length; j++ ) {
          if ( data.Result[j].Image === '' || data.Result[j].Image === null ) { // ImageThumb
            data.Result[j].Image = '/assets/img/blankprofile.png';
          }
          this.listItems.push(data.Result[j]);
        }
        this.indexesLoaded.push( this.loadMoreIndex );
        const pctFromTop = this.listItems.length - Math.round( data.Result.length * .25 );
        this.loadMoreIndex = pctFromTop;
        if (this.loadMoreIndex < 1 ) {
          this.loadMoreIndex =  this.listItems.length - 1;
        }
        console.log('host-list.ts this.accountService.getAllAccounts pctFromTop:', pctFromTop);
        console.log('host-list.ts this.accountService.getAllAccounts this.loadMoreIndex:', this.loadMoreIndex);
        console.log('host-list.ts this.accountService.getAllAccounts  this.listItems:', this.listItems);
        this.itemCount = data.TotalRecordCount;
        console.log('host-list.ts this.accountService.getAllAccounts  this.itemCount:', this.itemCount);
      }, (err) => {
        this.loadingControl.dismiss();
        if ( ObjectFunctions.isValid(this.refresherRef) === true) {
         this.refresherRef.complete();
        }
         this.messages.publish('service:err', err);
      });


  }

  resetParameters() {
    this.currentIndex = 0;
    this.indexesLoaded = [];
    this.listItems  = [];
    this.itemCount = 0;
    this.filter = new Filter();
    this.filter.PageSize = this.pageSize;
    this.filter.StartIndex = 0;
    this.filter.PageResults = true;
  }
}
