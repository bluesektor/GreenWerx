import { AfterViewInit, Component, Input, ViewEncapsulation, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { AlertController, ModalController } from '@ionic/angular';
import { SessionService } from '../../services/session.service';
import {InventoryService } from '../../services/store/inventory.service';
import {UserService} from '../../services/user/user.service';
import {FileEx, ServiceResult, User} from '../../models/index';
import { Events } from '@ionic/angular';
import { ObjectFunctions} from '../../common/object.functions';
import {ChangePasswordPage} from '../password/change-password';

@Component({
  selector: 'page-account',
  templateUrl: 'account.html',
  styleUrls: ['./account.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [InventoryService, UserService]
})
export class AccountPage implements AfterViewInit, OnInit {
  username: string;
  images:  Array<FileEx> = [];
  @Input() maxFileUpload = 1;
  processingRequest  = false;
  picsReadOnly = false;
  user: User = new User();

  constructor(
    public alertCtrl: AlertController,
    public router: Router,
    private session: SessionService,
    private inventoryService: InventoryService,
    private userService: UserService,
    public messages: Events,
    public modalCtrl: ModalController ) {
      this.user.Image = '../../assets/img/blankprofile.png';
    }

  ngAfterViewInit() { }

  ngOnInit() {
    this.loadUser();
  }

  loadUser() {
    if (ObjectFunctions.isValid(this.session.CurrentSession) === false || this.session.CurrentSession.UserUUID === '') {
      this.messages.publish('console:log', 'err', 'ACCOUNT.ts loadUser this.session.CurrentSession.UserUUID not set.');
      return;
    }

    console.log('ACCOUNT.ts loadUser this.session.CurrentSession.UserUUID:', this.session.CurrentSession.UserUUID);
    this.userService.getUser(this.session.CurrentSession.UserUUID).subscribe((response) => {
      console.log('ACCOUNT.ts loadUser response:', response);
      const data = response as ServiceResult;
      if (data.Code !== 200) {
        this.messages.publish('api:err', data);
        return;
      }
      this.user = data.Result;
    });

  }

  uploadImageEvent(imageEvent, index) {
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

      const formData = new FormData();
      console.log('appending form data:', file);

      if (this.picsReadOnly === true) {
          console.log('gallery is read only, uploads are not allowed.');
          return;
      }
      // if it's new or the default image is empty then set the first image to the default image.
       // todo put a button on gallery to let user select default image
       //
      if (   this.isDefaultSet() === false ) {
          formData.append('defaultImage', file);
          console.log('setting defaultImage');

      } else {

          formData.append('settingImage', file);
          console.log('setting settingImage');
      }

    const res = this.inventoryService.uploadFormEx(formData, this.session.CurrentSession.UserUUID, 'User' );
    res.subscribe(data => {
      const response = data as ServiceResult;
      this.processingRequest = false;

        if (response.Code !== 200) {
          this.messages.publish('service:err', response.Message);
            return false;
        }
        console.log('image upload response:',  response.Result);
        this.images.push(response.Result);
        this.user.Image = response.Result.ImageThumb;
      }, err => {
        this.processingRequest = false;
        this.messages.publish('service:err', err.statusText, 4);

        if (err.status === 401) {
           // setTimeout(() => {    this.navCtrl.push(LoginPage);}, 3000);
        }
    });
}

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

  async changePassword() {
    console.log('Clicked to change password');

      const modal = await this.modalCtrl.create({
        component: ChangePasswordPage
      });
      await modal.present();

      const { data } = await modal.onWillDismiss();
  }

  logout() {
    console.log('account.ts logout');
    this.session.logOut().then(() => {
      this.router.navigateByUrl('/login');
    });
  }

  support() {
    this.router.navigateByUrl('/support');
  }
}
