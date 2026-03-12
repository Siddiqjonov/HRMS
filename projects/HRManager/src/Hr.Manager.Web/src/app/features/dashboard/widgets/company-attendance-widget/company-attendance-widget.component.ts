import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { CompanyAttendance } from '../../../../core/models/response-models/employee-overview-response.model';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-company-attendance-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './company-attendance-widget.component.html',
  styleUrl: './company-attendance-widget.component.css'
})
export class CompanyAttendanceWidgetComponent implements OnInit, OnDestroy {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  attendance: CompanyAttendance = {
    totalEmployees: 0,
    present: 0,
    absent: 0,
    onLeave: 0,
    late: 0,
    remoteWorking: 0
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
      if (widget === 'all' || widget === 'company-attendance') {
        this.loadData();
      }
    });
  }

  private loadData(): void {
    this.isLoading = true;
    this.apiService.getEmployeeOverview().subscribe({
      next: (response) => {
        this.attendance = response.attendance;
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationService.handleError(error);
        this.isLoading = false;
      }
    });
  }

  get attendanceRate(): number {
    if (this.attendance.totalEmployees === 0) return 0;
    return Math.round((this.attendance.present / this.attendance.totalEmployees) * 100);
  }

  get presentPercentage(): number {
    if (this.attendance.totalEmployees === 0) return 0;
    return (this.attendance.present / this.attendance.totalEmployees) * 100;
  }

  get absentPercentage(): number {
    if (this.attendance.totalEmployees === 0) return 0;
    return (this.attendance.absent / this.attendance.totalEmployees) * 100;
  }

  get leavePercentage(): number {
    if (this.attendance.totalEmployees === 0) return 0;
    return (this.attendance.onLeave / this.attendance.totalEmployees) * 100;
  }

  get remotePercentage(): number {
    if (this.attendance.totalEmployees === 0) return 0;
    return (this.attendance.remoteWorking / this.attendance.totalEmployees) * 100;
  }

  // Getters to ensure non-negative values
  get safePresent(): number {
    return Math.max(0, this.attendance.present);
  }

  get safeAbsent(): number {
    return Math.max(0, this.attendance.absent);
  }

  get safeOnLeave(): number {
    return Math.max(0, this.attendance.onLeave);
  }

  get safeLate(): number {
    return Math.max(0, this.attendance.late);
  }

  get safeRemoteWorking(): number {
    return Math.max(0, this.attendance.remoteWorking);
  }
}

