import { Component, inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { EmployeeSelectionDialogComponent } from '../../attendance/employee-selection-dialog/employee-selection-dialog.component';
import { DepartmentSelectionDialogComponent } from '../../departments/department-selection-dialog/department-selection-dialog';
import { ApiService } from '../../../core/services/api.service';

export interface ReportFilterData {
  reportId: string;
  reportTitle: string;
  availableFilters: ('dateRange' | 'department' | 'employee')[];
  startDate?: string | null;
  endDate?: string | null;
  selectedDepartment?: DepartmentDetailsResponse | null;
  selectedEmployee?: EmployeesBriefResponse | null;
}

@Component({
  selector: 'app-reports-filter-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule,
    MatIconModule,
    FormsModule
  ],
  templateUrl: './reports-filter-dialog.component.html',
  styleUrl: './reports-filter-dialog.component.css'
})
export class ReportsFilterDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<ReportsFilterDialogComponent>);
  private dialog = inject(MatDialog);
  private apiService = inject(ApiService);
  data: ReportFilterData = inject(MAT_DIALOG_DATA) || {
    reportId: '',
    reportTitle: '',
    availableFilters: []
  };

  startDate: Date | null = null;
  endDate: Date | null = null;
  selectedDepartment: DepartmentDetailsResponse | null = null;
  selectedEmployee: EmployeesBriefResponse | null = null;

  ngOnInit(): void {
    if (this.data) {
      this.startDate = this.data.startDate ? new Date(this.data.startDate) : null;
      this.endDate = this.data.endDate ? new Date(this.data.endDate) : null;
      this.selectedDepartment = this.data.selectedDepartment || null;
      this.selectedEmployee = this.data.selectedEmployee || null;
    }

    // Prevent autofocus
    setTimeout(() => {
      const inputs = document.querySelectorAll('input');
      inputs.forEach(input => {
        if (input.closest('mat-dialog-container') && !input.classList.contains('selection-input')) {
          input.blur();
        }
      });
      const activeElement = document.activeElement as HTMLElement;
      if (activeElement && activeElement.tagName === 'INPUT' && !activeElement.classList.contains('selection-input')) {
        activeElement.blur();
      }
    }, 100);
  }

  hasFilter(filterType: 'dateRange' | 'department' | 'employee'): boolean {
    return this.data.availableFilters.includes(filterType);
  }

  openEmployeeSelectionDialog(): void {
    // If a department is selected, fetch only employees from that department
    if (this.selectedDepartment) {
      this.apiService.getDepartmentEmployees(this.selectedDepartment.id).subscribe({
        next: (employees) => {
          this.openEmployeeSelectionDialogWithEmployees(employees);
        },
        error: (error) => {
          console.error('Error loading department employees:', error);
          // Fallback to all employees if error occurs
          this.openEmployeeSelectionDialogWithEmployees();
        }
      });
    } else {
      // No department selected, show all employees
      this.openEmployeeSelectionDialogWithEmployees();
    }
  }

  private openEmployeeSelectionDialogWithEmployees(employees?: EmployeesBriefResponse[]): void {
    const dialogRef = this.dialog.open(EmployeeSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      data: {
        employees: employees,
        selectedEmployeeId: this.selectedEmployee?.id
      }
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result) {
        this.selectedEmployee = result;
      }
    });
  }

  openDepartmentSelectionDialog(): void {
    // If an employee is selected, fetch only that employee's department
    if (this.selectedEmployee) {
      // Get the employee's full details to get departmentId
      this.apiService.getEmployeeById({ id: this.selectedEmployee.id }).subscribe({
        next: (fullEmployee) => {
          // Get the department that the employee belongs to
          this.apiService.getDepartmentById({ id: fullEmployee.departmentId }).subscribe({
            next: (department) => {
              // Pass employee's departmentId so we can verify it matches when department is selected
              this.openDepartmentSelectionDialogWithDepartments([department], fullEmployee.departmentId);
            },
            error: (error) => {
              console.error('Error loading employee department:', error);
              // Fallback to all departments if error occurs
              this.openDepartmentSelectionDialogWithDepartments();
            }
          });
        },
        error: (error) => {
          console.error('Error loading employee details:', error);
          // Fallback to all departments if error occurs
          this.openDepartmentSelectionDialogWithDepartments();
        }
      });
    } else {
      // No employee selected, show all departments
      this.openDepartmentSelectionDialogWithDepartments();
    }
  }

  private openDepartmentSelectionDialogWithDepartments(departments?: DepartmentDetailsResponse[], employeeDepartmentId?: string): void {
    const dialogRef = this.dialog.open(DepartmentSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      data: {
        departments: departments,
        selectedDepartmentId: this.selectedDepartment?.id
      }
    });

    dialogRef.afterClosed().subscribe((result: DepartmentDetailsResponse | null) => {
      if (result) {
        // If department changed, check if we should clear the employee
        if (this.selectedDepartment?.id !== result.id) {
          // If employee is selected and we have their departmentId, only clear if department doesn't match
          if (this.selectedEmployee && employeeDepartmentId) {
            // Only clear employee if the selected department is NOT the employee's department
            if (result.id !== employeeDepartmentId) {
              this.selectedEmployee = null;
            }
          } else {
            // No employee selected or no departmentId available, clear employee to be safe
            this.selectedEmployee = null;
          }
        }
        // Always update the selected department
        this.selectedDepartment = result;
      }
    });
  }

  onEmployeeInputClick(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.openEmployeeSelectionDialog();
  }

  onEmployeeInputKeydown(event: KeyboardEvent): void {
    event.preventDefault();
    if (event.key === 'Enter' || event.key === ' ') {
      this.openEmployeeSelectionDialog();
    }
  }

  onDepartmentInputClick(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.openDepartmentSelectionDialog();
  }

  onDepartmentInputKeydown(event: KeyboardEvent): void {
    event.preventDefault();
    if (event.key === 'Enter' || event.key === ' ') {
      this.openDepartmentSelectionDialog();
    }
  }

  clearEmployee(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.selectedEmployee = null;
    // Clear department selection when employee is cleared
    this.selectedDepartment = null;
  }

  clearDepartment(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.selectedDepartment = null;
    // Clear employee selection when department is cleared
    this.selectedEmployee = null;
  }

  onApplyFilters(): void {
    const filterData: ReportFilterData = {
      reportId: this.data.reportId,
      reportTitle: this.data.reportTitle,
      availableFilters: this.data.availableFilters,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      selectedDepartment: this.selectedDepartment,
      selectedEmployee: this.selectedEmployee
    };
    this.dialogRef.close(filterData);
  }

  onClearAllFilters(): void {
    this.startDate = null;
    this.endDate = null;
    this.selectedDepartment = null;
    this.selectedEmployee = null;
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  private formatDateForRequest(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
