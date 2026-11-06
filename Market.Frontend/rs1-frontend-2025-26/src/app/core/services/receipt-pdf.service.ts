import { Injectable, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { jsPDF } from 'jspdf';
import { toDataURL } from 'qrcode';
import { OrderHistory } from '../../api-services/games/games-api.models';

/**
 * Generiše PDF potvrdu o kupovini za JEDNU narudžbu (parametar: narudžba / ID).
 * PDF se pravi u browseru (jsPDF) i odmah preuzima.
 * Sadrži: broj i datum narudžbe, kupca, stavke s cijenama, ukupno,
 * licencne ključeve i QR kod s brojem narudžbe (za provjeru).
 */
@Injectable({ providedIn: 'root' })
export class ReceiptPdfService {
  private translate = inject(TranslateService);

  async download(order: OrderHistory, customerEmail: string): Promise<void> {
    const t = (key: string) => this.ascii(this.translate.instant(key));
    const ref = order.referenceNumber || `#${order.id}`;

    const doc = new jsPDF({ unit: 'mm', format: 'a4' });
    const W = doc.internal.pageSize.getWidth();
    let y = 0;

    // ---------- Zaglavlje (tamna traka kao u aplikaciji) ----------
    doc.setFillColor(11, 12, 26);
    doc.rect(0, 0, W, 36, 'F');
    doc.setFillColor(34, 211, 238);
    doc.rect(0, 36, W / 2, 1.5, 'F');
    doc.setFillColor(224, 64, 251);
    doc.rect(W / 2, 36, W / 2, 1.5, 'F');

    doc.setTextColor(255, 255, 255);
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(24);
    doc.text('LOADOUT', 15, 20);
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(11);
    doc.setTextColor(141, 144, 184);
    doc.text(t('GAMES.PDF.TITLE'), 15, 28);

    // ---------- Podaci o narudžbi ----------
    y = 52;
    doc.setTextColor(20, 20, 30);
    doc.setFontSize(10);
    const info: [string, string][] = [
      [t('GAMES.PDF.ORDER'), ref],
      [t('GAMES.PDF.DATE'), new Date(order.date).toLocaleString('de-DE')],
      [t('GAMES.PDF.STATUS'), this.ascii(order.status)],
      [t('GAMES.PDF.CUSTOMER'), customerEmail],
    ];
    info.forEach(([label, value]) => {
      doc.setFont('helvetica', 'bold');
      doc.text(label, 15, y);
      doc.setFont('helvetica', 'normal');
      doc.text(value, 55, y);
      y += 7;
    });

    // QR kod s brojem narudžbe (desno gore)
    const qr = await toDataURL(`LOADOUT-ORDER:${ref}`, { width: 300, margin: 1 });
    doc.addImage(qr, 'PNG', W - 15 - 32, 45, 32, 32);

    // ---------- Tabela stavki ----------
    y += 8;
    doc.setFillColor(23, 26, 54);
    doc.rect(15, y - 6, W - 30, 9, 'F');
    doc.setTextColor(255, 255, 255);
    doc.setFont('helvetica', 'bold');
    doc.text(t('GAMES.PDF.ITEM'), 18, y);
    doc.text(t('GAMES.PDF.PRICE'), W - 18, y, { align: 'right' });
    y += 10;

    doc.setTextColor(20, 20, 30);
    for (const g of order.games) {
      if (y > 260) { doc.addPage(); y = 20; } // nova stranica ako ponestane mjesta

      doc.setFont('helvetica', 'bold');
      doc.text(this.ascii(g.title), 18, y);
      doc.text(`${g.priceAtPurchase.toFixed(2)} KM`, W - 18, y, { align: 'right' });
      y += 6;

      doc.setFont('courier', 'normal');
      doc.setFontSize(9);
      if (g.licenseKeys.length) {
        g.licenseKeys.forEach((k) => {
          doc.text(`${t('GAMES.PDF.KEY')}: ${this.formatKey(k)}`, 22, y);
          y += 5;
        });
      } else {
        doc.text(t('GAMES.PDF.KEY_LATER'), 22, y);
        y += 5;
      }
      doc.setFontSize(10);

      doc.setDrawColor(220, 220, 230);
      doc.line(15, y, W - 15, y);
      y += 7;
    }

    // ---------- Ukupno ----------
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(13);
    doc.text(t('GAMES.PDF.TOTAL'), 18, y + 2);
    doc.setTextColor(124, 58, 237);
    doc.text(`${order.totalAmount.toFixed(2)} KM`, W - 18, y + 2, { align: 'right' });

    // ---------- Podnožje ----------
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8);
    doc.setTextColor(141, 144, 184);
    doc.text(t('GAMES.PDF.FOOTER'), W / 2, 285, { align: 'center' });

    doc.save(`Loadout-${ref}.pdf`);
  }

  /** ABCD1234EFGH5678 -> ABCD-1234-EFGH-5678 */
  private formatKey(key: string): string {
    return key.replace(/(.{4})(?=.)/g, '$1-');
  }

  /**
   * jsPDF-ovi ugrađeni fontovi nemaju slova č, ć, š, ž, đ,
   * pa ih pretvaramo u najbliža ASCII slova (č -> c, đ -> dj).
   */
  private ascii(text: string): string {
    return (text ?? '')
      .replace(/đ/g, 'dj').replace(/Đ/g, 'Dj')
      .normalize('NFD').replace(/[̀-ͯ]/g, '');
  }
}
