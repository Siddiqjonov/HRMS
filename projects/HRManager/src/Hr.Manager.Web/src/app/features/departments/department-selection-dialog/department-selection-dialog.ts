import { Component, inject, OnInit } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { MatDialog, MatDialogRef, MatDialogModule, MatDialogContent, MatDialogActions, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { NotificationService } from '../../../core/services/notification.service';
import { GetDepartmentByIdRequest } from '../../../core/models/request-models/departmet-request.model';
import { DepartmentDetailsDialogComponent } from '../department-details-dialog/department-details-dialog.component';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-department-selection-dialog',
  imports: [CommonModule, MatDialogModule, MatDialogContent, MatDialogActions, MatProgressSpinner, MatIcon, MatButton, EmptyStateComponent],
  templateUrl: './department-selection-dialog.html',
  styleUrl: './department-selection-dialog.css'
})
export class DepartmentSelectionDialogComponent implements OnInit {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<DepartmentSelectionDialogComponent>);
  data: { departments?: DepartmentDetailsResponse[], selectedDepartmentId?: string } = inject(MAT_DIALOG_DATA) || {};
  
  departments: DepartmentDetailsResponse[] = [];
  selectedDepartment: DepartmentDetailsResponse | null = null;
  loading = true;

  ngOnInit(): void {
    // If departments array is provided (even if empty), use it instead of loading all departments
    if (this.data?.departments !== undefined && this.data.departments !== null) {
      this.departments = this.data.departments;
      this.loading = false;
      
      if (this.data.selectedDepartmentId) {
        this.selectedDepartment = this.departments.find(d => d.id === this.data.selectedDepartmentId) || null;
      }
    } else {
      // No departments provided, load all departments
      this.loadDepartments();
    }
  }

  private loadDepartments(): void {
    this.loading = true;
    this.apiService.getDepartmentsWithPagination({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (response: PaginatedModel<DepartmentDetailsResponse>) => {
        this.departments = response.items;
        this.loading = false;
        
        if (this.data?.selectedDepartmentId) {
          this.selectedDepartment = this.departments.find(d => d.id === this.data.selectedDepartmentId) || null;
        }
      },
      error: (error: any) => {
        // Don't show error toast for "not found" - just show empty state
        this.departments = [];
        this.loading = false;
      }
    });
  }

  selectDepartment(department: DepartmentDetailsResponse): void {
    this.selectedDepartment = department;
  }

  onConfirm(): void {
    if (this.selectedDepartment) {
      this.dialogRef.close(this.selectedDepartment);
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  viewDepartmentDetails(department: DepartmentDetailsResponse, event: Event): void {
    event.stopPropagation();

    const request: GetDepartmentByIdRequest = { id: department.id };

    this.apiService.getDepartmentById(request).subscribe({
      next: (fullDepartment: DepartmentDetailsResponse) => {
        this.dialog.open(DepartmentDetailsDialogComponent, {
          data: fullDepartment,
          width: '500px'
        });
      },
      error: (err) => {
        console.error('Failed to load department details:', err);
      }
    });
  }
}