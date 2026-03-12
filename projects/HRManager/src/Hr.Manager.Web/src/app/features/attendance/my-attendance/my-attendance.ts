import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MatTableDataSource } from '@angular/material/table';
import { GetAttendanceRecordsResponse } from '../../../core/models/response-models/attendance-response.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../../shared/components/error-display/error-display.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { GetAttendanceRecordsRequest } from '../../../core/models/request-models/attendance-request.models';
import { NotificationService } from '../../../core/services/notification.service';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { GetEmployeeByEmailRequest } from '../../../core/models/request-models/employee-request.model';
import { AttendanceFilterDialogComponent, AttendanceFilterData } from '../attendance-filter-dialog/attendance-filter-dialog.component';

interface AttendanceRecordDisplay extends GetAttendanceRecordsResponse {
  dateDisplay?: string;
  checkInDisplay?: string;
  checkOutDisplay?: string;
  totalMinutesDisplay?: string;
  overtimeMinutesDisplay?: string;
  isLateDisplay?: string;
  isEarlyDepartureDisplay?: string;
}

@Component({
  selector: 'app-my-attendance',
  imports: [
    LoadingSpinnerComponent,
    ErrorDisplayComponent,
    EmptyStateComponent,
    DataTableComponent,
    MatButtonModule,
    MatIconModule,
    CommonModule
  ],
  templateUrl: './my-attendance.html',
  styleUrl: './my-attendance.css'
})
export class MyAttendance implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthenticationService);

  dataSource = new MatTableDataSource<AttendanceRecordDisplay>([]);
  displayedColumns = [
    { key: 'dateDisplay', label: 'Date' },
    { key: 'checkInDisplay', label: 'Check In' },
    { key: 'checkOutDisplay', label: 'Check Out' },
    { key: 'totalMinutesDisplay', label: 'Total Time' },
    { key: 'overtimeMinutesDisplay', label: 'Overtime' },
    { key: 'isLateDisplay', label: 'Arrival' },
    { key: 'isEarlyDepartureDisplay', label: 'Departure' }
  ];

  paginationResult: PaginatedModel<GetAttendanceRecordsResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  employeeId: string | null = null;

  // Filter properties - matching GetAttendanceRecordsRequest interface
  startDate: Date | null = null;
  endDate: Date | null = null;
  arrivalStatus: 'all' | 'onTime' | 'late' = 'all';
  departureStatus: 'all' | 'onTime' | 'early' = 'all';

  ngOnInit(): void {
    this.loadCurrentUserEmployeeId();
  }

  loadCurrentUserEmployeeId(): void {
    const userEmail = this.authService.userEmail;
    if (!userEmail) {
      this.notificationService.warning('Unable to get user email. Please try logging in again.');
      return;
    }

    const request: GetEmployeeByEmailRequest = { email: userEmail };
    this.apiService.getEmployeeByEmail(request).subscribe({
      next: (employee) => {
        this.employeeId = employee.id;
        this.loadInitialData();
      },
      error: (error) => {
        this.notificationService.handleError(error);
        this.errorMessage = 'Failed to load employee information';
      }
    });
  }

  loadInitialData(): void {
    if (!this.employeeId) return;
    const request: GetAttendanceRecordsRequest = { 
      pageNumber: 1, 
      pageSize: 10,
      employeeId: this.employeeId
    };
    this.loadAttendanceRecords(request);
  }

  onPageChange(event: PageEvent) {
    if (!this.employeeId) return;
    const request = this.buildFilterRequest();
    request.pageNumber = event.pageIndex + 1;
    request.pageSize = event.pageSize;
    this.loadAttendanceRecords(request);
  }

  buildFilterRequest(): GetAttendanceRecordsRequest {
    // Map arrival and departure status to isLate and isEarlyDeparture
    let isLate: boolean | null = null;
    let isEarlyDeparture: boolean | null = null;

    // Handle Arrival Status
    switch (this.arrivalStatus) {
      case 'onTime':
        isLate = false;
        break;
      case 'late':
        isLate = true;
        break;
      case 'all':
      default:
        isLate = null;
        break;
    }

    // Handle Departure Status
    switch (this.departureStatus) {
      case 'onTime':
        isEarlyDeparture = false;
        break;
      case 'early':
        isEarlyDeparture = true;
        break;
      case 'all':
      default:
        isEarlyDeparture = null;
        break;
    }

    const request: any = {
      pageNumber: 1,
      pageSize: this.paginationResult?.pageSize || 10,
      employeeId: this.employeeId!
    };

    if (this.startDate) {
      request.startDate = this.formatDateForRequest(this.startDate);
    }
    if (this.endDate) {
      request.endDate = this.formatDateForRequest(this.endDate);
    }
    if (isLate !== null && isLate !== undefined) {
      request.isLate = isLate;
    }
    if (isEarlyDeparture !== null && isEarlyDeparture !== undefined) {
      request.isEarlyDeparture = isEarlyDeparture;
    }

    return request as GetAttendanceRecordsRequest;
  }

  formatDateForRequest(date: Date | null): string {
    if (!date) return '';
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  loadAttendanceRecords(request: GetAttendanceRecordsRequest): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.cdr.markForCheck();

    this.apiService.getAttendanceRecordsWithFilter(request).subscribe({
      next: (res: PaginatedModel<GetAttendanceRecordsResponse>) => {
        const displayRecords = res.items.map(record => {
          return {
            ...record,
            dateDisplay: record.date ? this.formatDateDisplay(record.date) : '-',
            checkInDisplay: this.formatTime(record.checkIn),
            checkOutDisplay: this.formatTime(record.checkOut),
            totalMinutesDisplay: this.formatMinutes(record.totalMinutes),
            overtimeMinutesDisplay: this.formatMinutes(record.overtimeMinutes),
            isLateDisplay: this.formatLateStatus(record.isLate),
            isEarlyDepartureDisplay: this.formatEarlyDepartureStatus(record.isEarlyDeparture)
          } as AttendanceRecordDisplay;
        });

        this.dataSource.data = displayRecords;
        this.paginationResult = res;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err: any) => {
        this.notificationService.handleError(err);
        this.errorMessage = 'Failed to load attendance records';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  onFilterChange(): void {
    if (!this.employeeId) return;
    const request = this.buildFilterRequest();
    request.pageNumber = 1;
    this.loadAttendanceRecords(request);
  }

  openFilterDialog(): void {
    const filterData: AttendanceFilterData = {
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      arrivalStatus: this.arrivalStatus,
      departureStatus: this.departureStatus,
      hideEmployeeSelection: true // Hide employee selection for my-attendance
    };

    const dialogRef = this.dialog.open(AttendanceFilterDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: filterData,
      disableClose: false,
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe((result: AttendanceFilterData | null) => {
      if (result) {
        // Apply filter results
        this.startDate = result.startDate ? new Date(result.startDate) : null;
        this.endDate = result.endDate ? new Date(result.endDate) : null;
        this.arrivalStatus = result.arrivalStatus || 'all';
        this.departureStatus = result.departureStatus || 'all';

        // Load filtered data
        this.onFilterChange();
      }
    });
  }

  onClearFilters(): void {
    this.startDate = null;
    this.endDate = null;
    this.arrivalStatus = 'all';
    this.departureStatus = 'all';
    this.onFilterChange();
  }


  formatDateDisplay(date: string | null | undefined): string {
    if (!date) return '-';
    return date;
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

  formatBoolean(value: boolean | null | undefined): string {
    if (value === null || value === undefined) return '-';
    return value ? 'Yes' : 'No';
  }

  formatLateStatus(value: boolean | null | undefined): string {
    if (value === null || value === undefined) return '-';
    return value ? 'Late' : 'On Time';
  }

  formatEarlyDepartureStatus(value: boolean | null | undefined): string {
    if (value === null || value === undefined) return '-';
    return value ? 'Early Departure' : 'On Time';
  }
}
