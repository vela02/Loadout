import {NgModule} from '@angular/core';

import {AdminRoutingModule} from './admin-routing-module';
import {ProductsComponent} from './catalogs/products/products.component';
import {ProductsAddComponent} from './catalogs/products/products-add/products-add.component';
import {ProductsEditComponent} from './catalogs/products/products-edit/products-edit.component';
import {AdminLayoutComponent} from './admin-layout/admin-layout.component';
import {ProductCategoriesComponent} from './catalogs/product-categories/product-categories.component';
import {
  ProductCategoryUpsertComponent
} from './catalogs/product-categories/product-category-upsert/product-category-upsert.component';
import {AdminOrdersComponent} from './orders/admin-orders.component';
import {AdminSettingsComponent} from './admin-settings/admin-settings.component';
import {SharedModule} from '../shared/shared-module';
import { OrderDetailsDialogComponent } from './orders/admin-orders-details-dialog/order-details-dialog.component';
import { ChangeStatusDialogComponent } from './orders/change-status-dialog/change-status-dialog.component';

import { CustomProductsAddComponent } from '../custom-ui/products/products-add/custom-products-add.component';
import { PromotionsComponent } from './catalogs/promotions/promotions.component';
import { AdminStatsComponent } from './stats/admin-stats.component';
import { AdminUsersComponent } from './users/admin-users.component';
import { AdminNewsComponent } from './news/admin-news.component';
import { AdminJobsComponent } from './jobs/admin-jobs.component';
import { PromotionCreateDialogComponent } from './catalogs/promotions/promotion-create-dialog/promotion-create-dialog.component';


@NgModule({
  declarations: [
    ProductsComponent,
    ProductsAddComponent,
    CustomProductsAddComponent,
    ProductsEditComponent,
    AdminLayoutComponent,
    ProductCategoriesComponent,
    ProductCategoryUpsertComponent,
    AdminOrdersComponent,
    AdminSettingsComponent,
    OrderDetailsDialogComponent,
    ChangeStatusDialogComponent,
    PromotionsComponent,
    PromotionCreateDialogComponent,
    AdminStatsComponent,
    AdminUsersComponent,
    AdminNewsComponent,
    AdminJobsComponent,
  ],
  imports: [
    AdminRoutingModule,
    SharedModule,
  ]
})
export class AdminModule { }
