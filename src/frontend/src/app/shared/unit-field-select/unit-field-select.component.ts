import { Component, ElementRef, HostListener, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto } from '../../core/models';

@Component({
  selector: 'app-unit-field-select',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './unit-field-select.component.html',
  styleUrls: ['./unit-field-select.component.scss']
})
export class UnitFieldSelectComponent {
  private readonly host = inject(ElementRef<HTMLElement>);

  /** Ürün formu veya kalem satırı; içinde `unit` kontrolü olmalı */
  @Input({ required: true }) itemGroup!: FormGroup;

  /** Boşken tetikleyicide gösterilecek metin */
  @Input() emptyLabel = 'Birim seçin';

  constructor(public lookups: LookupService) {}

  panelOpen = false;
  searchText = '';

  get unitControl(): FormControl<string> {
    return this.itemGroup.get('unit') as FormControl<string>;
  }

  get selectedTitle(): string {
    const u = (this.unitControl?.value ?? '').toString().trim();
    return u.length > 0 ? u : this.emptyLabel;
  }

  get filteredUnits(): LookupDto[] {
    const list = this.lookups.getGroup('ProductUnit')();
    const t = this.searchText?.trim().toLowerCase();
    if (!t) return list;
    return list.filter(
      l =>
        (l.name?.toLowerCase().includes(t) ?? false) ||
        (l.code?.toLowerCase().includes(t) ?? false)
    );
  }

  toggle(e: Event): void {
    e.stopPropagation();
    this.panelOpen = !this.panelOpen;
    if (this.panelOpen) this.searchText = '';
  }

  pick(l: LookupDto): void {
    const v = (l.name || l.code || '').trim();
    this.unitControl.patchValue((v.length > 0 ? v : 'Adet') as string);
    this.panelOpen = false;
    this.searchText = '';
  }

  @HostListener('document:click', ['$event'])
  onDoc(e: MouseEvent): void {
    if (!this.panelOpen) return;
    if (!this.host.nativeElement.contains(e.target as Node)) {
      this.panelOpen = false;
    }
  }
}
