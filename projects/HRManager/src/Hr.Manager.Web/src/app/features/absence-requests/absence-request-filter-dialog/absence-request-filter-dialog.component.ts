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
import { EmployeeSelectionDialogComponent } from '../employee-selection-dialog/employee-selection-dialog.component';
import { RequestStatus } from '../../../core/models/enums/request-status.enum';
import { RequestType } from '../../../core/models/enums/request-type.enum';

export interface AbsenceRequestFilterData {
  employeeId?: string | null;
  status?: RequestStatus | string | null;
  type?: RequestType | string | null;
  startDate?: string | null;
  endDate?: string | null;
  selectedEmployee?: EmployeesBriefResponse | null;
}

@Component({
  selector: 'app-absence-request-filter-dialog',
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
  templateUrl: './absence-request-filter-dialog.component.html',
  styleUrl: './absence-request-filter-dialog.component.css'
})
export class AbsenceRequestFilterDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<AbsenceRequestFilterDialogComponent>);
  private dialog = inject(MatDialog);
  data: AbsenceRequestFilterData = inject(MAT_DIALOG_DATA) || {};

  // Filter properties
  employeeId: string | null = null;
  status: RequestStatus | string | null = null;
  type: RequestType | string | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;
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
    // Initialize filter values from data
    if (this.data) {
      this.employeeId = this.data.employeeId || null;
      this.status = this.data.status !== null && this.data.status !== undefined ? this.data.status : null;
      // Ensure type is properly set, including 0 (Vacation)
      if (this.data.type !== null && this.data.type !== undefined) {
        // Convert to number if it's a string, then cast to RequestType
        const typeValue = typeof this.data.type === 'number' 
          ? this.data.type 
          : (typeof this.data.type === 'string' ? parseInt(this.data.type, 10) : Number(this.data.type));
        this.type = !isNaN(typeValue) ? (typeValue as RequestType) : null;
      } else {
        this.type = null;
      }
      this.startDate = this.data.startDate ? new Date(this.data.startDate) : null;
      this.endDate = this.data.endDate ? new Date(this.data.endDate) : null;
      this.selectedEmployee = this.data.selectedEmployee || null;
    }
  }

  openEmployeeSelectionDialog(): void {
    const dialogRef = this.dialog.open(EmployeeSelectionDialogComponent, {
      width: '600px',
      maxWidth: '90vw',
      disableClose: false,
      data: {}
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result) {
        this.selectedEmployee = result;
        this.employeeId = result.id;
      }
    });
  }

  clearEmployee(): void {
    this.selectedEmployee = null;
    this.employeeId = null;
  }

  onApplyFilters(): void {
    // Convert status to number if it's a string (from mat-select)
    let statusValue: RequestStatus | null = null;
    if (this.status !== null && this.status !== undefined) {
      if (typeof this.status === 'string' && this.status !== 'ALL') {
        const parsed = parseInt(this.status, 10);
        if (!isNaN(parsed)) {
          statusValue = parsed as RequestStatus;
        }
      } else if (typeof this.status === 'number') {
        statusValue = this.status as RequestStatus;
      } else if (this.status !== 'ALL') {
        statusValue = Number(this.status) as RequestStatus;
      }
    }

    // Convert type to number if it's a string (from mat-select)
    let typeValue: RequestType | null = null;
    if (this.type !== null && this.type !== undefined) {
      if (typeof this.type === 'string' && this.type !== 'ALL') {
        const parsed = parseInt(this.type, 10);
        if (!isNaN(parsed)) {
          typeValue = parsed as RequestType;
        }
      } else if (typeof this.type === 'number') {
        typeValue = this.type as RequestType;
      } else if (this.type !== 'ALL') {
        typeValue = Number(this.type) as RequestType;
      }
    }

    const filterData: AbsenceRequestFilterData = {
      employeeId: this.employeeId,
      status: statusValue,
      type: typeValue,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      selectedEmployee: this.selectedEmployee
    };
    this.dialogRef.close(filterData);
  }

  onClearAllFilters(): void {
    this.employeeId = null;
    this.status = null;
    this.type = null;
    this.startDate = null;
    this.endDate = null;
    this.selectedEmployee = null;
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  // Compare functions for mat-select to properly handle enum values
  compareStatus(a: RequestStatus | string | null, b: RequestStatus | string | null): boolean {
    if (a === null || b === null) return a === b;
    // Explicitly handle 0 (Pending) and other numeric values
    const aNum = typeof a === 'number' ? a : Number(a);
    const bNum = typeof b === 'number' ? b : Number(b);
    return !isNaN(aNum) && !isNaN(bNum) && aNum === bNum;
  }

  compareType(a: RequestType | string | null, b: RequestType | string | null): boolean {
    if (a === null || b === null) return a === b;
    // Explicitly handle 0 (Vacation) and other numeric values
    const aNum = typeof a === 'number' ? a : Number(a);
    const bNum = typeof b === 'number' ? b : Number(b);
    return !isNaN(aNum) && !isNaN(bNum) && aNum === bNum;
  }

  private formatDateForRequest(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}

