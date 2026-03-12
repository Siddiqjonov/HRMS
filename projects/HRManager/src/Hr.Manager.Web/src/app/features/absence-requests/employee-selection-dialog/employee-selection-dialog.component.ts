import { Component, inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogActions, MatDialogContent, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { MatButton } from '@angular/material/button';
import { EmployeeDetailsDialogComponent } from '../../employees/employee-details-dialog/employee-details-dialog.component';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
import { ApiService } from '../../../core/services/api.service';
import { EmployeeResponse } from '../../../core/models/response-models/employee-response.model';

@Component({
  selector: 'app-employee-selection-dialog',
  standalone: true,
  imports: [MatDialogActions, MatIcon, MatProgressSpinner, MatDialogContent, MatDialogModule, MatButton],
  templateUrl: './employee-selection-dialog.component.html',
  styleUrl: './employee-selection-dialog.component.css'
})
export class EmployeeSelectionDialogComponent implements OnInit {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<EmployeeSelectionDialogComponent>);

  employees: EmployeesBriefResponse[] = [];
  selectedEmployee: EmployeesBriefResponse | null = null;
  loading = true;

  ngOnInit(): void {
    this.loadEmployees();
  }

  private loadEmployees(): void {
    this.loading = true;
    // Request pagination with pageNumber: 1, pageSize: 100 as requested
    this.apiService.getEmployeesWithPagination({ pageNumber: 1, pageSize: 100 })
      .subscribe({
        next: (response) => {
          this.employees = response.items;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading employees:', error);
          this.loading = false;
        }
      });
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
      next: (fullEmployee: EmployeeResponse) => {
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

