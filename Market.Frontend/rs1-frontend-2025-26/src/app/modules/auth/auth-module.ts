import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';

import {AuthRoutingModule} from './auth-routing-module';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {ReactiveFormsModule} from '@angular/forms';
import {materialModules} from '../shared/material-modules';
import {LogoutComponent} from './logout/logout.component';
import {TranslatePipe} from '@ngx-translate/core';


@NgModule({
  declarations: [
    AuthLayoutComponent,
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    LogoutComponent
  ],
  imports: [
    CommonModule,
    AuthRoutingModule,
    ...materialModules,
    ReactiveFormsModule,
    TranslatePipe,
  ]
})
export class AuthModule { }
