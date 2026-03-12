import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { ScheduleResponse } from '../../../core/models/response-models/schedule-response.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { PaginationRequest } from '../../../core/models/common/pagination-request.model';
import { PageEvent } from '@angular/material/paginator';
import { DeleteScheduleRequest } from '../../../core/models/request-models/schedule-request.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { DataTableComponent } from "../../../shared/components/data-table/data-table.component";
import { MatIcon } from "@angular/material/icon";
import { LoadingSpinnerComponent } from "../../../shared/components/loading-spinner/loading-spinner.component";
import { ErrorDisplayComponent } from "../../../shared/components/error-display/error-display.component";
import { EmptyStateComponent } from "../../../shared/components/empty-state/empty-state.component";
import { MatButtonModule } from '@angular/material/button';
import { NotificationService } from '../../../core/services/notification.service';
import { ScheduleDetailsDialogComponent } from '../schedule-details-dialog/schedule-details-dialog.component';
import { ScheduleEmployeesDialogComponent } from '../schedule-employees-dialog/schedule-employees-dialog.component';
import { DaysOfWeek } from '../../../core/models/enums/days-of-week.enum';

@Component({
  selector: 'app-schedule-list',
  imports: [DataTableComponent, MatIcon, LoadingSpinnerComponent, ErrorDisplayComponent, MatButtonModule, EmptyStateComponent],
  templateUrl: './schedule-list.component.html',
  styleUrl: './schedule-list.component.css'
})
export class ScheduleListComponent implements OnInit {
  private apiService = inject(ApiService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);

  dataSource = new MatTableDataSource<ScheduleResponse>([]);

  displayedColumns = [
    { key: 'name', label: 'Schedule Name' },
    { key: 'description', label: 'Description' },
    { key: 'startTime', label: 'Start Time' },
    { key: 'endTime', label: 'End Time' },
    { key: 'daysOfWeek', label: 'Days' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<ScheduleResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const request: PaginationRequest = { pageNumber: 1, pageSize: 10 }
    this.loadSchedules(request);
  }

  onPageChange(event: PageEvent) {
    const request: PaginationRequest = {
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.loadSchedules(request);
  }

  loadSchedules(request: PaginationRequest): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.apiService.getSchedulesWithPagination(request).subscribe({
      next: (response) => {
        this.paginationResult = response;
        this.dataSource.data = response.items;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load schedules. Please try again.';
        this.isLoading = false;
        this.notificationService.handleError(error);
      }
    });
  }

  formatDaysOfWeek(days: DaysOfWeek): string {
    const dayNames: string[] = [];
    
    if (days & DaysOfWeek.Monday) dayNames.push('Mon');
    if (days & DaysOfWeek.Tuesday) dayNames.push('Tue');
    if (days & DaysOfWeek.Wednesday) dayNames.push('Wed');
    if (days & DaysOfWeek.Thursday) dayNames.push('Thu');
    if (days & DaysOfWeek.Friday) dayNames.push('Fri');
    if (days & DaysOfWeek.Saturday) dayNames.push('Sat');
    if (days & DaysOfWeek.Sunday) dayNames.push('Sun');
    
    return dayNames.length > 0 ? dayNames.join(', ') : 'None';
  }

  onAddSchedule(): void {
    this.router.navigate(['/schedules/new']);
  }

  onEditSchedule(schedule: ScheduleResponse): void {
    this.router.navigate([`/schedules/${schedule.id}/edit`]);
  }

  onViewSchedule(schedule: ScheduleResponse) {
    this.dialog.open(ScheduleDetailsDialogComponent, {
      data: schedule,
      width: '500px',
    });
  }

  onDeleteSchedule(schedule: DeleteScheduleRequest) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { message: 'Are you sure you want to delete this schedule?' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deleteSchedule(schedule)
          .subscribe({
            next: () => {
              const pageNumber = this.paginationResult?.page || 1;
              const pageSize = this.paginationResult?.pageSize || 10;
              const request: PaginationRequest = { pageNumber: pageNumber, pageSize: pageSize };
              this.loadSchedules(request);
              this.notificationService.success('Schedule deleted successfully!');
            },
            error: (error) => {
              this.notificationService.handleError(error);
            }
          });
      }
    });
  }

  onViewEmployees(schedule: ScheduleResponse) {
    this.isLoading = true;
    
    this.apiService.getScheduleEmployees({ scheduleId: schedule.id }).subscribe({
      next: (employees) => {
        this.isLoading = false;
        this.dialog.open(ScheduleEmployeesDialogComponent, {
          data: {
            scheduleName: schedule.name,
            employees: employees
          },
          width: '700px',
          maxWidth: '90vw'
        });
      },
      error: (error) => {
        this.isLoading = false;
        this.notificationService.handleError(error);
      }
    });
  }
}

