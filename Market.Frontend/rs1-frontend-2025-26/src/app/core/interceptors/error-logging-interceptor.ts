import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToasterService } from '../services/toaster.service';

/**
 * HTTP interceptor that logs errors and shows user-friendly notifications.
 *
 * Features:
 * - Logs errors to console (can be extended to send to logging service)
 * - Shows user-friendly error messages via toaster
 * - Handles different HTTP error status codes
 */
export const errorLoggingInterceptor: HttpInterceptorFn = (req, next) => {
  const toaster = inject(ToasterService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Log error to console (in production, send to logging service)
      console.error('HTTP Error:', {
        url: req.url,
        status: error.status,
        message: error.message,
        error: error.error,
        timestamp: new Date().toISOString()
      });

      // Don't show toaster for auth endpoints (handled by auth interceptor)
      if (!req.url.includes('/Auth/')) {
        const errorMessage = getErrorMessage(error);
        toaster.error(errorMessage);
      }

      // Re-throw error so components can handle it
      return throwError(() => error);
    })
  );
};

/**
 * Get user-friendly error message based on HTTP status code.
 */
function getErrorMessage(error: HttpErrorResponse): string {
  // Server-side error
  if (error.error instanceof ErrorEvent) {
    return `Network error: ${error.error.message}`;
  }

  // HTTP error response
  switch (error.status) {
    case 0:
      return 'Unable to connect to server. Please check your internet connection.';
    case 400:
      return error.error?.message || 'Invalid request. Please check your input.';
    case 401:
      return 'Unauthorized. Please log in again.';
    case 403:
      return 'You do not have permission to perform this action.';
    case 404:
      return 'The requested resource was not found.';
    case 500:
      return 'Server error. Please try again later.';
    case 503:
      return 'Service temporarily unavailable. Please try again later.';
    default:
      return `An error occurred: ${error.statusText || 'Unknown error'}`;
  }
}
