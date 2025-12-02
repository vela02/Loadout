import { BasePagedRequest } from '../models/basePagedRequest';
import { PageResult } from '../models/pageResult';
import {BaseListComponent} from './baseListComponent';

export abstract class BasePagedComponent<TItem, TRequest extends BasePagedRequest>
  extends BaseListComponent<TItem> {

  request!: TRequest;
  totalItems = 0;
  totalPages = 0;

  protected abstract loadPagedData(): void;

  protected override loadData(): void {
    this.loadPagedData();
  }

  protected handlePageResult(result: PageResult<TItem>) {
    this.items = result.items;
    this.totalItems = result.totalItems;
    this.totalPages = result.totalPages;
  }

  goToPage(page: number): void {
    if (page < 1 || (this.totalPages && page > this.totalPages)) return;
    this.request.page = page;
    this.loadPagedData();
  }

  nextPage() { this.goToPage(this.request.page + 1); }
  prevPage() { this.goToPage(this.request.page - 1); }

  changePageSize(size: number) {
    this.request.pageSize = size;
    this.request.page = 1;
    this.loadPagedData();
  }
}
