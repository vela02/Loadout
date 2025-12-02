import { PageResult } from '../../models/pageResult';
import { BasePagedRequest } from '../../models/basePagedRequest';

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
export class ListProductsRequest extends BasePagedRequest {
  // future filters: categoryId?, isEnabled?, priceMin?, priceMax?
  search ="";
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
