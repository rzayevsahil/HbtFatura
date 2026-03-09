import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LookupService } from '../../core/services/lookup.service';
import { LookupDto, LookupGroupDto } from '../../core/models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-lookup-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page-header">
      <div class="header-content">
        <h1 class="page-title">Sistem Tanımları <span class="subtitle">(KeyValues)</span></h1>
        <p class="page-description">Sistem genelindeki açılır menü, durum ve tip tanımlarını buradan yönetebilirsiniz.</p>
      </div>
      <button class="btn btn-primary" (click)="openAdd()">
        <span class="icon">+</span> Yeni Tanım Ekle
      </button>
    </div>

    <div class="card table-card">
      <div class="table-container">
        <table>
          <thead>
            <tr>
              <th>Grup / Kategori</th>
              <th style="width: 100px">Kod</th>
              <th>Ad (Değer)</th>
              <th style="width: 140px">Görsel / Renk</th>
              <th style="text-align: center; width: 80px">Sıra</th>
              <th style="width: 100px">Durum</th>
              <th style="text-align: right; width: 160px">İşlemler</th>
            </tr>
          </thead>
          <tbody>
            @for (item of rawLookups(); track item.id; let index = $index) {
              <tr [class.group-start]="isNewGroup(item, index)">
                <td class="group-cell">
                  @if (isNewGroup(item, index)) {
                    <div class="group-info">
                      <span class="group-badge">{{ item.group?.displayName }}</span>
                      <small class="group-name">{{ item.group?.name }}</small>
                    </div>
                  }
                </td>
                <td><code class="code-badge">{{ item.code }}</code></td>
                <td class="name-cell">{{ item.name }}</td>
                <td>
                  <div class="color-item">
                    @if (item.color) {
                      <span class="color-preview" [style.backgroundColor]="item.color"></span>
                      <span class="color-code">{{ item.color }}</span>
                    } @else {
                      <span class="no-color">—</span>
                    }
                  </div>
                </td>
                <td style="text-align: center">{{ item.sortOrder }}</td>
                <td>
                  <span class="badge" [class.badge-success]="item.isActive" [class.badge-danger]="!item.isActive">
                    {{ item.isActive ? 'Aktif' : 'Pasif' }}
                  </span>
                </td>
                <td style="text-align: right">
                  <div class="actions">
                    <button class="btn-action edit" (click)="openEdit(item)" title="Düzenle">Düzenle</button>
                    <button class="btn-action delete" (click)="delete(item)" title="Sil">Sil</button>
                  </div>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Form -->
    @if (showForm) {
      <div class="modal-overlay">
        <div class="modal-card" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>{{ editId ? 'Tanım Düzenle' : 'Yeni Tanım Ekle' }}</h3>
            <button class="modal-close" (click)="showForm = false">×</button>
          </div>
          <form (ngSubmit)="save()">
            <div class="modal-body">
              <div class="form-grid">
                <div class="form-group full">
                  <label>Grup / Kategori</label>
                  <select [(ngModel)]="model.lookupGroupId" name="lookupGroupId" required>
                    <option [value]="undefined" disabled>Grup seçiniz...</option>
                    @for (g of groups(); track g.id) {
                      <option [value]="g.id">{{ g.displayName }} ({{ g.name }})</option>
                    }
                  </select>
                  <small>Hangi kategoriye ait olduğunu belirler.</small>
                </div>
                <div class="form-group">
                  <label>Kod (Value)</label>
                  <input type="text" [(ngModel)]="model.code" name="code" required placeholder="0, 1 veya StatusKey" />
                </div>
                <div class="form-group">
                  <label>Görünen Ad</label>
                  <input type="text" [(ngModel)]="model.name" name="name" required placeholder="Ekranda görünecek metin" />
                </div>
                <div class="form-group">
                  <label>Renk</label>
                  <div class="color-input-wrap">
                    <input type="color" [value]="model.color || '#000000'" (input)="model.color = $any($event.target).value" />
                    <input type="text" [(ngModel)]="model.color" name="color" placeholder="#hex" />
                  </div>
                </div>
                <div class="form-group">
                  <label>Sıralama</label>
                  <input type="number" [(ngModel)]="model.sortOrder" name="sortOrder" />
                </div>
                <div class="form-group full checkbox-group">
                  <label class="switch">
                    <input type="checkbox" [(ngModel)]="model.isActive" name="isActive" />
                    <span class="slider"></span>
                  </label>
                  <span>Tanım sistemde aktif olarak kullanılsın</span>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showForm = false">Vazgeç</button>
              <button type="submit" class="btn btn-primary" [disabled]="saving">
                {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
  styles: [`
    .header-content { flex: 1; }
    .subtitle { font-weight: 300; opacity: 0.7; font-size: 0.9em; }
    .page-description { color: #666; margin-top: 4px; font-size: 0.95rem; }
    
    .table-card { padding: 0; overflow: hidden; border: none; box-shadow: 0 10px 30px rgba(0,0,0,0.05); }
    .table-container { overflow-x: auto; }
    
    table { width: 100%; border-collapse: separate; border-spacing: 0; }
    th { background: #f8f9fa; padding: 16px 20px; font-weight: 600; text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.05em; color: #555; border-bottom: 2px solid #eee; text-align: left; }
    td { padding: 14px 20px; vertical-align: middle; border-bottom: 1px solid #f0f0f0; color: #333; font-size: 0.95rem; }
    
    .group-start { border-top: 1px solid #eee; }
    .group-info { display: flex; flex-direction: column; gap: 2px; }
    .group-badge { background: #e9ecef; color: #495057; padding: 4px 10px; border-radius: 6px; font-size: 0.8rem; font-weight: 700; width: fit-content; }
    .group-name { color: #999; font-size: 0.7rem; margin-left: 4px; }
    
    .code-badge { background: #f1f3f5; color: #0d6efd; padding: 2px 6px; border-radius: 4px; font-family: 'Monaco', 'Consolas', monospace; font-size: 0.85rem; }
    .name-cell { font-weight: 500; }
    
    .color-item { display: flex; align-items: center; gap: 8px; }
    .color-preview { width: 24px; height: 24px; border-radius: 50%; border: 2px solid #fff; box-shadow: 0 0 0 1px #ddd; }
    .color-code { font-size: 0.85rem; color: #777; font-family: monospace; }
    .no-color { color: #ccc; }
    
    .badge { padding: 4px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 600; display: inline-block; }
    .badge-success { background: #e6fcf5; color: #0ca678; }
    .badge-danger { background: #fff5f5; color: #fa5252; }
    
    .actions { display: flex; justify-content: flex-end; gap: 8px; }
    .btn-action { padding: 6px 14px; border-radius: 6px; font-size: 0.85rem; font-weight: 600; cursor: pointer; border: none; transition: all 0.2s; }
    .btn-action.edit { background: #f1f3f5; color: #495057; }
    .btn-action.edit:hover { background: #e9ecef; color: #111; }
    .btn-action.delete { background: #fff5f5; color: #fa5252; }
    .btn-action.delete:hover { background: #fa5252; color: #fff; }
    
    /* Modal Styling */
    .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.4); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 2000; padding: 20px; }
    .modal-card { background: #fff; width: 100%; max-width: 500px; border-radius: 16px; box-shadow: 0 20px 50px rgba(0,0,0,0.2); animation: slideUp 0.3s ease-out; }
    @keyframes slideUp { from { transform: translateY(20px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
    
    .modal-header { padding: 24px; border-bottom: 1px solid #f0f0f0; display: flex; justify-content: space-between; align-items: center; }
    .modal-header h3 { margin: 0; font-size: 1.25rem; color: #111; }
    .modal-close { background: none; border: none; font-size: 28px; color: #999; cursor: pointer; transition: color 0.2s; }
    .modal-close:hover { color: #333; }
    
    .modal-body { padding: 24px; }
    .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
    .form-group.full { grid-column: span 2; }
    .form-group label { display: block; margin-bottom: 8px; font-weight: 600; font-size: 0.85rem; color: #555; }
    .form-group input:not([type="checkbox"]), .form-group select { width: 100%; padding: 10px 14px; border: 1.5px solid #eee; border-radius: 8px; font-size: 0.95rem; transition: border-color 0.2s; }
    .form-group input:focus, .form-group select:focus { outline: none; border-color: #0ca678; }
    .form-group small { display: block; margin-top: 6px; color: #999; font-size: 0.8rem; }
    
    .color-input-wrap { display: flex; gap: 8px; align-items: center; }
    .color-input-wrap input[type="color"] { width: 44px; height: 44px; padding: 0; border: none; border-radius: 8px; overflow: hidden; cursor: pointer; }
    
    .checkbox-group { display: flex; align-items: center; gap: 12px; padding-top: 10px; }
    .switch { position: relative; display: inline-block; width: 44px; height: 24px; }
    .switch input { opacity: 0; width: 0; height: 0; }
    .slider { position: absolute; cursor: pointer; inset: 0; background-color: #dee2e6; transition: .4s; border-radius: 24px; }
    .slider:before { position: absolute; content: ""; height: 18px; width: 18px; left: 3px; bottom: 3px; background-color: white; transition: .4s; border-radius: 50%; }
    input:checked + .slider { background-color: #0ca678; }
    input:checked + .slider:before { transform: translateX(20px); }
    
    .modal-footer { padding: 20px 24px; border-top: 1px solid #f0f0f0; display: flex; justify-content: flex-end; gap: 12px; }
    .btn-secondary { background: #f8f9fa; color: #495057; border: 1.5px solid #eee; }
    .btn-secondary:hover { background: #e9ecef; }
  `]
})
export class LookupListComponent implements OnInit {
  rawLookups = signal<LookupDto[]>([]);
  groups = signal<LookupGroupDto[]>([]);
  showForm = false;
  saving = false;
  editId: string | null = null;
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

  openAdd() {
    this.editId = null;
    this.model = { isActive: true, sortOrder: 0 };
    this.showForm = true;
  }

  openEdit(item: LookupDto) {
    this.editId = item.id;
    this.model = { ...item };
    this.showForm = true;
  }

  save() {
    if (!this.model.lookupGroupId) {
      this.toastr.error('Lütfen bir grup seçiniz');
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
      error: () => {
        this.toastr.error('Hata oluştu');
        this.saving = false;
      }
    });
  }

  delete(item: LookupDto) {
    if (!confirm(`"${item.name}" tanımını silmek istediğinize emin misiniz?`)) return;
    this.service.delete(item.id).subscribe(() => {
      this.toastr.success('Silindi');
      this.refresh();
    });
  }
}
