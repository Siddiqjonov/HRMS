import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialog, MatDialogModule } from '@angular/material/dialog';
import { EmployeeResponse } from '../../../core/models/response-models/employee-response.model';
import { MatIcon } from "@angular/material/icon";
import { MatButton, MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-employee-details-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatButton,
    MatDialogModule,
    MatDialogContent
  ],
  templateUrl: './employee-details-dialog.component.html',
  styleUrl: './employee-details-dialog.component.css'
})
export class EmployeeDetailsDialogComponent {
  private dialogRef = inject(MatDialogRef<EmployeeDetailsDialogComponent>);
  data: EmployeeResponse = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }
}
