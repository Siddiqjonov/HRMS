import { Component, inject } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog, MatDialogRef, MatDialogModule, MatDialogContent, MatDialogActions } from '@angular/material/dialog';
import { ScheduleResponse } from '../../../core/models/response-models/schedule-response.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { GetScheduleByIdRequest } from '../../../core/models/request-models/schedule-request.model';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ScheduleDetailsDialogComponent } from '../schedule-details-dialog/schedule-details-dialog.component';

@Component({
  selector: 'app-schedule-selection-dialog',
  imports: [CommonModule, MatDialogModule, MatDialogContent, MatDialogActions, MatProgressSpinner, MatIcon, MatButton, EmptyStateComponent],
  templateUrl: './schedule-selection-dialog.html',
  styleUrl: './schedule-selection-dialog.css'
})
export class ScheduleSelectionDialogComponent {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<ScheduleSelectionDialogComponent>);
  schedules: ScheduleResponse[] = [];
  selectedSchedule: ScheduleResponse | null = null;
  loading = true;

  ngOnInit(): void {
    this.loadSchedules();
  }

  private loadSchedules(): void {
    this.loading = true;
    this.apiService.getSchedulesWithPagination({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (response: PaginatedModel<ScheduleResponse>) => {
        this.schedules = response.items;
        this.loading = false;
      },
      error: (error) => {
        // Don't show error toast for "not found" - just show empty state
        this.schedules = [];
        this.loading = false;
      }
    });
  }

  selectSchedule(schedule: ScheduleResponse): void {
    this.selectedSchedule = schedule;
  }

  onConfirm(): void {
    if (this.selectedSchedule) {
      this.dialogRef.close(this.selectedSchedule);
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  viewScheduleDetails(schedule: ScheduleResponse, event: Event): void {
    event.stopPropagation();
    
    this.dialog.open(ScheduleDetailsDialogComponent, {
      data: schedule,
      width: '500px'
    });
  }
}
