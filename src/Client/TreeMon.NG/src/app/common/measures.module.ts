
import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PickListModule } from 'primeng/primeng';
import { DataTableModule, SharedModule, DialogModule, CheckboxModule  } from 'primeng/primeng';
import { MessageBoxesModule } from './messageboxes.module';
import { MeasuresComponent} from './measures.component';


@NgModule({
    imports: [
        FormsModule,
        CommonModule,
        MessageBoxesModule,
        DataTableModule,
        SharedModule,
        DialogModule,
        CheckboxModule
    ],
    declarations: [
        MeasuresComponent
    ],
    exports: [
        MeasuresComponent
    ]
})
export class MeasuresModule {

}
