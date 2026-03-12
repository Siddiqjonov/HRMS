import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { PaginationRequest } from '../../../core/models/common/pagination-request.model';
import { PageEvent } from '@angular/material/paginator';
import { DeletePositionRequest, GetPositionByIdRequest, UpdatePositionRequest } from '../../../core/models/request-models/position-request.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { PositionDetailsDialogComponent } from '../position-details-dialog/position-details-dialog.component';
import { PositionEmployeesDialogComponent } from '../position-employees-dialog/position-employees-dialog.component';
import { DataTableComponent } from "../../../shared/components/data-table/data-table.component";
import { MatIcon } from "@angular/material/icon";
import { LoadingSpinnerComponent } from "../../../shared/components/loading-spinner/loading-spinner.component";
import { ErrorDisplayComponent } from "../../../shared/components/error-display/error-display.component";
import { EmptyStateComponent } from "../../../shared/components/empty-state/empty-state.component";
import { MatButtonModule } from '@angular/material/button';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-position-list',
  imports: [DataTableComponent, MatIcon, LoadingSpinnerComponent, ErrorDisplayComponent, MatButtonModule, EmptyStateComponent],
  templateUrl: './position-list.component.html',
  styleUrl: './position-list.component.css'
})
export class PositionListComponent implements OnInit {
  private apiService = inject(ApiService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private notificationService = inject(NotificationService);

  dataSource = new MatTableDataSource<PositionResponse>([]);

  displayedColumns = [
    { key: 'title', label: 'Position Title' },
    { key: 'departmentName', label: 'Department' },
    { key: 'salaryMin', label: 'Min Salary' },
    { key: 'salaryMax', label: 'Max Salary' },
    { key: 'actions', label: 'Actions' }
  ];

  paginationResult: PaginatedModel<PositionResponse> | null = null;
  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const request: PaginationRequest = { pageNumber: 1, pageSize: 10 }
    this.loadPositions(request);
  }

  onPageChange(event: PageEvent) {
    const request: PaginationRequest = {
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.loadPositions(request);
    console.log('Request', request);
  }

  loadPositions(request: PaginationRequest): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.apiService.getPositionsWithPagination(request).subscribe({
      next: (res: PaginatedModel<PositionResponse>) => {
        this.dataSource.data = res.items;
        this.paginationResult = res;
        this.isLoading = false;
      },
      error: (error: any) => {
        this.errorMessage = 'Failed to load positions. Please try again.';
        this.notificationService.handleError(error);
        this.isLoading = false;
      }
    });
  }

  onAddPosition() {
    this.router.navigate(['/positions/new']);
  }

  onEditPosition(position: UpdatePositionRequest) {
    this.router.navigate(['/positions', position.id, 'edit']);
  }

  onViewPosition(position: PositionResponse) {
    this.dialog.open(PositionDetailsDialogComponent, {
      data: position,
      width: '500px',
    });
  }

  onDeletePosition(position: DeletePositionRequest) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { message: 'Are you sure you want to delete this position?' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.apiService.deletePosition(position)
          .subscribe({
            next: () => {
              this.notificationService.success('Position deleted successfully!');
              const pageNumber = this.paginationResult?.page || 1;
              const pageSize = this.paginationResult?.pageSize || 10;
              const request: PaginationRequest = { pageNumber: pageNumber, pageSize: pageSize };
              this.loadPositions(request);
            },
            error: (error) => {
              this.notificationService.handleError(error);
            }
          });
      }
    });
  }

  onViewEmployees(position: PositionResponse) {
    this.isLoading = true;
    
    this.apiService.getPositionEmployees({ positionId: position.id }).subscribe({
      next: (employees) => {
        this.isLoading = false;
        this.dialog.open(PositionEmployeesDialogComponent, {
          data: {
            positionTitle: position.title,
            employees: employees
          },
          width: '500px',
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

