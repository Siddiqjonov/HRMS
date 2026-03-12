import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogModule } from '@angular/material/dialog';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { MatIcon } from "@angular/material/icon";
import { MatButtonModule } from '@angular/material/button';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-position-details-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatDialogModule,
    DecimalPipe
  ],
  templateUrl: './position-details-dialog.component.html',
  styleUrl: './position-details-dialog.component.css'
})
export class PositionDetailsDialogComponent {
  private dialogRef = inject(MatDialogRef<PositionDetailsDialogComponent>);
  data: PositionResponse = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }
}

