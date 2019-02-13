import { Component } from '@angular/core';

import { PopoverController } from '@ionic/angular';


@Component({
  template: `
    <ion-list>
      <ion-item button (click)="close('https://github.com/bluesektor/TreeMon')">
        <ion-label>GitHub Repo</ion-label>
      </ion-item>
    </ion-list>
  `
})
export class PopoverPage {
  constructor(public popoverCtrl: PopoverController) {}

  close(url: string) {
    window.open(url, '_blank');
    this.popoverCtrl.dismiss();
  }
}
