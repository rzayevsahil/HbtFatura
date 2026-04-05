import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { FirmService } from '../../services/firm.service';
import { FirmDto } from '../../core/models';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-firm-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, TranslateModule],
  templateUrl: './firm-list.component.html',
  styleUrls: ['./firm-list.component.scss']
})
export class FirmListComponent implements OnInit {
  items: FirmDto[] = [];
  search = '';
  loading = true;

  constructor(private api: FirmService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getAll().subscribe({
      next: (list) => {
        this.items = list;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  get filteredItems(): FirmDto[] {
    const q = this.search?.trim().toLowerCase() || '';
    if (!q) return this.items;
    return this.items.filter((f) => f.name?.toLowerCase().includes(q));
  }
}
