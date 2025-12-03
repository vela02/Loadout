import { PageResult } from '../../core/models/pageResult';
import {BasePagedQuery} from '../../core/models/basePagedQuery';

// === DTO ===
export interface ProductListItem {
  id: number;
  name: string;
  isEnabled: boolean;
  description?: string | null;
  price: number;
  stockQuantity: number;
  categoryName: string;
}

// === QUERY (READ) ===
export class ListProductsRequest extends BasePagedQuery {
  // future filters: categoryId?, isEnabled?, priceMin?, priceMax?
  search?: string | null;
}

// === RESPONSE ===
export type ListProductsResponse = PageResult<ProductListItem>;

// === COMMANDS (WRITE) ===
export interface CreateProductCommand {
  name?: string | null;
  description?: string | null;
  price: number;
  categoryId: number;
}

export interface UpdateProductCommand {
  name?: string | null;
  description?: string | null;
  price: number;
  categoryId: number;
}
