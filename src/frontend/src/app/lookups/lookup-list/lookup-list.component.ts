import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto, LookupGroupDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';

const VAT_RATE_GROUP_NAME = 'VatRate';

@Component({
  selector: 'app-lookup-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './lookup-list.component.html',
  styleUrls: ['./lookup-list.component.scss']
})
export class LookupListComponent implements OnInit {
  rawLookups = signal<LookupDto[]>([]);
  groups = signal<LookupGroupDto[]>([]);
  showForm = false;
  saving = false;
  editId: string | null = null;
  /** KDV satırı düzenlenirken grup/sıra/durum kilitli; kod + renk (görünen ad koddan türetilir). */
  editingVatRate = false;
  model: Partial<LookupDto> = {};

  constructor(private service: LookupService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.refresh();
    this.service.loadGroups().subscribe(g => this.groups.set(g));
  }

  refresh(): void {
    this.service.load().subscribe(list => this.rawLookups.set(list));
  }

  isNewGroup(item: LookupDto, index: number): boolean {
    if (index === 0) return true;
    return item.lookupGroupId !== this.rawLookups()[index - 1].lookupGroupId;
  }

  isVatRateRow(item: LookupDto): boolean {
    return item.group?.name === VAT_RATE_GROUP_NAME;
  }

  /** Yeni kayıtta KDV grubu listeden çıkarılır — zaten tek satır varsa ikincisi eklenemez. */
  groupsForAdd(): LookupGroupDto[] {
    const all = this.groups();
    const hasVat = this.rawLookups().some(l => l.group?.name === VAT_RATE_GROUP_NAME);
    if (!hasVat) return all;
    return all.filter(g => g.name !== VAT_RATE_GROUP_NAME);
  }

  formGroupOptions(): LookupGroupDto[] {
    return this.editId ? this.groups() : this.groupsForAdd();
  }

  /** Yeni kayıtta KDV grubu seçiliyse veya mevcut KDV düzenleniyorsa. */
  isVatRateForm(): boolean {
    if (this.editingVatRate) return true;
    const id = this.model.lookupGroupId;
    if (!id) return false;
    return this.groups().some(g => g.id === id && g.name === VAT_RATE_GROUP_NAME);
  }

  /** KDV için görünen ad: % + kod (sunucu ile aynı kural). */
  syncVatRateDisplayName(): void {
    const c = (this.model.code ?? '').toString().trim();
    this.model.name = c.length > 0 ? `%${c}` : '';
  }

  vatDisplayPreview(): string {
    const c = (this.model.code ?? '').toString().trim();
    return c.length > 0 ? `%${c}` : '—';
  }

  onLookupGroupChange(): void {
    if (this.isVatRateForm()) this.syncVatRateDisplayName();
  }

  onCodeChange(): void {
    if (this.isVatRateForm()) this.syncVatRateDisplayName();
  }

  openAdd() {
    this.editId = null;
    this.editingVatRate = false;
    if (this.groupsForAdd().length === 0) {
      this.toastr.warning('Eklenebilecek grup bulunmuyor.');
      return;
    }
    this.model = { isActive: true, sortOrder: 0 };
    this.showForm = true;
  }

  openEdit(item: LookupDto) {
    this.editId = item.id;
    this.editingVatRate = item.group?.name === VAT_RATE_GROUP_NAME;
    this.model = { ...item };
    if (this.editingVatRate) this.syncVatRateDisplayName();
    this.showForm = true;
  }

  save() {
    if (!this.model.lookupGroupId) {
      this.toastr.error('Lütfen bir grup seçiniz');
      return;
    }
    if (this.isVatRateForm()) {
      this.syncVatRateDisplayName();
      if (!(this.model.code ?? '').toString().trim()) {
        this.toastr.error('KDV oranı (kod) gerekli');
        return;
      }
    } else if (!(this.model.name ?? '').toString().trim()) {
      this.toastr.error('Görünen ad gerekli');
      return;
    }
    this.saving = true;
    const obs = this.editId
      ? this.service.update(this.editId, this.model)
      : this.service.create(this.model);

    obs.subscribe({
      next: () => {
        this.toastr.success('Kaydedildi');
        this.showForm = false;
        this.saving = false;
        this.refresh();
      },
      error: (e: unknown) => {
        const err = e as { error?: { message?: string } };
        this.toastr.error(err?.error?.message ?? 'Hata oluştu');
        this.saving = false;
      }
    });
  }

  delete(item: LookupDto) {
    if (!confirm(`"${item.name}" tanımını silmek istediğinize emin misiniz?`)) return;
    this.service.delete(item.id).subscribe({
      next: () => {
        this.toastr.success('Silindi');
        this.refresh();
      },
      error: (e: unknown) => {
        const err = e as { error?: { message?: string } };
        this.toastr.error(err?.error?.message ?? 'Silinemedi');
      }
    });
  }
}
