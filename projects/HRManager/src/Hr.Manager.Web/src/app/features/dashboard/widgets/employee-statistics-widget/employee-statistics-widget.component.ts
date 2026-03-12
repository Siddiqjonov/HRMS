import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { EmployeeStatistics } from '../../../../core/models/response-models/employee-overview-response.model';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-employee-statistics-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './employee-statistics-widget.component.html',
  styleUrl: './employee-statistics-widget.component.css'
})
export class EmployeeStatisticsWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  statistics: EmployeeStatistics = {
    totalEmployees: 0,
    activeEmployees: 0,
    newHiresThisMonth: 0,
    averageTenure: 0,
    turnoverRate: 0
  };

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
      if (widget === 'all' || widget === 'employee-statistics') {
        this.loadData();
      }
    });
  }

  private loadData(): void {
    this.isLoading = true;
    this.apiService.getEmployeeOverview().subscribe({
      next: (response) => {
        this.statistics = response.statistics;
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.handleError(error);
        this.isLoading = false;
      }
    });
  }
}

