import { Component, Input, OnChanges } from '@angular/core';
import { toDataURL } from 'qrcode';

/**
 * Dinamički QR kod: generiše se u browseru iz bilo kojeg teksta (link, ključ...).
 *   <app-qr-code [value]="link" [size]="180"></app-qr-code>
 * Svaki put kad se "value" promijeni, QR se ponovo generiše (ngOnChanges).
 */
@Component({
  selector: 'app-qr-code',
  standalone: false,
  template: `
    @if (dataUrl) {
      <img class="qr" [src]="dataUrl" [width]="size" [height]="size" [alt]="value" />
    }
  `,
  styles: [`
    .qr {
      display: block;
      padding: 8px;
      background: #ffffff;           /* QR mora biti taman na svijetlom da bi ga kamera očitala */
      border-radius: 12px;
      box-shadow: 0 0 24px rgba(34, 211, 238, 0.35);
    }
  `],
})
export class QrCodeComponent implements OnChanges {
  @Input() value = '';
  @Input() size = 160;

  dataUrl = '';

  async ngOnChanges(): Promise<void> {
    if (!this.value) {
      this.dataUrl = '';
      return;
    }
    this.dataUrl = await toDataURL(this.value, {
      width: this.size * 2,          // dvostruka rezolucija -> oštar QR na retina ekranima
      margin: 1,
      errorCorrectionLevel: 'M',
      color: { dark: '#0b0c1a', light: '#ffffff' },
    });
  }
}
