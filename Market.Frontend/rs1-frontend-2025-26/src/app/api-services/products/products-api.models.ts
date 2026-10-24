import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

// === QUERIES (READ) ===

/**
 * Query parameters for GET /Products
 * Corresponds to: ListProductsQuery.cs
 */
export class ListProductsRequest extends BasePagedQuery {
  search?: string | null;
  categoryId?: number | null;
  /** true = only active, false = only inactive, null = all */
  isEnabled?: boolean | null;
  minPrice?: number | null;
  maxPrice?: number | null;
}

/**
 * Response item for GET /Products
 * Corresponds to: ListProductsQueryDto.cs
 */
export interface ListProductsQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  categoryName: string;
  isEnabled: boolean;
  /** Best discount currently running for this game (null = not on sale) */
  activeDiscountPercentage?: number | null;
}

/**
 * Response for GET /Products/{id}
 * Corresponds to: GetProductByIdQueryDto.cs
 */
export interface GetProductByIdQueryDto {
  id: number;
  name: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  categoryName: string;
  categoryId: number;
  isEnabled: boolean;
}

/**
 * Paged response for GET /Products
 */
export type ListProductsResponse = PageResult<ListProductsQueryDto>;

// === COMMANDS (WRITE) ===

/**
 * Command for POST /Products
 * Corresponds to: CreateProductCommand.cs
 */
export interface CreateProductCommand {
  title: string;
  description?: string | null;
  price: number;
  categoryId: number;
}

/**
 * Command for PUT /Products/{id}
 * Corresponds to: UpdateProductCommand.cs
 */
export interface UpdateProductCommand {
  title: string;
  description?: string | null;
  price: number;
  categoryId: number;
}
