import { Component, inject, Inject } from '@angular/core';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { MatDialog, MatDialogRef, MatDialogModule, MatDialogContent, MatDialogActions, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { GetPositionByIdRequest } from '../../../core/models/request-models/position-request.model';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { CommonModule } from '@angular/common';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { PositionDetailsDialogComponent } from '../position-details-dialog/position-details-dialog.component';

export interface PositionSelectionDialogData {
  departmentId?: string;
}

@Component({
  selector: 'app-position-selection-dialog',
  imports: [CommonModule, MatDialogModule, MatDialogContent, MatDialogActions, MatProgressSpinner, MatIcon, MatButton, EmptyStateComponent],
  templateUrl: './position-selection-dialog.html',
  styleUrl: './position-selection-dialog.css'
})
export class PositionSelectionDialogComponent {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private dialog = inject(MatDialog);
  private dialogRef = inject(MatDialogRef<PositionSelectionDialogComponent>);
  positions: PositionResponse[] = [];
  selectedPosition: PositionResponse | null = null;
  loading = true;
  departmentId: string | undefined;

  constructor(@Inject(MAT_DIALOG_DATA) public data: PositionSelectionDialogData) {
    this.departmentId = data?.departmentId;
  }

  ngOnInit(): void {
    this.loadPositions();
  }

  private loadPositions(): void {
    this.loading = true;
    this.apiService.getPositionsWithPagination({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (response: PaginatedModel<PositionResponse>) => {
        // Filter positions by department if departmentId is provided
        if (this.departmentId) {
          this.positions = response.items.filter(p => p.departmentId === this.departmentId);
        } else {
          this.positions = response.items;
        }
        this.loading = false;
      },
      error: (error) => {
        // Don't show error toast for "not found" - just show empty state
        this.positions = [];
        this.loading = false;
      }
    });
  }

  selectPosition(position: PositionResponse): void {
    this.selectedPosition = position;
  }

  onConfirm(): void {
    if (this.selectedPosition) {
      this.dialogRef.close(this.selectedPosition);
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  viewPositionDetails(position: PositionResponse, event: Event): void {
    event.stopPropagation();
    
    this.dialog.open(PositionDetailsDialogComponent, {
      data: position,
      width: '500px'
    });
  }
}
