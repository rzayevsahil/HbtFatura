import { Component, EventEmitter, OnInit, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MATERIAL_ICONS_CURATED, filterMaterialIcons } from './material-icons.registry';
import { MaterialIconService } from '../../core/services/material-icon.service';
import { MaterialIconLigaturePipe } from './material-icon-ligature.pipe';

/**
 * Material ikon ligatürü seçimi — backdrop + liste + arama.
 * Liste API’den (aktif kayıtlar); hata/boşta yerel registry yedeği.
 */
@Component({
  selector: 'app-icon-picker-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, MaterialIconLigaturePipe],
  templateUrl: './icon-picker-modal.component.html',
  styleUrls: ['./icon-picker-modal.component.scss']
})
export class IconPickerModalComponent implements OnInit {
  @Output() iconSelected = new EventEmitter<string>();
  @Output() dismiss = new EventEmitter<void>();

  search = '';
  private readonly iconNames = signal<string[]>([...MATERIAL_ICONS_CURATED]);

  constructor(private materialIconsApi: MaterialIconService) {}

  ngOnInit(): void {
    this.materialIconsApi.getForPicker().subscribe({
      next: (names) => {
        if (names?.length) {
          this.iconNames.set(names);
        }
      },
      error: () => {
        /* yerel MATERIAL_ICONS_CURATED */
      }
    });
  }

  get filteredIcons(): string[] {
    return filterMaterialIcons(this.search, this.iconNames());
  }

  pick(name: string): void {
    this.iconSelected.emit(name);
  }

  stopBubble(event: Event): void {
    event.stopPropagation();
  }
}
