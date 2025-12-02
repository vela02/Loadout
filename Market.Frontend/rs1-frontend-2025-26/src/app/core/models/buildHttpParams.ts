import { HttpParams } from '@angular/common/http';

/**
 * Pretvara bilo koji objekt u Angular HttpParams.
 *
 * Pravila:
 *  - null/undefined se preskaču
 *  - prazni stringovi ("") se preskaču
 *  - nizovi se dodaju višestruko (?ids=1&ids=2)
 *  - objekti se serializuju u JSON (ako treba)
 */
export function buildHttpParams(obj: Record<string, any>): HttpParams {
  let params = new HttpParams();
  if (obj === undefined || obj === null)
    return params;

  Object.entries(obj).forEach(([key, value]) => {
    // 1) null / undefined → preskoči
    if (value === null || value === undefined) {
      return;
    }

    // 2) prazni stringovi → preskoči
    if (typeof value === 'string' && value.trim() === '') {
      return;
    }

    // 3) niz → dodaj svaki element posebno
    if (Array.isArray(value)) {
      value.forEach(val => {
        if (val !== null && val !== undefined) {
          params = params.append(key, String(val));
        }
      });
      return;
    }

    // 4) objekti → JSON.stringify
    if (typeof value === 'object') {
      params = params.set(key, JSON.stringify(value));
      return;
    }

    // 5) sve ostalo → kao string
    params = params.set(key, String(value));
  });

  return params;
}
