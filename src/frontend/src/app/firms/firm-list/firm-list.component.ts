import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FirmService, FirmDto } from '../../services/firm.service';

@Component({
  selector: 'app-firm-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './firm-list.component.html',
  styleUrls: ['./firm-list.component.scss']
})
export class FirmListComponent implements OnInit {
  items: FirmDto[] = [];

  constructor(private api: FirmService) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getAll().subscribe(list => (this.items = list));
  }
}
