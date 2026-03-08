import { Component, computed } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { LookupService } from './core/services/lookup.service';
import { MenuService } from './core/services/menu.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  headerMenuOpen = false;

  constructor(public auth: AuthService, private lookup: LookupService, public menu: MenuService) {
    this.lookup.load().subscribe();
  }



  closeHeaderMenu(): void {
    this.headerMenuOpen = false;
  }
}
