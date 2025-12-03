import {Component, Input} from '@angular/core';
import {BasePagedComponent} from '../basePagedComponent';

@Component({
  selector: 'app-fit-paginator-bar',
  standalone: false,
  templateUrl: './fit-paginator-bar.component.html',
  styleUrl: './fit-paginator-bar.component.scss',
})
export class FitPaginatorBarComponent {
  // ViewModel je bilo koja komponenta koja nasljeđuje BasePagedComponent
  @Input({ required: true }) vm!: BasePagedComponent<any, any>;
}
