import {NgModule} from '@angular/core';

import {ClientRoutingModule} from './client-routing-module';
import {SharedModule} from '../shared/shared-module';
import {ClientLayoutComponent} from './client-layout/client-layout.component';
import {AccountOverviewComponent} from './account/account-overview.component';
import {ChangePasswordComponent} from './change-password/change-password.component';
import {MyOrdersComponent} from './orders/my-orders.component';


@NgModule({
  declarations: [
    ClientLayoutComponent,
    AccountOverviewComponent,
    ChangePasswordComponent,
    MyOrdersComponent
  ],
  imports: [
    SharedModule,
    ClientRoutingModule
  ]
})
export class ClientModule { }
