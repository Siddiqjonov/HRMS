import { Component, inject, OnInit, OnChanges, SimpleChanges, Input, Output, EventEmitter, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { ApiService } from '../../../../core/services/api.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { DashboardRefreshService } from '../../../../core/services/dashboard-refresh.service';
import { CheckInRequest, CheckOutRequest, GetAttendanceRecordsRequest } from '../../../../core/models/request-models/attendance-request.models';
import { GetAttendanceRecordsResponse } from '../../../../core/models/response-models/attendance-response.model';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { WidgetSkeletonComponent } from '../../../../shared/components/widget-skeleton/widget-skeleton.component';

@Component({
  selector: 'app-attendance-clock-widget',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, WidgetSkeletonComponent],
  templateUrl: './attendance-clock-widget.component.html',
  styleUrl: './attendance-clock-widget.component.css'
})
export class AttendanceClockWidgetComponent implements OnInit, OnChanges, OnDestroy {
  @Input() employeeId: string | null = null;
  @Output() attendanceUpdated = new EventEmitter<void>();

  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private refreshService = inject(DashboardRefreshService);
  private cdr = inject(ChangeDetectorRef);
  private dialog = inject(MatDialog);
  private refreshSubscription?: Subscription;

  todayRecord: GetAttendanceRecordsResponse | null = null;
  isLoading = false;
  isCheckingIn = false;
  isCheckingOut = false;
  currentTime: Date = new Date();

  ngOnInit(): void {
    this.loadTodayRecord();
    // Update current time every second
    setInterval(() => {
      this.currentTime = new Date();
      this.cdr.markForCheck();
    }, 1000);

    // Subscribe to refresh events
    this.refreshSubscription = this.refreshService.refresh$.subscribe(target => {
      if (target === 'all' || target === 'attendance-clock') {
        this.loadTodayRecord();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employeeId'] && this.employeeId) {
      this.loadTodayRecord();
    }
  }

  ngOnDestroy(): void {
    this.refreshSubscription?.unsubscribe();
  }

  loadTodayRecord(): void {
    if (!this.employeeId) return;

    this.isLoading = true;
    const today = new Date();
    const todayString = this.formatDateForRequest(today);

    const request: GetAttendanceRecordsRequest = {
      pageNumber: 1,
      pageSize: 1,
      employeeId: this.employeeId,
      startDate: todayString,
      endDate: todayString
    };

    this.apiService.getAttendanceRecordsWithFilter(request).subscribe({
      next: (response) => {
        this.todayRecord = response.items.length > 0 ? response.items[0] : null;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.notificationService.handleError(err);
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onClockIn(): void {
    if (!this.employeeId || this.isCheckingIn) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        message: 'Are you sure you want to check in?'
      }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isCheckingIn = true;
        const request: CheckInRequest = { employeeId: this.employeeId! };

        this.apiService.checkIn(request).subscribe({
          next: () => {
            this.notificationService.success('Checked in successfully');
            this.loadTodayRecord();
            this.attendanceUpdated.emit();
            // Refresh all attendance-related widgets to show updated counts
            this.refreshService.triggerRefresh('team-attendance');
            this.refreshService.triggerRefresh('department-overview');
            this.refreshService.triggerRefresh('attendance-summary');
            this.isCheckingIn = false;
            this.cdr.markForCheck();
          },
          error: (err) => {
            this.notificationService.handleError(err);
            this.isCheckingIn = false;
            this.cdr.markForCheck();
          }
        });
      }
    });
  }

  onClockOut(): void {
    if (!this.employeeId || this.isCheckingOut) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        message: 'Are you sure you want to check out?'
      }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.isCheckingOut = true;
        const request: CheckOutRequest = { employeeId: this.employeeId! };

        this.apiService.checkOut(request).subscribe({
          next: () => {
            this.notificationService.success('Checked out successfully');
            this.loadTodayRecord();
            this.attendanceUpdated.emit();
            // Refresh all attendance-related widgets to show updated counts
            this.refreshService.triggerRefresh('team-attendance');
            this.refreshService.triggerRefresh('department-overview');
            this.refreshService.triggerRefresh('attendance-summary');
            this.isCheckingOut = false;
            this.cdr.markForCheck();
          },
          error: (err) => {
            this.notificationService.handleError(err);
            this.isCheckingOut = false;
            this.cdr.markForCheck();
          }
        });
      }
    });
  }

  formatDateForRequest(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  formatTime(time: string | null | undefined): string {
    if (!time) return '-';
    return time.substring(0, 5);
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

  get canCheckIn(): boolean {
    return !this.isLoading && !this.todayRecord && !this.isCheckingIn;
  }

  get canCheckOut(): boolean {
    return !this.isLoading && 
           this.todayRecord !== null && 
           !this.todayRecord.checkOut && 
           !this.isCheckingOut;
  }

  get isCompleted(): boolean {
    return this.todayRecord !== null && this.todayRecord.checkOut !== null;
  }
}

