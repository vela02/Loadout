import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SearchProductsComponent } from './search-products/search-products.component';
import { GamesCatalogComponent } from './games-catalog/games-catalog.component';
import { GameDetailsComponent } from './game-details/game-details.component';
import { CheckoutComponent } from './checkout/checkout.component';
import { myAuthData, myAuthGuard } from '../../core/guards/my-auth-guard';

const routes: Routes = [
  // Katalog i detalji igre (moraju biti PRIJE '' rute s '**' redirectom)
  { path: 'games', component: GamesCatalogComponent },
  { path: 'games/:id', component: GameDetailsComponent }, // :id = parametar, npr. /games/5
  // Korpa + checkout wizard: samo za prijavljene korisnike
  {
    path: 'cart',
    component: CheckoutComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true }),
  },
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        component: SearchProductsComponent
      },
      // kasnije može i ovako:
      // { path: 'about', component: AboutComponent },
      // { path: 'contact', component: ContactComponent },

      { path: '**', redirectTo: '' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule {}
