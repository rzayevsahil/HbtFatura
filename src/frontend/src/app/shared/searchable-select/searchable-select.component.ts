import {
  Component,
  ElementRef,
  HostListener,
  Input,
  forwardRef,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface SearchableSelectOption {
  id: string;
  /** Ana satır (ör. cari ünvanı, grup adı) */
  primary: string;
  /** İkinci satır veya arama metni (ör. kod, vergi no) */
  secondary?: string;
}

@Component({
  selector: 'app-searchable-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './searchable-select.component.html',
  styleUrls: ['./searchable-select.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SearchableSelectComponent),
      multi: true
    }
  ]
})
export class SearchableSelectComponent implements ControlValueAccessor {
  private readonly host = inject(ElementRef<HTMLElement>);

  /** Seçenek listesi */
  @Input() options: SearchableSelectOption[] = [];

  /** `dropdown`: tetikleyici + açılır panel (sipariş cari gibi). `inline`: arama + liste sürekli görünür (modal içi). */
  @Input() layout: 'dropdown' | 'inline' = 'dropdown';

  /** Tetikleyicide / üst özetde boşken */
  @Input() placeholder = '';

  /** Arama kutusu placeholder */
  @Input() searchPlaceholder = '';

  /** Liste için max yükseklik (css değeri, örn. 220px) */
  @Input() listMaxHeight = '220px';

  /** İlk satırda "seçimi temizle" (value = null) */
  @Input() showNoneOption = false;

  @Input() noneOptionLabel = '';

  /** true ise temizlemede forma `''` yazar (örn. izin kodu alanı). */
  @Input() noneEmitsEmptyString = false;

  /** Filtre sonucu boş */
  @Input() noResultsLabel = '';

  /** Arama input id — panel açıkken odak / erişilebilirlik */
  @Input() searchInputId = '';

  /** Dropdown tetikleyici id — kapalıyken dış `<label for="...">` buraya bağlansın */
  @Input() triggerId = '';

  /**
   * true ise `pick` ile forma `number` (veya null) yazar; `formControl` sayısal enum için kullanılır.
   * Seçenek `id` değerleri sayıya çevrilebilir olmalıdır.
   */
  @Input() emitNumeric = false;

  value: string | null = null;
  searchText = '';
  panelOpen = false;
  private cvaDisabled = false;

  private onChange: (v: string | number | null) => void = () => {};
  private onTouched: () => void = () => {};

  get isDisabled(): boolean {
    return this.cvaDisabled;
  }

  get displayLabel(): string {
    if (!this.value) return this.placeholder;
    const o = this.options.find(x => x.id === this.value);
    return o?.primary ?? this.placeholder;
  }

  get filteredOptions(): SearchableSelectOption[] {
    const t = this.searchText?.trim().toLowerCase() ?? '';
    if (!t) return this.options;
    return this.options.filter(
      o =>
        (o.primary?.toLowerCase().includes(t) ?? false) ||
        (o.secondary?.toLowerCase().includes(t) ?? false)
    );
  }

  writeValue(v: string | number | null | undefined): void {
    if (v === null || v === undefined || v === '') {
      this.value = null;
    } else {
      this.value = String(v);
    }
  }

  registerOnChange(fn: (v: string | number | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.cvaDisabled = isDisabled;
  }

  toggle(ev: Event): void {
    ev.stopPropagation();
    if (this.isDisabled) return;
    this.panelOpen = !this.panelOpen;
    if (this.panelOpen) {
      this.searchText = '';
    }
  }

  pick(id: string | null): void {
    if (this.isDisabled) return;
    if (id === null || id === undefined) {
      this.value = null;
      this.onChange(this.noneEmitsEmptyString ? '' : null);
      this.onTouched();
      this.panelOpen = false;
      this.searchText = '';
      return;
    }
    this.value = id;
    let emitted: string | number | null = id;
    if (this.emitNumeric) {
      const n = Number(id);
      emitted = id === '' || !Number.isFinite(n) ? null : n;
    }
    this.onChange(emitted);
    this.onTouched();
    this.panelOpen = false;
    this.searchText = '';
  }

  onSearchInput(ev: Event): void {
    const v = (ev.target as HTMLInputElement).value;
    this.searchText = v;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(e: MouseEvent): void {
    if (this.layout !== 'dropdown' || !this.panelOpen) return;
    if (!this.host.nativeElement.contains(e.target as Node)) {
      this.panelOpen = false;
    }
  }
}
