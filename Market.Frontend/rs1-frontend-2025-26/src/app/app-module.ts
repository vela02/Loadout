import { NgModule, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideAnimations} from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import {authInterceptor} from './core/services/auth/auth-interceptor.service';

@NgModule({
  declarations: [
    App
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [
    provideAnimations(),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection(),
    provideHttpClient(
      withInterceptors([authInterceptor]) // ✅ Novi pristup
    )
  ],
  bootstrap: [App]
})
export class AppModule { }
