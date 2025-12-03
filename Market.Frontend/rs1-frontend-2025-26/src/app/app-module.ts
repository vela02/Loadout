import { NgModule, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { provideAnimations} from '@angular/platform-browser/animations';
import {HttpClient, provideHttpClient, withInterceptors} from '@angular/common/http';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import {authInterceptor} from './core/services/auth/auth-interceptor.service';
import {TranslateLoader, TranslateModule} from '@ngx-translate/core';
import {CustomTranslateLoader} from './core/services/custom-translate-loader';
import { FitPaginatorBarComponent } from './core/components/fit-paginator-bar/fit-paginator-bar.component';
import {materialModules} from './modules/shared/material-modules';

@NgModule({
  declarations: [
    App,
    FitPaginatorBarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: (http: HttpClient) => new CustomTranslateLoader(http),
        deps: [HttpClient]
      }
    }),
    materialModules,
  ],
  providers: [
    provideAnimations(),
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection(),
    provideHttpClient(
      withInterceptors([authInterceptor])
    )
  ],
  exports: [
    FitPaginatorBarComponent
  ],
  bootstrap: [App]
})
export class AppModule { }
