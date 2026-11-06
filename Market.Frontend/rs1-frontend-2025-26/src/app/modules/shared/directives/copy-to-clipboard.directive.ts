import { Directive, ElementRef, HostBinding, HostListener, Input, OnDestroy, inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ToasterService } from '../../../core/services/toaster.service';

/**
 * CUSTOM DIREKTIVA: bilo koji element postaje dugme za kopiranje teksta.
 *   <button [appCopyToClipboard]="key">Kopiraj</button>
 * Na klik kopira vrijednost, prikaže poruku i na 1.5 s doda CSS klasu "copied"
 * (komponenta je može stilizovati, npr. zelena kvačica).
 */
@Directive({
  selector: '[appCopyToClipboard]',
  standalone: false,
})
export class CopyToClipboardDirective implements OnDestroy {
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private el = inject(ElementRef<HTMLElement>);

  /** Tekst koji se kopira */
  @Input('appCopyToClipboard') value: string | null | undefined = '';

  /** Ključ prevoda za poruku nakon kopiranja (opcionalno) */
  @Input() copyMessage = 'GAMES.COPY.COPIED';

  @HostBinding('class.copied') copied = false;
  @HostBinding('attr.title') get title() {
    return this.translate.instant('GAMES.COPY.TOOLTIP');
  }

  private timer?: ReturnType<typeof setTimeout>;

  @HostListener('click', ['$event'])
  async onClick(event: Event): Promise<void> {
    event.preventDefault();
    event.stopPropagation(); // da klik ne "procuri" do roditelja (npr. kartice s linkom)
    if (!this.value) return;

    try {
      await navigator.clipboard.writeText(this.value);
    } catch {
      this.fallbackCopy(this.value); // stariji browseri / http bez dozvole
    }

    this.toaster.success(this.translate.instant(this.copyMessage));
    this.copied = true;
    clearTimeout(this.timer);
    this.timer = setTimeout(() => (this.copied = false), 1500);
  }

  /** Rezervni način kopiranja preko privremenog <textarea> */
  private fallbackCopy(text: string): void {
    const ta = document.createElement('textarea');
    ta.value = text;
    ta.style.position = 'fixed';
    ta.style.opacity = '0';
    (this.el.nativeElement as HTMLElement).appendChild(ta);
    ta.select();
    document.execCommand('copy');
    ta.remove();
  }

  ngOnDestroy(): void {
    clearTimeout(this.timer);
  }
}
