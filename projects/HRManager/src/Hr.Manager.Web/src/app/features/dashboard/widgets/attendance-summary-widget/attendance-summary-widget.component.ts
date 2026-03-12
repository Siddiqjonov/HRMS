import { Component, Input, OnInit, OnChanges, SimpleChanges, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { GetAttendanceRecordsRequest } from '../../../../core/models/request-models/attendance-request.models';
import { GetAttendanceRecordsResponse } from '../../../../core/models/response-models/attendance-response.model';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

interface AttendanceSummary {
  totalDays: number;
  lateDays: number;
  earlyDepartures: number;
  totalMinutes: number;
  overtimeMinutes: number;
  averageMinutes: number;
}

@Component({
  selector: 'app-attendance-summary-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, WidgetSkeletonComponent],
  templateUrl: './attendance-summary-widget.component.html',
  styleUrl: './attendance-summary-widget.component.css'
})
export class AttendanceSummaryWidgetComponent implements OnInit, OnChanges, OnDestroy {
  @Input() employeeId: string | null = null;
  
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private refreshSubscription?: Subscription;

  summary: AttendanceSummary = {
    totalDays: 0,
    lateDays: 0,
    earlyDepartures: 0,
    totalMinutes: 0,
    overtimeMinutes: 0,
    averageMinutes: 0
  };

  isLoading = false;

  ngOnInit(): void {
    this.loadAttendanceSummary();
    
    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'attendance-summary') {
        this.loadAttendanceSummary();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employeeId'] && this.employeeId) {
      this.loadAttendanceSummary();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadAttendanceSummary(): void {
    if (!this.employeeId) return;

    this.isLoading = true;
    const { startDate, endDate, totalDays } = this.getCurrentMonthRange();

    // Build request with only non-null values
    const request: any = {
      pageNumber: 1,
      pageSize: 100, // Get all records for the month
      employeeId: this.employeeId
    };

    this.apiService.getAttendanceRecordsWithFilter(request).subscribe({
      next: (response) => {
        this.calculateSummary(response.items, startDate, endDate, totalDays);
        this.isLoading = false;
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  calculateSummary(records: GetAttendanceRecordsResponse[], startDate: Date, endDate: Date, totalDays: number): void {
    // Filter records for current month
    const currentMonthRecords = records.filter(record => {
      const recordDate = new Date(record.date);
      return recordDate >= startDate && recordDate <= endDate;
    });

    const presentDays = currentMonthRecords.length;
    const lateDays = currentMonthRecords.filter(r => r.isLate).length;
    const earlyDepartures = currentMonthRecords.filter(r => r.isEarlyDeparture).length;
    const totalMinutes = currentMonthRecords.reduce((sum, r) => sum + (r.totalMinutes || 0), 0);
    const overtimeMinutes = currentMonthRecords.reduce((sum, r) => sum + (r.overtimeMinutes || 0), 0);
    const averageMinutes = presentDays > 0 ? Math.round(totalMinutes / presentDays) : 0;

    this.summary = {
      totalDays: totalDays,
      lateDays: lateDays,
      earlyDepartures: earlyDepartures,
      totalMinutes: totalMinutes,
      overtimeMinutes: overtimeMinutes,
      averageMinutes: averageMinutes
    };
  }

  getCurrentMonthRange(): { startDate: Date; endDate: Date; totalDays: number } {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth();
    
    // First day of current month
    const startDate = new Date(year, month, 1);
    
    // Last day of current month
    const endDate = new Date(year, month + 1, 0);
    
    // Calculate working days (excluding weekends)
    const totalDays = this.getWorkingDaysInMonth(year, month);
    
    return { startDate, endDate, totalDays };
  }

  getWorkingDaysInMonth(year: number, month: number): number {
    const lastDay = new Date(year, month + 1, 0).getDate();
    let workingDays = 0;
    
    for (let day = 1; day <= lastDay; day++) {
      const date = new Date(year, month, day);
      const dayOfWeek = date.getDay();
      // Count only weekdays (Monday to Friday)
      if (dayOfWeek !== 0 && dayOfWeek !== 6) {
        workingDays++;
      }
    }
    
    return workingDays;
  }

  formatMinutes(minutes: number | null | undefined): string {
    if (minutes === null || minutes === undefined) return '-';
    
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (hours > 0 && remainingMinutes > 0) {
      return `${hours}h ${remainingMinutes}m`;
    } else if (hours > 0) {
      return `${hours}h`;
    } else {
      return `${remainingMinutes}m`;
    }
  }
}

