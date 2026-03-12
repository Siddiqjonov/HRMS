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
import { GetAttendanceRecordsRequest, CorrectAttendanceRecordRequest } from '../../../core/models/request-models/attendance-request.models';
import { NotificationService } from '../../../core/services/notification.service';
import { AttendanceCorrectionDialogComponent } from '../attendance-correction-dialog/attendance-correction-dialog.component';
import { CommonModule } from '@angular/common';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { EmployeeDetailsDialogComponent } from '../../employees/employee-details-dialog/employee-details-dialog.component';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
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
  selector: 'app-attendance-list',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorDisplayComponent,
    EmptyStateComponent,
    DataTableComponent,
    MatButtonModule,
    MatIconModule,
    CommonModule
  ],
  templateUrl: './attendance-list.component.html',
  styleUrl: './attendance-list.component.css'
})
export class AttendanceListComponent implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  dataSource = new MatTableDataSource<AttendanceRecordDisplay>([]);
  displayedColumns = [
    { key: 'employeeName', label: 'Employee Name' },
    { key: 'dateDisplay', label: 'Date' },
    { key: 'checkInDisplay', label: 'Check In' },
    { key: 'checkOutDisplay', label: 'Check Out' },
    { key: 'totalMinutesDisplay', label: 'Total Time' },
    { key: 'overtimeMinutesDisplay', label: 'Overtime' },
    { key: 'isLateDisplay', label: 'Arrival' },
    { key: 'isEarlyDepartureDisplay', label: 'Departure' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<GetAttendanceRecordsResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  // Filter properties - matching GetAttendanceRecordsRequest interface
  selectedEmployeeId: string | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;
  arrivalStatus: 'all' | 'onTime' | 'late' = 'all';
  departureStatus: 'all' | 'onTime' | 'early' = 'all';

  // For employee selection dialog
  employees: EmployeesBriefResponse[] = [];
  selectedEmployee: EmployeesBriefResponse | null = null;

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData(): void {
    const request: GetAttendanceRecordsRequest = { pageNumber: 1, pageSize: 10 };
    this.loadAttendanceRecords(request);
  }

  onPageChange(event: PageEvent) {
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

    // Build request object, filtering out null and undefined values
    const request: any = {
      pageNumber: 1, // Always reset to first page when building filter request
      pageSize: this.paginationResult?.pageSize || 10
    };

    // Only include properties that are not null or undefined
    if (this.selectedEmployeeId) {
      request.employeeId = this.selectedEmployeeId;
    }
    if (this.startDate) {
      request.startDate = this.formatDateForRequest(this.startDate);
    }
    if (this.endDate) {
      request.endDate = this.formatDateForRequest(this.endDate);
    }
    // Include isLate when it's explicitly set (including false)
    if (isLate !== null && isLate !== undefined) {
      request.isLate = isLate;
    }
    // Include isEarlyDeparture when it's explicitly set (including false)
    if (isEarlyDeparture !== null && isEarlyDeparture !== undefined) {
      request.isEarlyDeparture = isEarlyDeparture;
    }

    return request as GetAttendanceRecordsRequest;
  }

  formatDateForRequest(date: Date): string {
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
        // Format data for display
        const displayRecords = res.items.map(record => {
          return {
            ...record, // Keep original values for API calls including employeeName
            // Format for display
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
    const request = this.buildFilterRequest();
    request.pageNumber = 1; // Reset to first page when filtering
    this.loadAttendanceRecords(request);
  }

  onClearFilters(): void {
    this.selectedEmployeeId = null;
    this.startDate = null;
    this.endDate = null;
    this.arrivalStatus = 'all';
    this.departureStatus = 'all';
    this.selectedEmployee = null;
    this.onFilterChange();
  }

  openFilterDialog(): void {
    const filterData: AttendanceFilterData = {
      selectedEmployeeId: this.selectedEmployeeId,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      arrivalStatus: this.arrivalStatus,
      departureStatus: this.departureStatus,
      selectedEmployee: this.selectedEmployee
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
        this.selectedEmployeeId = result.selectedEmployeeId || null;
        this.startDate = result.startDate ? new Date(result.startDate) : null;
        this.endDate = result.endDate ? new Date(result.endDate) : null;
        this.arrivalStatus = result.arrivalStatus || 'all';
        this.departureStatus = result.departureStatus || 'all';
        this.selectedEmployee = result.selectedEmployee || null;

        // Load filtered data
        this.onFilterChange();
      }
    });
  }

  onViewEmployee(record: AttendanceRecordDisplay): void {
    const request: GetEmployeeByIdRequest = { id: record.employeeId };
    
    this.apiService.getEmployeeById(request).subscribe({
      next: (employee) => {
        this.dialog.open(EmployeeDetailsDialogComponent, {
          data: employee,
          width: '500px',
          maxWidth: '90vw'
        });
      },
      error: (err) => {
        this.notificationService.handleError(err);
      }
    });
  }

  onCorrectAttendance(record: AttendanceRecordDisplay): void {
    const dialogRef = this.dialog.open(AttendanceCorrectionDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: record
    });

    dialogRef.afterClosed().subscribe((result: CorrectAttendanceRecordRequest | null) => {
      if (result) {
        this.apiService.correctAttendance(result).subscribe({
          next: () => {
            this.notificationService.success('Attendance record corrected successfully');
            const request = this.buildFilterRequest();
            this.loadAttendanceRecords(request);
          },
          error: (err) => {
            this.notificationService.handleError(err);
          }
        });
      }
    });
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
