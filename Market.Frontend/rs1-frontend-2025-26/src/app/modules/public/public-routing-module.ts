import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SearchProductsComponent } from './search-products/search-products.component';
import { GamesCatalogComponent } from './games-catalog/games-catalog.component';
import { GameDetailsComponent } from './game-details/game-details.component';

const routes: Routes = [
  // Katalog i detalji igre (moraju biti PRIJE '' rute s '**' redirectom)
  { path: 'games', component: GamesCatalogComponent },
  { path: 'games/:id', component: GameDetailsComponent }, // :id = parametar, npr. /games/5
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
