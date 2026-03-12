import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent, MatDialogActions, MatDialogModule } from '@angular/material/dialog';
import { ScheduleResponse } from '../../../core/models/response-models/schedule-response.model';
import { MatIcon } from "@angular/material/icon";
import { MatButton, MatButtonModule } from '@angular/material/button';
import { DaysOfWeek } from '../../../core/models/enums/days-of-week.enum';

@Component({
  selector: 'app-schedule-details-dialog',
  imports: [
    MatIcon,
    MatDialogContent,
    MatDialogActions,
    MatButtonModule,
    MatButton,
    MatDialogModule,
    MatDialogContent
  ],
  templateUrl: './schedule-details-dialog.component.html',
  styleUrl: './schedule-details-dialog.component.css'
})
export class ScheduleDetailsDialogComponent {
  private dialogRef = inject(MatDialogRef<ScheduleDetailsDialogComponent>);
  data: ScheduleResponse = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }

  formatDaysOfWeek(days: DaysOfWeek): string {
    const dayNames: string[] = [];
    
    if (days & DaysOfWeek.Monday) dayNames.push('Monday');
    if (days & DaysOfWeek.Tuesday) dayNames.push('Tuesday');
    if (days & DaysOfWeek.Wednesday) dayNames.push('Wednesday');
    if (days & DaysOfWeek.Thursday) dayNames.push('Thursday');
    if (days & DaysOfWeek.Friday) dayNames.push('Friday');
    if (days & DaysOfWeek.Saturday) dayNames.push('Saturday');
    if (days & DaysOfWeek.Sunday) dayNames.push('Sunday');
    
    return dayNames.length > 0 ? dayNames.join(', ') : 'None';
  }
}

