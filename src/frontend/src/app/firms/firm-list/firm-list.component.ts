import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FirmService } from '../../services/firm.service';
import { FirmDto } from '../../core/models';

@Component({
  selector: 'app-firm-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './firm-list.component.html',
  styleUrls: ['./firm-list.component.scss']
})
export class FirmListComponent implements OnInit {
  items: FirmDto[] = [];
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
}
