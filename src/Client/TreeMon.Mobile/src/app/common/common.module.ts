import { NgModule } from '@angular/core';
 import { IonicModule } from 'ionic-angular';
import { EscapeHtmlPipe} from '../common/pipes/keep-html.pipe';



@NgModule({
  declarations: [EscapeHtmlPipe],
  exports: [EscapeHtmlPipe]
})
export class CommonModule { }
