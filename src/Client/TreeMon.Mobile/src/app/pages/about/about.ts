import { Component, ViewEncapsulation } from '@angular/core';
import { PopoverController } from '@ionic/angular';
import { PopoverPage } from '../about-popover/about-popover';
import { AppSetting} from '../../app.settings';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'page-about',
  templateUrl: 'about.html',
  styleUrls: ['./about.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AboutPage {

  constructor(
    public popoverCtrl: PopoverController,
    public appSettings: AppSetting) {
      console.log('about.ts this.appSettings:', this.appSettings);
     }

  async presentPopover(event: Event) {
    const popover = await this.popoverCtrl.create({
      component: PopoverPage,
      event
    });
    await popover.present();
  }
}
