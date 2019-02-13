import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/primeng';

 import { GraphsComponent } from './graphs.component';


@NgModule({
    imports: [
        CommonModule,
        ChartModule
  ],
    declarations: [
        GraphsComponent
    ],
    exports: [
        GraphsComponent
    ]
})
export class GraphsModule {
}
