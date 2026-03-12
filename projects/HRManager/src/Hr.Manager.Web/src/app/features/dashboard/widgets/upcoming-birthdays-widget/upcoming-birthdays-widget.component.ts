import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { UpcomingBirthday } from '../../../../core/models/response-models/employee-overview-response.model';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-upcoming-birthdays-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, WidgetSkeletonComponent],
  templateUrl: './upcoming-birthdays-widget.component.html',
  styleUrl: './upcoming-birthdays-widget.component.css'
})
export class UpcomingBirthdaysWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  birthdays: UpcomingBirthday[] = [];
  isLoading = false;

  ngOnInit(): void {
    this.loadData();
    this.setupRefreshListener();
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  private setupRefreshListener(): void {
    this.refreshSubscription = this.refreshService.refreshTrigger$.subscribe(widget => {
      if (widget === 'all' || widget === 'upcoming-birthdays') {
        this.loadData();
      }
    });
  }

  private loadData(): void {
    this.isLoading = true;
    this.apiService.getEmployeeOverview().subscribe({
      next: (response) => {
        this.birthdays = response.upcomingBirthdays;
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.handleError(error);
        this.isLoading = false;
      }
    });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric'
    });
  }

  isToday(dateString: string): boolean {
    const today = new Date();
    const date = new Date(dateString);
    return today.getDate() === date.getDate() && 
           today.getMonth() === date.getMonth();
  }

  isTomorrow(dateString: string): boolean {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const date = new Date(dateString);
    return tomorrow.getDate() === date.getDate() && 
           tomorrow.getMonth() === date.getMonth();
  }

  getDateLabel(dateString: string): string {
    if (this.isToday(dateString)) return 'Today';
    if (this.isTomorrow(dateString)) return 'Tomorrow';
    return this.formatDate(dateString);
  }
}

