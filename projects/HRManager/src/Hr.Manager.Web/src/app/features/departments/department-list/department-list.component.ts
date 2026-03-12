import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { PaginationRequest } from '../../../core/models/common/pagination-request.model';
import { PageEvent } from '@angular/material/paginator';
import { DeleteDepartmentRequest, GetDepartmentByIdRequest, UpdateDepartmentRequest } from '../../../core/models/request-models/departmet-request.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { DataTableComponent } from "../../../shared/components/data-table/data-table.component";
import { MatIcon } from "@angular/material/icon";
import { LoadingSpinnerComponent } from "../../../shared/components/loading-spinner/loading-spinner.component";
import { ErrorDisplayComponent } from "../../../shared/components/error-display/error-display.component";
import { EmptyStateComponent } from "../../../shared/components/empty-state/empty-state.component";
import { MatButtonModule } from '@angular/material/button';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { ManagerSelectionDialog } from '../manager-selection-dialog/manager-selection-dialog';
import { NotificationService } from '../../../core/services/notification.service';
import { DepartmentDetailsDialogComponent } from '../department-details-dialog/department-details-dialog.component';

@Component({
  selector: 'app-department-list',
  imports: [DataTableComponent, MatIcon, LoadingSpinnerComponent, ErrorDisplayComponent, EmptyStateComponent, MatButtonModule],
  templateUrl: './department-list.component.html',
  styleUrl: './department-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentListComponent implements OnInit {
  private apiService = inject(ApiService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  dataSource = new MatTableDataSource<DepartmentDetailsResponse>([]);

  displayedColumns = [
    { key: 'name', label: 'Department Name' },
    { key: 'description', label: 'Description' },
    { key: 'manager', label: 'Manager' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<DepartmentDetailsResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;
  ngOnInit(): void {
    const request: PaginationRequest = { pageNumber: 1, pageSize: 10 }
    this.loadDepartments(request);
  }

  onPageChange(event: PageEvent) {
    const request: PaginationRequest = {
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.loadDepartments(request);
  }

  loadDepartments(request: PaginationRequest): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.cdr.markForCheck();

    this.apiService.getDepartmentsWithPagination(request).subscribe({
      next: (res: PaginatedModel<DepartmentDetailsResponse>) => {
        this.dataSource.data = res.items;
        this.paginationResult = res;
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

  onAddDepartment() {
    this.router.navigate(['/departments/new']);
  }

  onEditDepartment(department: UpdateDepartmentRequest) {
    this.router.navigate(['/departments', department.id, 'edit']);
  }

  onViewDepartment(department: DepartmentDetailsResponse) {
    this.dialog.open(DepartmentDetailsDialogComponent, {
      data: department,
      width: '500px',
      maxWidth: '90vw'
    });
  };

  onDeleteDepartment(departmet: DeleteDepartmentRequest) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { message: 'Are you sure you want to delete this department?' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deleteDepartment(departmet)
          .subscribe({
            next: () => {
              const pageNumber = this.paginationResult?.page || 1;
              const pageSize = this.paginationResult?.pageSize || 10;
              const request: PaginationRequest = { pageNumber: pageNumber, pageSize: pageSize };
              this.loadDepartments(request);
            },
            error: (error) => {
              this.notificationService.handleError(error);
            }
          });
      }
    });

  }

  onAssignManager(department: DepartmentDetailsResponse): void {
    const hasCurrentManager = !!department.manager;

    const dialogRef = this.dialog.open(ManagerSelectionDialog, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result && department.id) {
        if (hasCurrentManager) {
          this.apiService.removeDepartmentManager({ departmentId: department.id }).subscribe({
            next: () => {
              this.assignNewManager(department, result);
            },
            error: (error) => {
              this.notificationService.handleError(error);
              console.error('Error removing current manager:', error);
            }
          });
        } else {
          this.assignNewManager(department, result);
        }
      }
    });
  }

  private assignNewManager(department: DepartmentDetailsResponse, employee: EmployeesBriefResponse): void {
    const updateRequest: UpdateDepartmentRequest = {
      id: department.id!,
      name: department.name,
      description: department.description,
      managerId: employee.id
    };

    this.apiService.updateDepartment(updateRequest).subscribe({
      next: () => {
        this.notificationService.success(`${employee.fullName} assigned as manager to ${department.name}.`);
        const pageNumber = this.paginationResult?.page || 1;
        const pageSize = this.paginationResult?.pageSize || 10;
        this.loadDepartments({ pageNumber, pageSize });
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error assigning manager:', error);
      }
    });
  }
}
