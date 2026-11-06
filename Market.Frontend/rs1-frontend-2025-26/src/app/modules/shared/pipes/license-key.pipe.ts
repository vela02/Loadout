import { Pipe, PipeTransform } from '@angular/core';

/**
 * CUSTOM PIPE: formatira licencni ključ u grupe radi lakšeg čitanja/prepisivanja.
 *   {{ 'ABCD1234EFGH5678' | licenseKey }}      -> ABCD-1234-EFGH-5678
 *   {{ key | licenseKey: 4 : ' ' }}             -> ABCD 1234 EFGH 5678
 *   {{ key | licenseKey: 4 : '-' : true }}      -> ABCD-****-****-5678  (maskirano)
 */
@Pipe({
  name: 'licenseKey',
  standalone: false,
})
export class LicenseKeyPipe implements PipeTransform {
  transform(value: string | null | undefined, groupSize = 4, separator = '-', mask = false): string {
    if (!value) return '';

    const clean = value.replace(/[^A-Za-z0-9]/g, '').toUpperCase(); // ukloni stare crtice/razmake
    const groups = clean.match(new RegExp(`.{1,${groupSize}}`, 'g')) ?? [];

    // Maskiranje: sakrij sve grupe osim prve i zadnje
    const visible = mask
      ? groups.map((g, i) => (i === 0 || i === groups.length - 1 ? g : '*'.repeat(g.length)))
      : groups;

    return visible.join(separator);
  }
}
