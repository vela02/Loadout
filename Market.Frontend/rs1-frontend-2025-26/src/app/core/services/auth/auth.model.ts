// Request for POST /Auth/login
export interface LoginRequest {
  email: string;
  password: string;
  fingerprint?: string | null;
}

// Response for POST /Auth/login
// C# LoginCommandDto: AccessToken, RefreshToken, ExpiresAtUtc
export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  // ISO string (UTC) returned by backend, e.g. "2025-12-02T23:59:59Z"
  expiresAtUtc: string;
}

// Request for POST /Auth/refresh
// C# RefreshTokenCommand: RefreshToken, Fingerprint?
export interface RefreshTokenRequest {
  refreshToken: string;
  fingerprint?: string | null;
}

// Response for POST /Auth/refresh
// C# RefreshTokenCommandDto:
//  AccessToken, RefreshToken, AccessTokenExpiresAtUtc, RefreshTokenExpiresAtUtc
export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
}

// Request for POST /Auth/logout
// C# LogoutCommand: RefreshToken
export interface LogoutRequest {
  refreshToken: string;
}
