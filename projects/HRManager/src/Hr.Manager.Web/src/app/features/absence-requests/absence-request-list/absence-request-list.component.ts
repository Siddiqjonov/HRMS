import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { AbsenceRequestBriefResponse } from '../../../core/models/response-models/absence-request-request.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { PaginationRequest } from '../../../core/models/common/pagination-request.model';
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
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { AbsenceRequestFilterDialogComponent, AbsenceRequestFilterData } from '../absence-request-filter-dialog/absence-request-filter-dialog.component';
import { ProcessAbsenceRequestDialogComponent, ProcessAbsenceRequestDialogData } from '../process-absence-request-dialog/process-absence-request-dialog.component';

interface AbsenceRequestDisplayModel extends Omit<AbsenceRequestBriefResponse, 'type' | 'status' | 'startDate' | 'endDate'> {
  type: string;
  status: string;
  startDate: string; // Formatted for display
  endDate: string; // Formatted for display
  originalStatus: RequestStatus;
  originalType: RequestType;
  originalStartDate: string; // Original date string from API
  originalEndDate: string; // Original date string from API
}

@Component({
  selector: 'app-absence-request-list',
  imports: [
    DataTableComponent,
    MatIcon,
    LoadingSpinnerComponent,
    ErrorDisplayComponent,
    MatButtonModule,
    EmptyStateComponent,
    CommonModule
  ],
  templateUrl: './absence-request-list.component.html',
  styleUrl: './absence-request-list.component.css'
})
export class AbsenceRequestListComponent implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);

  dataSource = new MatTableDataSource<AbsenceRequestDisplayModel>([]);

  displayedColumns = [
    { key: 'employeeName', label: 'Employee' },
    { key: 'type', label: 'Type' },
    { key: 'status', label: 'Status' },
    { key: 'startDate', label: 'Start Date' },
    { key: 'endDate', label: 'End Date' },
    { key: 'daysRequested', label: 'Days' },
    { key: 'managerName', label: 'Manager' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<AbsenceRequestBriefResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  hasActiveFilters = false;

  // Filter form
  filterForm = new FormGroup({
    employeeId: new FormControl<string | null>(null),
    status: new FormControl<RequestStatus | string | null>(null),
    type: new FormControl<RequestType | string | null>(null),
    startDate: new FormControl<Date | null>(null),
    endDate: new FormControl<Date | null>(null)
  });

  selectedEmployee: EmployeesBriefResponse | null = null;

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
    this.applyFilters();
  }

  onPageChange(event: PageEvent) {
    const request = this.buildFilterRequest({
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    });
    this.loadAbsenceRequests(request);
  }


  applyFilters(): void {
    const request = this.buildFilterRequest({ pageNumber: 1, pageSize: 10 });
    this.loadAbsenceRequests(request);
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.selectedEmployee = null;
    this.hasActiveFilters = false;
    this.applyFilters();
  }

  buildFilterRequest(baseRequest: { pageNumber: number; pageSize: number }): GetAbsenceRequestsWithPaginationRequest {
    const formValue = this.filterForm.value;

    const hasStatusFilter = formValue.status !== null && formValue.status !== undefined;
    const hasTypeFilter = formValue.type !== null && formValue.type !== undefined;
    
    this.hasActiveFilters = !!(
      formValue.employeeId ||
      hasStatusFilter ||
      hasTypeFilter ||
      formValue.startDate ||
      formValue.endDate
    );

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
      employeeId: formValue.employeeId || null,
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

  openFilterDialog(): void {
    const dialogRef = this.dialog.open(AbsenceRequestFilterDialogComponent, {
      width: '400px',
      maxWidth: '90vw',
      data: {
        employeeId: this.filterForm.value.employeeId,
        status: this.filterForm.value.status,
        type: this.filterForm.value.type,
        startDate: this.filterForm.value.startDate ? this.formatDateForApi(this.filterForm.value.startDate) : null,
        endDate: this.filterForm.value.endDate ? this.formatDateForApi(this.filterForm.value.endDate) : null,
        selectedEmployee: this.selectedEmployee
      } as AbsenceRequestFilterData
    });

    dialogRef.afterClosed().subscribe((filterData: AbsenceRequestFilterData | null) => {
      if (filterData) {
        this.selectedEmployee = filterData.selectedEmployee || null;
        this.filterForm.patchValue({
          employeeId: filterData.employeeId || null,
          status: filterData.status !== null && filterData.status !== undefined ? filterData.status : null,
          type: filterData.type !== null && filterData.type !== undefined ? filterData.type : null,
          startDate: filterData.startDate ? new Date(filterData.startDate) : null,
          endDate: filterData.endDate ? new Date(filterData.endDate) : null
        });
        this.applyFilters();
      }
    });
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
          originalStatus: item.status,
          originalType: item.type,
          originalStartDate: item.startDate,
          originalEndDate: item.endDate,
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

  onApproveAbsenceRequest(request: AbsenceRequestDisplayModel): void {
    // Convert display model back to brief response for the dialog
    const briefRequest: AbsenceRequestBriefResponse = {
      id: request.id,
      employeeId: request.employeeId,
      employeeName: request.employeeName,
      type: request.originalType,
      status: request.originalStatus,
      startDate: request.originalStartDate,
      endDate: request.originalEndDate,
      daysRequested: request.daysRequested,
      managerName: request.managerName
    };

    const dialogRef = this.dialog.open(ProcessAbsenceRequestDialogComponent, {
      width: '500px',
      data: { request: briefRequest, approved: true } as ProcessAbsenceRequestDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload the list to reflect the changes
        this.applyFilters();
      }
    });
  }

  onRejectAbsenceRequest(request: AbsenceRequestDisplayModel): void {
    // Convert display model back to brief response for the dialog
    const briefRequest: AbsenceRequestBriefResponse = {
      id: request.id,
      employeeId: request.employeeId,
      employeeName: request.employeeName,
      type: request.originalType,
      status: request.originalStatus,
      startDate: request.originalStartDate,
      endDate: request.originalEndDate,
      daysRequested: request.daysRequested,
      managerName: request.managerName
    };

    const dialogRef = this.dialog.open(ProcessAbsenceRequestDialogComponent, {
      width: '500px',
      data: { request: briefRequest, approved: false } as ProcessAbsenceRequestDialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload the list to reflect the changes
        this.applyFilters();
      }
    });
  }

}

