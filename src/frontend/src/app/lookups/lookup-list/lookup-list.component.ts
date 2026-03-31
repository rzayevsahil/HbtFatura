import { ChangeDetectorRef, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto, LookupGroupDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

const VAT_RATE_GROUP_NAME = 'VatRate';

/** Lookup code → i18n key suffix (lookups.names.<Group>.<suffix>) for codes that are not plain alphanumeric. */
const LOOKUP_CODE_KEY_SUFFIX: Record<string, string> = {
  'yarı mamul': 'yari_mamul',
  'ticari mal': 'ticari_mal',
  'demirbaş': 'demirbas',
  'm²': 'm2',
  'm³': 'm3'
};

@Component({
  selector: 'app-lookup-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './lookup-list.component.html',
  styleUrls: ['./lookup-list.component.scss']
})
export class LookupListComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  rawLookups = signal<LookupDto[]>([]);
  groups = signal<LookupGroupDto[]>([]);
  showForm = false;
  saving = false;
  editId: string | null = null;
  /** KDV satırı düzenlenirken grup/sıra/durum kilitli; kod + renk (görünen ad koddan türetilir). */
  editingVatRate = false;
  model: Partial<LookupDto> = {};

  constructor(
    private service: LookupService,
    private toastr: ToastrService,
    private translate: TranslateService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.refresh();
    this.service.loadGroups().subscribe(g => this.groups.set(g));
    this.translate.onLangChange.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.cdr.markForCheck());
  }

  /** i18n key segment for lookup code (matches JSON under lookups.names.<Group>.<segment>). */
  static lookupCodeKeySuffix(code: string | undefined | null): string {
    const c = (code ?? '').trim();
    return LOOKUP_CODE_KEY_SUFFIX[c] ?? c;
  }

  /** Seed / sistem lookup adları: EN’de çeviri, bilinmeyen veya özel kayıtta veritabanı adı. */
  displayLookupName(item: LookupDto): string {
    const g = item.group?.name;
    if (!g) return item.name;
    const suffix = LookupListComponent.lookupCodeKeySuffix(item.code);
    const key = `lookups.names.${g}.${suffix}`;
    const t = this.translate.instant(key);
    return t === key ? item.name : t;
  }

  /** Grup/kategori adı: lookups.groups.<GroupName>; yoksa displayName/name fallback. */
  displayLookupGroup(group: LookupGroupDto | undefined): string {
    if (!group) return '';
    const key = `lookups.groups.${group.name}`;
    const t = this.translate.instant(key);
    return t === key ? (group.displayName || group.name) : t;
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
    return c.length > 0 ? `%${c}` : this.translate.instant('lookupsPage.vatPreviewEmpty');
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
      this.toastr.warning(this.translate.instant('lookupsPage.toasts.noGroupToAdd'));
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
      this.toastr.error(this.translate.instant('lookupsPage.toasts.selectGroup'));
      return;
    }
    if (this.isVatRateForm()) {
      this.syncVatRateDisplayName();
      if (!(this.model.code ?? '').toString().trim()) {
        this.toastr.error(this.translate.instant('lookupsPage.toasts.vatCodeRequired'));
        return;
      }
    } else if (!(this.model.name ?? '').toString().trim()) {
      this.toastr.error(this.translate.instant('lookupsPage.toasts.displayNameRequired'));
      return;
    }
    this.saving = true;
    const obs = this.editId
      ? this.service.update(this.editId, this.model)
      : this.service.create(this.model);

    obs.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('lookupsPage.toasts.saved'));
        this.showForm = false;
        this.saving = false;
        this.refresh();
      },
      error: (e: unknown) => {
        const err = e as { error?: { message?: string } };
        this.toastr.error(err?.error?.message ?? this.translate.instant('lookupsPage.toasts.saveError'));
        this.saving = false;
      }
    });
  }

  delete(item: LookupDto) {
    if (!confirm(this.translate.instant('lookupsPage.confirmDelete', { name: this.displayLookupName(item) }))) return;
    this.service.delete(item.id).subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('lookupsPage.toasts.deleted'));
        this.refresh();
      },
      error: (e: unknown) => {
        const err = e as { error?: { message?: string } };
        this.toastr.error(err?.error?.message ?? this.translate.instant('lookupsPage.toasts.deleteError'));
      }
    });
  }
}
