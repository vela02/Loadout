// src/app/core/models/pageResult.ts

export interface PageResult<T> {
  items: T[];
  pageSize: number;
  currentPage: number;
  includedTotal: boolean;
  totalItems: number;
  totalPages: number;
}
