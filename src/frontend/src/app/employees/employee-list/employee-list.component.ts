import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EmployeeService, EmployeeListDto } from '../../services/employee.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.scss']
})
export class EmployeeListComponent implements OnInit {
  items: EmployeeListDto[] = [];

  constructor(private api: EmployeeService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getAll().subscribe(list => (this.items = list));
  }
}
