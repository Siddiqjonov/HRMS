import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ProcessAbsenceRequest } from '../../../core/models/request-models/absence-request.model';
import { AbsenceRequestBriefResponse } from '../../../core/models/response-models/absence-request-request.model';
import { RequestType } from '../../../core/models/enums/request-type.enum';

export interface ProcessAbsenceRequestDialogData {
  request: AbsenceRequestBriefResponse;
  approved: boolean;
}

@Component({
  selector: 'app-process-absence-request-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule
  ],
  templateUrl: './process-absence-request-dialog.component.html',
  styleUrl: './process-absence-request-dialog.component.css'
})
export class ProcessAbsenceRequestDialogComponent {
  private dialogRef = inject(MatDialogRef<ProcessAbsenceRequestDialogComponent>);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  public data = inject<ProcessAbsenceRequestDialogData>(MAT_DIALOG_DATA);

  isLoading = false;

  processForm = new FormGroup({
    reason: new FormControl<string | null>(null, [Validators.maxLength(500)])
  });

  get reasonMaxLength(): boolean {
    const control = this.processForm.get('reason');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  formatRequestType(type: RequestType): string {
    const typeMap: { [key in RequestType]: string } = {
      [RequestType.Vacation]: 'Vacation',
      [RequestType.Sick]: 'Sick Leave',
      [RequestType.Remote]: 'Remote Work',
      [RequestType.Unpaid]: 'Unpaid Leave'
    };
    return typeMap[type] || 'Unknown';
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  }

  onSubmit(): void {
    if (this.processForm.invalid) {
      this.processForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.processForm.value;

    const request: ProcessAbsenceRequest = {
      id: this.data.request.id,
      approved: this.data.approved,
      reason: formValue.reason || null
    };

    this.apiService.processAbsenceRequest(request).subscribe({
      next: () => {
        this.notificationService.success(
          this.data.approved 
            ? 'Absence request approved successfully!' 
            : 'Absence request rejected successfully!'
        );
        this.dialogRef.close(true);
      },
      error: (error) => {
        this.isLoading = false;
        this.notificationService.handleError(error);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}

