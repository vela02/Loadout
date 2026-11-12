import {
  AfterViewInit, Component, ElementRef, Input, OnDestroy, ViewChild, ViewEncapsulation, forwardRef,
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import Quill from 'quill';

/**
 * WYSIWYG EDITOR (Quill) kao Angular form kontrola.
 *
 * Implementira ControlValueAccessor -> koristi se kao obično polje u reactive formi:
 *   <app-rich-text-editor formControlName="message"></app-rich-text-editor>
 * Vrijednost forme je HTML string (npr. "<p><strong>Popust</strong> 30%</p>").
 */
@Component({
  selector: 'app-rich-text-editor',
  standalone: false,
  template: `<div class="loadout-rte" [class.invalid]="invalid"><div #editor></div></div>`,
  styleUrl: './rich-text-editor.component.scss',
  // Quill sam pravi HTML (toolbar, editor) -> Angularovi "scoped" stilovi ga ne bi pogodili,
  // pa su stilovi globalni, ali svi počinju s .loadout-rte da ne utiču na ostatak aplikacije.
  encapsulation: ViewEncapsulation.None,
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => RichTextEditorComponent), multi: true },
  ],
})
export class RichTextEditorComponent implements ControlValueAccessor, AfterViewInit, OnDestroy {
  @ViewChild('editor', { static: true }) editorEl!: ElementRef<HTMLDivElement>;
  @Input() placeholder = '';
  @Input() invalid = false;

  private quill?: Quill;
  private pendingValue = '';                     // vrijednost stigla prije nego što je Quill napravljen
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  ngAfterViewInit(): void {
    this.quill = new Quill(this.editorEl.nativeElement, {
      theme: 'snow',
      placeholder: this.placeholder,
      modules: {
        // Alatna traka: bold, italic, underline, liste, link, brisanje formatiranja
        toolbar: [
          ['bold', 'italic', 'underline'],
          [{ list: 'ordered' }, { list: 'bullet' }],
          ['link'],
          ['clean'],
        ],
      },
    });

    this.setHtml(this.pendingValue);

    // Svaka promjena teksta -> javi formi novi HTML
    this.quill.on('text-change', () => this.onChange(this.getHtml()));
    this.quill.root.addEventListener('blur', () => this.onTouched());
  }

  /** HTML iz editora; prazan editor ("<p><br></p>") -> prazan string, da "required" validacija radi */
  private getHtml(): string {
    if (!this.quill || this.quill.getText().trim().length === 0) return '';
    // getSemanticHTML daje "čist" HTML (<ul>/<ol> umjesto Quill-ovih internih oznaka)
    return this.quill.getSemanticHTML().replace(/&nbsp;/g, ' ');
  }

  private setHtml(html: string): void {
    if (!this.quill) return;
    const delta = this.quill.clipboard.convert({ html: html || '' });
    this.quill.setContents(delta, 'silent');      // 'silent' -> ne okida text-change (nije korisnikova izmjena)
  }

  // ---------- ControlValueAccessor ----------
  writeValue(value: string | null): void {
    this.pendingValue = value ?? '';
    this.setHtml(this.pendingValue);
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.quill?.enable(!isDisabled);
  }

  ngOnDestroy(): void {
    this.quill?.off('text-change');
  }
}
