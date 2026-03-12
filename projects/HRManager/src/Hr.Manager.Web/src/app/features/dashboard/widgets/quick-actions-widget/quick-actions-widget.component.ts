import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

interface QuickAction {
  icon: string;
  label: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-quick-actions-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule],
  templateUrl: './quick-actions-widget.component.html',
  styleUrl: './quick-actions-widget.component.css'
})
export class QuickActionsWidgetComponent {
  actions: QuickAction[] = [
    { icon: 'add_circle', label: 'Request Leave', route: '/absence-requests', color: '#4CAF50' },
    { icon: 'access_time', label: 'Clock In/Out', route: '/attendance', color: '#2196F3' },
    { icon: 'calendar_month', label: 'View Schedule', route: '/schedule', color: '#FF9800' },
    { icon: 'groups', label: 'Team', route: '/employees', color: '#9C27B0' },
  ];

  constructor(private router: Router) {}

  navigate(route: string): void {
    this.router.navigate([route]);
  }
}

