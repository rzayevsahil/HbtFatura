import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MATERIAL_ICONS_CURATED, filterMaterialIcons } from './material-icons.registry';

/**
 * Material ikon ligatürü seçimi — backdrop + liste + arama.
 * Başka formlarda da `app-icon-picker-modal` olarak kullanılabilir.
 */
@Component({
  selector: 'app-icon-picker-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './icon-picker-modal.component.html',
  styleUrls: ['./icon-picker-modal.component.scss']
})
export class IconPickerModalComponent {
  @Output() iconSelected = new EventEmitter<string>();
  @Output() dismiss = new EventEmitter<void>();

  search = '';

  get filteredIcons(): string[] {
    return filterMaterialIcons(this.search, MATERIAL_ICONS_CURATED);
  }

  pick(name: string): void {
    this.iconSelected.emit(name);
  }

  onBackdropClick(): void {
    this.dismiss.emit();
  }

  stopBubble(event: Event): void {
    event.stopPropagation();
  }
}
