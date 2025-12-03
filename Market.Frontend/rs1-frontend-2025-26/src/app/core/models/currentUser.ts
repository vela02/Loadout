// src/app/core/auth/current-user.model.ts
export interface CurrentUser {
  userId: number;
  email: string;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  tokenVersion: number;
}
