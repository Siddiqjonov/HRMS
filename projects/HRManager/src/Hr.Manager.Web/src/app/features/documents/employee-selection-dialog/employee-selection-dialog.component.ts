import { Component, inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogActions, MatDialogContent, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { ApiService } from '../../../core/services/api.service';
import { MatButton } from '@angular/material/button';
import { EmployeeDetailsDialogComponent } from '../../employees/employee-details-dialog/employee-details-dialog.component';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-employee-selection-dialog',
  standalone: true,
  imports: [
    MatDialogActions, 
    MatIcon, 
    MatProgressSpinner, 
    MatDialogContent, 
    MatDialogModule, 
    MatButton,
    CommonModule,
    EmptyStateComponent
  ],
  templateUrl: './employee-selection-dialog.component.html',
  styleUrl: './employee-selection-dialog.component.css'
})
export class EmployeeSelectionDialogComponent implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<EmployeeSelectionDialogComponent>);
  data: { employees?: EmployeesBriefResponse[], selectedEmployeeId?: string } = inject(MAT_DIALOG_DATA) || {};

  employees: EmployeesBriefResponse[] = [];
  selectedEmployee: EmployeesBriefResponse | null = null;
  loading = true;

  ngOnInit(): void {
    if (this.data?.employees && this.data.employees.length > 0) {
      this.employees = this.data.employees;
      this.loading = false;
      
      if (this.data.selectedEmployeeId) {
        this.selectedEmployee = this.employees.find(e => e.id === this.data.selectedEmployeeId) || null;
      }
    } else {
      this.loadEmployees();
    }
  }

  private async loadEmployees(): Promise<void> {
    this.loading = true;
    try {
      const allEmployees: EmployeesBriefResponse[] = [];
      let currentPage = 1;
      const pageSize = 100;
      let hasMorePages = true;

      while (hasMorePages) {
        const response = await this.apiService.getEmployeesWithPagination({ 
          pageNumber: currentPage, 
          pageSize 
        }).toPromise();

        if (response) {
          allEmployees.push(...response.items);
          hasMorePages = response.hasNextPage;
          currentPage++;
        } else {
          hasMorePages = false;
        }
      }

      this.employees = allEmployees;
      
      if (this.data?.selectedEmployeeId) {
        this.selectedEmployee = this.employees.find(e => e.id === this.data.selectedEmployeeId) || null;
      }
    } catch (error) {
      console.error('Error loading employees:', error);
    } finally {
      this.loading = false;
    }
  }

  selectEmployee(employee: EmployeesBriefResponse): void {
    this.selectedEmployee = employee;
  }

  onConfirm(): void {
    if (this.selectedEmployee) {
      this.dialogRef.close(this.selectedEmployee);
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  viewEmployeeDetails(employee: EmployeesBriefResponse, event: Event): void {
    event.stopPropagation();

    const request: GetEmployeeByIdRequest = { id: employee.id };

    this.apiService.getEmployeeById(request).subscribe({
      next: (fullEmployee) => {
        this.dialog.open(EmployeeDetailsDialogComponent, {
          data: fullEmployee,
          width: '500px'
        });
      },
      error: (err) => {
        console.error('Failed to load employee details:', err);
      }
    });
  }
}
