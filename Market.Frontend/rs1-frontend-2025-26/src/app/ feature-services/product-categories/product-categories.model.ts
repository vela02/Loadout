import {BasePagedQuery} from '../../core/models/basePagedQuery';
import {PageResult} from '../../core/models/pageResult';

export class ListProductCategoriesRequest extends BasePagedQuery {
  search?: string | null;
  onlyEnabled?: boolean | null;
}

// Za sada komponenta bez paginga može komotno koristiti samo ovo:
export interface ProductCategoryListItem {
  id: number;
  name: string;
  isEnabled: boolean;
}

export type ListProductCategoriesResponse = PageResult<ProductCategoryListItem>;

// === COMMANDS (WRITE) ===
export interface UpsertProductCategoryCommand {
  name?: string | null;
}
