import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FitPaginatorBarComponent} from './fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './material-modules';



@NgModule({
  declarations: [
    FitPaginatorBarComponent
  ],
  imports: [
    CommonModule,
    ...materialModules
  ],
  exports:[
    FitPaginatorBarComponent
  ]
})
export class SharedModule { }
