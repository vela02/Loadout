// === ENUMS ===

/**
 * What a promotion targets.
 * Corresponds to: PromotionTargetTypes (Market.Domain.Models.Promotion.cs)
 */
export type PromotionTargetType =
  | 'Games'
  | 'Category'
  | 'Genre'
  | 'Publisher'
  | 'Developer'
  | 'Tag';

// === QUERIES (READ) ===

/**
 * Response item for GET /api/admin/promotions
 * Corresponds to: ListPromotionsQueryDto.cs
 */
export interface ListPromotionsQueryDto {
  id: number;
  name: string;
  description?: string | null;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  targetType: PromotionTargetType;
  targetValue?: string | null;
  gameCount: number;
  createdAt: string;
}

export interface PromotionOptionDto {
  id: number;
  name: string;
}

export interface PromotionGameDto {
  id: number;
  title: string;
  price: number;
  categoryId?: number | null;
  genre?: string | null;
  publisher?: string | null;
  developer?: string | null;
  tagIds: number[];
}

/**
 * Response for GET /api/admin/promotions/options
 * Corresponds to: GetPromotionOptionsQueryDto.cs
 */
export interface GetPromotionOptionsQueryDto {
  categories: PromotionOptionDto[];
  tags: PromotionOptionDto[];
  genres: string[];
  publishers: string[];
  developers: string[];
  games: PromotionGameDto[];
}

// === COMMANDS (WRITE) ===

/**
 * Command for POST /api/admin/promotions
 * Corresponds to: CreatePromotionCommand.cs
 */
export interface CreatePromotionCommand {
  name: string;
  description?: string | null;
  discountPercentage: number;
  startDate: string;
  endDate: string;
  targetType: PromotionTargetType;
  targetId?: number | null;
  targetText?: string | null;
  gameIds?: number[] | null;
}
