import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminRoutingModule } from './admin-routing-module';
import { ProductsComponent } from './catalogs/products/products.component';
import { ProductsAddComponent } from './catalogs/products/products-add/products-add.component';
import { ProductsEditComponent } from './catalogs/products/products-edit/products-edit.component';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';

import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { ProductCategoriesComponent } from './catalogs/product-categories/product-categories.component';
import {materialModules} from '../shared/material-modules';
import { ProductCategoryUpsertComponent } from './catalogs/product-categories/product-category-upsert/product-category-upsert.component';
import { AdminOrdersComponent } from './orders/admin-orders.component';
import {TranslatePipe} from '@ngx-translate/core';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';



@NgModule({
  declarations: [
    ProductsComponent,
    ProductsAddComponent,
    ProductsEditComponent,
    AdminLayoutComponent,
    ProductCategoriesComponent,
    ProductCategoryUpsertComponent,
    AdminOrdersComponent,
    AdminSettingsComponent,
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ReactiveFormsModule,
    ...materialModules,
    FormsModule,
    TranslatePipe,
  ]
})
export class AdminModule { }
