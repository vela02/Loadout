import { Injectable, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { jsPDF } from 'jspdf';
import { toDataURL } from 'qrcode';
import { OrderHistory } from '../../api-services/games/games-api.models';
import { UserPurchaseHistory } from '../../api-services/admin-users/admin-users-api.service';

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

  /**
   * PDF IZVJEŠTAJ O KUPOVINAMA KORISNIKA (admin)
   * Parametri: korisnik + period (datum od, datum do) – dolaze iz UserPurchaseHistory.
   * Sadrži: podatke o korisniku, period, sažetak, tabelu narudžbi i ukupan iznos.
   */
  async downloadUserReport(data: UserPurchaseHistory): Promise<void> {
    const t = (key: string, params?: object) => this.ascii(this.translate.instant(key, params));
    const fmtDate = (iso: string | null) => (iso ? new Date(iso).toLocaleDateString('de-DE') : '');

    const doc = new jsPDF({ unit: 'mm', format: 'a4' });
    const W = doc.internal.pageSize.getWidth();

    // ---------- Zaglavlje ----------
    const header = () => {
      doc.setFillColor(11, 12, 26);
      doc.rect(0, 0, W, 30, 'F');
      doc.setFillColor(34, 211, 238);
      doc.rect(0, 30, W / 2, 1.2, 'F');
      doc.setFillColor(224, 64, 251);
      doc.rect(W / 2, 30, W / 2, 1.2, 'F');
      doc.setTextColor(255, 255, 255);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(20);
      doc.text('LOADOUT', 15, 17);
      doc.setFont('helvetica', 'normal');
      doc.setFontSize(10);
      doc.setTextColor(141, 144, 184);
      doc.text(t('GAMES.PDF.REPORT_TITLE'), 15, 24);
    };
    header();

    // ---------- Korisnik i period ----------
    let y = 44;
    doc.setTextColor(20, 20, 30);
    doc.setFontSize(10);
    const period = data.from || data.to
      ? `${fmtDate(data.from) || '...'} - ${fmtDate(data.to) || '...'}`
      : t('GAMES.PDF.REPORT_ALL_TIME');
    const info: [string, string][] = [
      [t('GAMES.PDF.CUSTOMER'), `${this.ascii(data.username)} (${data.email})`],
      [t('GAMES.PDF.REPORT_ROLE'), this.ascii(data.roleName)],
      [t('GAMES.PDF.REPORT_PERIOD'), period],
      [t('GAMES.PDF.REPORT_GENERATED'), new Date().toLocaleString('de-DE')],
    ];
    info.forEach(([label, value]) => {
      doc.setFont('helvetica', 'bold');
      doc.text(label, 15, y);
      doc.setFont('helvetica', 'normal');
      doc.text(value, 60, y);
      y += 7;
    });

    // ---------- Sažetak (3 "kartice") ----------
    y += 4;
    const boxW = (W - 30 - 10) / 3;
    const summary: [string, string][] = [
      [t('GAMES.USER_HISTORY.ORDERS'), String(data.ordersCount)],
      [t('GAMES.USER_HISTORY.GAMES'), String(data.gamesCount)],
      [t('GAMES.USER_HISTORY.TOTAL'), `${data.totalSpent.toFixed(2)} KM`],
    ];
    summary.forEach(([label, value], i) => {
      const x = 15 + i * (boxW + 5);
      doc.setFillColor(243, 244, 250);
      doc.setDrawColor(220, 222, 235);
      doc.roundedRect(x, y, boxW, 18, 2, 2, 'FD');
      doc.setFont('helvetica', 'normal');
      doc.setFontSize(8);
      doc.setTextColor(110, 112, 140);
      doc.text(label, x + 4, y + 6);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(13);
      doc.setTextColor(i === 2 ? 124 : 20, i === 2 ? 58 : 20, i === 2 ? 237 : 30);
      doc.text(value, x + 4, y + 14);
    });
    y += 28;

    // ---------- Tabela narudžbi ----------
    const tableHeader = () => {
      doc.setFillColor(23, 26, 54);
      doc.rect(15, y - 6, W - 30, 9, 'F');
      doc.setTextColor(255, 255, 255);
      doc.setFont('helvetica', 'bold');
      doc.setFontSize(9);
      doc.text(t('GAMES.PDF.DATE'), 18, y);
      doc.text(t('GAMES.PDF.ORDER'), 45, y);
      doc.text(t('GAMES.PDF.ITEM'), 65, y);
      doc.text(t('GAMES.PDF.STATUS'), 140, y);
      doc.text(t('GAMES.PDF.PRICE'), W - 18, y, { align: 'right' });
      y += 9;
    };
    tableHeader();

    doc.setFontSize(9);
    if (data.orders.length === 0) {
      doc.setTextColor(110, 112, 140);
      doc.setFont('helvetica', 'italic');
      doc.text(t('GAMES.USER_HISTORY.EMPTY'), 18, y);
      y += 8;
    }

    for (const o of data.orders) {
      const titles = o.games.map((g) => this.ascii(g.title));
      const rowH = Math.max(1, titles.length) * 5 + 3;
      if (y + rowH > 270) {              // nova stranica + ponovo zaglavlje tabele
        doc.addPage();
        header();
        y = 44;
        tableHeader();
        doc.setFontSize(9);
      }
      const cancelled = o.statusId === 5;
      doc.setFont('helvetica', 'normal');
      doc.setTextColor(cancelled ? 160 : 20, cancelled ? 160 : 20, cancelled ? 170 : 30);
      doc.text(new Date(o.date).toLocaleDateString('de-DE'), 18, y);
      doc.text(`#${o.id}`, 45, y);
      titles.forEach((title, i) => doc.text(title.length > 38 ? title.slice(0, 37) + '...' : title, 65, y + i * 5));
      doc.text(this.ascii(o.status), 140, y);
      doc.setFont('helvetica', 'bold');
      doc.text(`${o.totalAmount.toFixed(2)} KM`, W - 18, y, { align: 'right' });
      y += rowH;
      doc.setDrawColor(225, 226, 236);
      doc.line(15, y - 3, W - 15, y - 3);
    }

    // ---------- Ukupno ----------
    y += 4;
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(12);
    doc.setTextColor(20, 20, 30);
    doc.text(t('GAMES.PDF.TOTAL'), 18, y);
    doc.setTextColor(124, 58, 237);
    doc.text(`${data.totalSpent.toFixed(2)} KM`, W - 18, y, { align: 'right' });
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8);
    doc.setTextColor(141, 144, 184);
    doc.text(t('GAMES.PDF.REPORT_NOTE'), 18, y + 6);

    // ---------- Broj stranice u podnožju ----------
    const pages = doc.getNumberOfPages();
    for (let p = 1; p <= pages; p++) {
      doc.setPage(p);
      doc.setFontSize(8);
      doc.setTextColor(141, 144, 184);
      doc.text(`Loadout  |  ${p} / ${pages}`, W / 2, 287, { align: 'center' });
    }

    const suffix = data.from || data.to ? `_${data.from ?? 'start'}_${data.to ?? 'today'}` : '';
    doc.save(`Loadout-report-${this.ascii(data.username)}${suffix}.pdf`);
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
