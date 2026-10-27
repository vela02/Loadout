// === QUERIES ===

/**
 * Response for GET /api/users/profile
 */
export interface GetProfileDto {
  username: string;
  email: string;
  createdAt: string;
  lastLogin: string | null;
  role: string | null;
  ordersCount: number;
  totalSpent: number;
  ownedGamesCount: number;
  wishlistCount: number;
  recentOrders: GetProfileRecentOrderDto[];
}

export interface GetProfileRecentOrderDto {
  id: number;
  date: string;
  totalAmount: number;
  statusId: number | null;
  status: string;
  itemsCount: number;
}

// === COMMANDS ===

/**
 * Request for POST /api/users/change-password
 */
export interface ChangePasswordCommand {
  oldPassword: string;
  newPassword: string;
  /** Refresh token of this session – it stays valid, all other sessions are logged out */
  currentRefreshToken?: string | null;
}
