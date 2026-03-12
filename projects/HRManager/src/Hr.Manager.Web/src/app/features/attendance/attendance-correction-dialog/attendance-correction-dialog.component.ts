import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogModule } from '@angular/material/dialog';
import { GetAttendanceRecordsResponse } from '../../../core/models/response-models/attendance-response.model';
import { CorrectAttendanceRecordRequest } from '../../../core/models/request-models/attendance-request.models';
import { MatIcon } from "@angular/material/icon";
import { MatButton, MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-attendance-correction-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CommonModule
  ],
  templateUrl: './attendance-correction-dialog.component.html',
  styleUrl: './attendance-correction-dialog.component.css'
})
export class AttendanceCorrectionDialogComponent {
  private dialogRef = inject(MatDialogRef<AttendanceCorrectionDialogComponent>);
  data: GetAttendanceRecordsResponse = inject(MAT_DIALOG_DATA);

  checkInTime: string = '';
  checkOutTime: string = '';
  attendanceId: string;

  constructor() {
    this.attendanceId = this.data.attendanceId;
    // Initialize with current values
    this.checkInTime = this.data.checkIn ? this.formatTimeForInput(this.data.checkIn) : '';
    this.checkOutTime = this.data.checkOut ? this.formatTimeForInput(this.data.checkOut) : '';
  }

  formatTimeForInput(time: string): string {
    // Convert "HH:mm:ss" or "HH:mm" to "HH:mm" format for input
    if (!time) return '';
    return time.substring(0, 5);
  }

  formatTimeForDisplay(time: string): string {
    // Extract HH:mm:ss from timestamp like "07:53:48.6026620"
    if (!time) return 'Not set';
    // If it contains a dot, take everything before it, otherwise take first 8 characters (HH:mm:ss)
    const dotIndex = time.indexOf('.');
    if (dotIndex !== -1) {
      return time.substring(0, dotIndex);
    }
    // If no dot, return first 8 characters (HH:mm:ss)
    return time.substring(0, 8);
  }

  onSave(): void {
    const request: CorrectAttendanceRecordRequest = {
      attendanceRecordId: this.attendanceId,
      checkIn: this.checkInTime ? this.checkInTime + ':00' : null,
      checkOut: this.checkOutTime ? this.checkOutTime + ':00' : null
    };

    this.dialogRef.close(request);
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}

