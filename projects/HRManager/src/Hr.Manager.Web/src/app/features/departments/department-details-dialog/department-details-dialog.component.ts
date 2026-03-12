import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { MatIcon } from "@angular/material/icon";
import { MatButton, MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-department-details-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatDialogModule
  ],
  templateUrl: './department-details-dialog.component.html',
  styleUrl: './department-details-dialog.component.css'
})
export class DepartmentDetailsDialogComponent {
  private dialogRef = inject(MatDialogRef<DepartmentDetailsDialogComponent>);
  data: DepartmentDetailsResponse = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }
}

