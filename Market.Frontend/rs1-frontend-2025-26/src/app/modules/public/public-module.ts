import {NgModule} from '@angular/core';

import {PublicRoutingModule} from './public-routing-module';
import {PublicLayoutComponent} from './public-layout/public-layout.component';
import {SearchProductsComponent} from './search-products/search-products.component';
import {SharedModule} from '../shared/shared-module';
import {GamesCatalogComponent} from './games-catalog/games-catalog.component';
import {GameDetailsComponent} from './game-details/game-details.component';
import {NewsFeedComponent} from './news-feed/news-feed.component';
import {StoreHeaderComponent} from './store-header/store-header.component';


@NgModule({
  declarations: [
    PublicLayoutComponent,
    SearchProductsComponent,
    GamesCatalogComponent,
    GameDetailsComponent,
    NewsFeedComponent,
    StoreHeaderComponent
  ],
  imports: [
    SharedModule,
    PublicRoutingModule,
  ]
})
export class PublicModule { }
