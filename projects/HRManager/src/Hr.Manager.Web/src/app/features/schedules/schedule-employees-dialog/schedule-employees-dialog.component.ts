import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogModule } from '@angular/material/dialog';
import { MatIcon } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { EmployeeResponse, EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { ApiService } from '../../../core/services/api.service';
import { GetEmployeeByIdRequest } from '../../../core/models/request-models/employee-request.model';
import { EmployeeDetailsDialogComponent } from '../../employees/employee-details-dialog/employee-details-dialog.component';

export interface ScheduleEmployeesDialogData {
  scheduleName: string;
  employees: EmployeesBriefResponse[];
}

@Component({
  selector: 'app-schedule-employees-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatDialogModule,
    CommonModule
  ],
  templateUrl: './schedule-employees-dialog.component.html',
  styleUrl: './schedule-employees-dialog.component.css'
})
export class ScheduleEmployeesDialogComponent {
  private dialogRef = inject(MatDialogRef<ScheduleEmployeesDialogComponent>);
  private dialog = inject(MatDialog);
  private apiService = inject(ApiService);
  data: ScheduleEmployeesDialogData = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
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

