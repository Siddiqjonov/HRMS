import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Router } from '@angular/router';
import { MatTableDataSource } from '@angular/material/table';
import { AbsenceRequestBriefResponse } from '../../../core/models/response-models/absence-request-request.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { PageEvent } from '@angular/material/paginator';
import { DataTableComponent } from "../../../shared/components/data-table/data-table.component";
import { MatIcon } from "@angular/material/icon";
import { LoadingSpinnerComponent } from "../../../shared/components/loading-spinner/loading-spinner.component";
import { ErrorDisplayComponent } from "../../../shared/components/error-display/error-display.component";
import { EmptyStateComponent } from "../../../shared/components/empty-state/empty-state.component";
import { MatButtonModule } from '@angular/material/button';
import { NotificationService } from '../../../core/services/notification.service';
import { RequestStatus } from '../../../core/models/enums/request-status.enum';
import { RequestType } from '../../../core/models/enums/request-type.enum';
import { GetAbsenceRequestsWithPaginationRequest } from '../../../core/models/response-models/absence-request-request.model';
import { FormControl, FormGroup } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { GetEmployeeByEmailRequest } from '../../../core/models/request-models/employee-request.model';
import { MatDialog } from '@angular/material/dialog';
import { AbsenceRequestFilterDialogComponent, AbsenceRequestFilterData } from '../absence-request-filter-dialog/absence-request-filter-dialog.component';

interface AbsenceRequestDisplayModel extends Omit<AbsenceRequestBriefResponse, 'type' | 'status'> {
  type: string;
  status: string;
}

@Component({
  selector: 'app-my-absence-list',
  imports: [
    DataTableComponent, 
    MatIcon, 
    LoadingSpinnerComponent, 
    ErrorDisplayComponent, 
    MatButtonModule, 
    EmptyStateComponent,
    CommonModule
  ],
  templateUrl: './my-absence-list.component.html',
  styleUrl: './my-absence-list.component.css'
})
export class MyAbsenceListComponent implements OnInit {
  private apiService = inject(ApiService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);
  private authService = inject(AuthenticationService);
  private dialog = inject(MatDialog);

  dataSource = new MatTableDataSource<AbsenceRequestDisplayModel>([]);

  displayedColumns = [
    { key: 'type', label: 'Type' },
    { key: 'status', label: 'Status' },
    { key: 'startDate', label: 'Start Date' },
    { key: 'endDate', label: 'End Date' },
    { key: 'daysRequested', label: 'Days' },
    { key: 'managerName', label: 'Manager' }
  ];

  paginationResult: PaginatedModel<AbsenceRequestBriefResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  employeeId: string | null = null;
  hasActiveFilters = false;

  // Filter form (no employee filter since it's always filtered to current employee)
  filterForm = new FormGroup({
    status: new FormControl<RequestStatus | string | null>(null),
    type: new FormControl<RequestType | string | null>(null),
    startDate: new FormControl<Date | null>(null),
    endDate: new FormControl<Date | null>(null)
  });

  // Enum options for dropdowns
  statusOptions = [
    { value: RequestStatus.Pending, label: 'Pending' },
    { value: RequestStatus.Approved, label: 'Approved' },
    { value: RequestStatus.Rejected, label: 'Rejected' }
  ];

  typeOptions = [
    { value: RequestType.Vacation, label: 'Vacation' },
    { value: RequestType.Sick, label: 'Sick Leave' },
    { value: RequestType.Remote, label: 'Remote Work' },
    { value: RequestType.Unpaid, label: 'Unpaid Leave' }
  ];

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
        this.applyFilters();
      },
      error: (error) => {
        this.notificationService.handleError(error);
      }
    });
  }

  onPageChange(event: PageEvent) {
    if (!this.employeeId) return;
    
    const request = this.buildFilterRequest({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    });
    this.loadAbsenceRequests(request);
  }

  openFilterDialog(): void {
    const dialogRef = this.dialog.open(AbsenceRequestFilterDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        status: this.filterForm.value.status,
        type: this.filterForm.value.type,
        startDate: this.filterForm.value.startDate ? this.formatDateForApi(this.filterForm.value.startDate) : null,
        endDate: this.filterForm.value.endDate ? this.formatDateForApi(this.filterForm.value.endDate) : null
      } as AbsenceRequestFilterData
    });

    dialogRef.afterClosed().subscribe((filterData: AbsenceRequestFilterData | null) => {
      if (filterData) {
        this.filterForm.patchValue({
          status: filterData.status !== null && filterData.status !== undefined ? filterData.status : null,
          type: filterData.type !== null && filterData.type !== undefined ? filterData.type : null,
          startDate: filterData.startDate ? new Date(filterData.startDate) : null,
          endDate: filterData.endDate ? new Date(filterData.endDate) : null
        });
        this.applyFilters();
      }
    });
  }

  applyFilters(): void {
    if (!this.employeeId) return;
    
    const request = this.buildFilterRequest({ pageNumber: 1, pageSize: 10 });
    this.loadAbsenceRequests(request);
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.hasActiveFilters = false;
    this.applyFilters();
  }

  buildFilterRequest(baseRequest: { pageNumber: number; pageSize: number }): GetAbsenceRequestsWithPaginationRequest {
    const formValue = this.filterForm.value;
    
    // Check if any filters are active (null means "All" is selected, so not a filter)
    const hasStatusFilter = formValue.status !== null && formValue.status !== undefined;
    const hasTypeFilter = formValue.type !== null && formValue.type !== undefined;
    
    this.hasActiveFilters = !!(
      hasStatusFilter ||
      hasTypeFilter ||
      formValue.startDate ||
      formValue.endDate
    );
    
    // Convert type to number if needed
    let typeValue: RequestType | null = null;
    if (hasTypeFilter) {
      const type = formValue.type;
      if (typeof type === 'string') {
        const parsed = parseInt(type, 10);
        if (!isNaN(parsed)) {
          typeValue = parsed as RequestType;
        }
      } else if (typeof type === 'number') {
        typeValue = type as RequestType;
      } else {
        typeValue = Number(type) as RequestType;
      }
    }

    // Convert status to number if needed
    let statusValue: RequestStatus | null = null;
    if (hasStatusFilter) {
      const status = formValue.status;
      if (typeof status === 'string') {
        const parsed = parseInt(status, 10);
        if (!isNaN(parsed)) {
          statusValue = parsed as RequestStatus;
        }
      } else if (typeof status === 'number') {
        statusValue = status as RequestStatus;
      } else {
        statusValue = Number(status) as RequestStatus;
      }
    }
    
    const request: GetAbsenceRequestsWithPaginationRequest = {
      ...baseRequest,
      employeeId: this.employeeId!, // Always filter by current employee
      status: statusValue,
      type: typeValue,
      startDate: formValue.startDate ? this.formatDateForApi(formValue.startDate) : null,
      endDate: formValue.endDate ? this.formatDateForApi(formValue.endDate) : null
    };

    return request;
  }

  formatDateForApi(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  loadAbsenceRequests(request: GetAbsenceRequestsWithPaginationRequest): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.apiService.getAbsenceRequestsWithPagination(request).subscribe({
      next: (response) => {
        this.paginationResult = response;
        // Format the data for display
        const formattedItems: AbsenceRequestDisplayModel[] = response.items.map(item => ({
          ...item,
          type: this.formatRequestType(item.type),
          status: this.formatRequestStatus(item.status),
          startDate: this.formatDate(item.startDate),
          endDate: this.formatDate(item.endDate),
          employeeName: item.employeeName || 'N/A'
        }));
        
        // Always set dataSource.data to the formatted items, even if empty
        // This ensures the empty state is shown when filters return no results
        this.dataSource.data = formattedItems;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load absence requests. Please try again.';
        this.isLoading = false;
        this.dataSource.data = []; // Clear data on error
        this.notificationService.handleError(error);
      }
    });
  }

  formatRequestType(type: RequestType): string {
    const typeMap: { [key in RequestType]: string } = {
      [RequestType.Vacation]: 'Vacation',
      [RequestType.Sick]: 'Sick Leave',
      [RequestType.Remote]: 'Remote Work',
      [RequestType.Unpaid]: 'Unpaid Leave'
    };
    return typeMap[type] || 'Unknown';
  }

  formatRequestStatus(status: RequestStatus): string {
    const statusMap: { [key in RequestStatus]: string } = {
      [RequestStatus.Pending]: 'Pending',
      [RequestStatus.Approved]: 'Approved',
      [RequestStatus.Rejected]: 'Rejected'
    };
    return statusMap[status] || 'Unknown';
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  onAddAbsenceRequest(): void {
    this.router.navigate(['/my-absences/new']);
  }
}

