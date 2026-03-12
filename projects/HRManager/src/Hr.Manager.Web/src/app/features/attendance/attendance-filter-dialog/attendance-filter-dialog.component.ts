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

export interface AttendanceFilterData {
  selectedEmployeeId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  arrivalStatus?: 'all' | 'onTime' | 'late';
  departureStatus?: 'all' | 'onTime' | 'early';
  selectedEmployee?: EmployeesBriefResponse | null;
  hideEmployeeSelection?: boolean;
}

@Component({
  selector: 'app-attendance-filter-dialog',
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
  templateUrl: './attendance-filter-dialog.component.html',
  styleUrl: './attendance-filter-dialog.component.css'
})
export class AttendanceFilterDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<AttendanceFilterDialogComponent>);
  private dialog = inject(MatDialog);
  data: AttendanceFilterData = inject(MAT_DIALOG_DATA) || {};

  selectedEmployeeId: string | null = null;
  startDate: Date | null = null;
  endDate: Date | null = null;
  arrivalStatus: 'all' | 'onTime' | 'late' = 'all';
  departureStatus: 'all' | 'onTime' | 'early' = 'all';
  selectedEmployee: EmployeesBriefResponse | null = null;
  hideEmployeeSelection: boolean = false;

  ngOnInit(): void {
    if (this.data) {
      this.selectedEmployeeId = this.data.selectedEmployeeId || null;
      this.startDate = this.data.startDate ? new Date(this.data.startDate) : null;
      this.endDate = this.data.endDate ? new Date(this.data.endDate) : null;
      this.arrivalStatus = this.data.arrivalStatus || 'all';
      this.departureStatus = this.data.departureStatus || 'all';
      this.selectedEmployee = this.data.selectedEmployee || null;
      this.hideEmployeeSelection = this.data.hideEmployeeSelection || false;
    }

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

  openEmployeeSelectionDialog(): void {
    const dialogRef = this.dialog.open(EmployeeSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      data: {
        selectedEmployeeId: this.selectedEmployeeId
      }
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result) {
        this.selectedEmployee = result;
        this.selectedEmployeeId = result.id;
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

  clearEmployee(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.selectedEmployee = null;
    this.selectedEmployeeId = null;
  }

  onApplyFilters(): void {
    const filterData: AttendanceFilterData = {
      selectedEmployeeId: this.selectedEmployeeId,
      startDate: this.startDate ? this.formatDateForRequest(this.startDate) : null,
      endDate: this.endDate ? this.formatDateForRequest(this.endDate) : null,
      arrivalStatus: this.arrivalStatus,
      departureStatus: this.departureStatus,
      selectedEmployee: this.selectedEmployee
    };
    this.dialogRef.close(filterData);
  }

  onClearAllFilters(): void {
    this.selectedEmployeeId = null;
    this.startDate = null;
    this.endDate = null;
    this.arrivalStatus = 'all';
    this.departureStatus = 'all';
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
