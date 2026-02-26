import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EmployeeService, EmployeeListDto } from '../../services/employee.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})
export class EmployeeListComponent implements OnInit {
  items: EmployeeListDto[] = [];
  search = '';
  loading = false;

  constructor(private api: EmployeeService) { }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.api.getAll().subscribe({
      next: list => {
        this.items = list;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  get filteredItems(): EmployeeListDto[] {
    const q = this.search?.trim().toLowerCase() || '';
    if (!q) return this.items;
    return this.items.filter(
      e =>
        e.fullName?.toLowerCase().includes(q) ||
        e.email?.toLowerCase().includes(q)
    );
  }
}
