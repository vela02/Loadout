import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PublicRoutingModule } from './public-routing-module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SearchProductsComponent } from './search-products/search-products.component';
import {materialModules} from '../shared/material-modules';


@NgModule({
  declarations: [
    PublicLayoutComponent,
    SearchProductsComponent
  ],
  imports: [
    CommonModule,
    PublicRoutingModule,
    ...materialModules,
  ]
})
export class PublicModule { }
