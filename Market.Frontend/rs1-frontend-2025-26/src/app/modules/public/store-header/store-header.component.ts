import { Component, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

/**
 * Zajedničko zaglavlje za katalog i stranicu igre:
 * lijevo logo (link na početnu), desno izbor jezika (isti princip kao na Velidovom landing pageu).
 */
@Component({
  selector: 'app-store-header',
  standalone: false,
  templateUrl: './store-header.component.html',
  styleUrl: './store-header.component.scss',
})
export class StoreHeaderComponent {
  private translate = inject(TranslateService);

  // Jezik se pamti u localStorage pod istim ključem kao kod Velida -> isti izbor na svim stranicama
  currentLang: string = localStorage.getItem('language') || this.translate.currentLang || 'bs';

  languages = [
    { code: 'bs', name: 'Bosanski', flag: '🇧🇦' },
    { code: 'en', name: 'English', flag: '🇬🇧' },
  ];

  constructor() {
    // Ako je korisnik ranije izabrao jezik, primijeni ga i ovdje
    if (this.translate.currentLang !== this.currentLang) {
      this.translate.use(this.currentLang);
    }
  }

  switchLanguage(code: string): void {
    this.currentLang = code;
    this.translate.use(code);
    localStorage.setItem('language', code);
  }

  get current() {
    return this.languages.find((l) => l.code === this.currentLang);
  }
}
