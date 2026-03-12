import { Component, inject } from '@angular/core';
import { MatDialog, MatDialogActions, MatDialogContent, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { EmployeeResponse, EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { MatFormField, MatLabel } from "@angular/material/form-field";
import { ApiService } from '../../../core/services/api.service';
import { MatButton } from '@angular/material/button';
import { EmployeeDetailsDialogComponent } from '../../employees/employee-details-dialog/employee-details-dialog.component';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';

@Component({
  selector: 'app-manager-selection-dialog',
  imports: [MatDialogActions, MatIcon, MatProgressSpinner, MatDialogContent, MatDialogModule, MatButton],
  templateUrl: './manager-selection-dialog.html',
  styleUrl: './manager-selection-dialog.css'
})
export class ManagerSelectionDialog {
  private apiService = inject(ApiService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<ManagerSelectionDialog>);

  employees: EmployeesBriefResponse[] = [];
  selectedEmployee: EmployeesBriefResponse | null = null;
  loading = true;

  ngOnInit(): void {
    this.loadEmployees();
  }

  private loadEmployees(): void {
    this.loading = true;
    this.apiService.getEmployeesWithPagination({ pageNumber: 1, pageSize: 100 })
      .subscribe({
        next: (response) => {
          this.employees = response.items.filter(e => !e.isManagerOfDepartment);
          this.loading = false;
          console.log(this.employees)
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
