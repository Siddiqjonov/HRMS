import { Component, inject, OnInit, ViewChild, viewChild } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MatTableDataSource } from '@angular/material/table';
import { EmployeeResponse, EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../../shared/components/error-display/error-display.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { PaginationRequest } from '../../../core/models/common/pagination-request.model';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { DeleteEmployeeRequest, GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
import { EmployeeDetailsDialogComponent } from '../employee-details-dialog/employee-details-dialog.component';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [
    LoadingSpinnerComponent,
    ErrorDisplayComponent,
    EmptyStateComponent,
    DataTableComponent,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css'
})
export class EmployeeListComponent implements OnInit {
  private apiService = inject(ApiService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);

  dataSource = new MatTableDataSource<EmployeesBriefResponse>([]);
  displayedColumns = [
    { key: 'fullName', label: 'Full Name' },
    { key: 'email', label: 'Email' },
    { key: 'departmentName', label: 'Department' },
    { key: 'positionName', label: 'Position' },
    { key: 'hireDate', label: 'Hire Date' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<EmployeesBriefResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const request: PaginationRequest = { pageNumber: 1, pageSize: 10 }
    this.loadEmployees(request);
  }

  onPageChange(event: PageEvent) {
    const request: PaginationRequest = {
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.loadEmployees(request);
  }

  loadEmployees(request: PaginationRequest): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.apiService.getEmployeesWithPagination(request).subscribe({
      next: (res: PaginatedModel<EmployeesBriefResponse>) => {
        console.log('Response', res); 
        this.dataSource.data = res.items;
        this.paginationResult = res;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load employees';
        this.notificationService.handleError(err);
        this.isLoading = false;
      }
    });
  }

  onAddEmployee() {
    this.router.navigate(['/employees/new']);
  }

  onEditEmployee(employee: any) {
    this.router.navigate(['/employees', employee.id, 'edit']);
  }

  onDeleteEmployee(employee: any) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { message: `Are you sure you want to delete ${employee.fullName}?` }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deleteEmployee(employee)
          .subscribe(() => {
            const pageNumber = this.paginationResult?.page || 1;
            const pageSize = this.paginationResult?.pageSize || 10;
            const request: PaginationRequest = { pageNumber: pageNumber, pageSize: pageSize };
            this.loadEmployees(request);
          });
      }
    });
  }

  onViewEmployee(request: GetEmployeeByIdRequest) {
    this.isLoading = true;

    this.apiService.getEmployeeById(request).subscribe({
      next: (res: EmployeeResponse) => {
        this.isLoading = false;
        this.dialog.open(EmployeeDetailsDialogComponent, {
          data: res,
          width: '500px',
          maxWidth: '90vw'
        });
      },
      error: (err) => {
        this.isLoading = false;
        this.notificationService.handleError(err);
      }
    });
  }
}
