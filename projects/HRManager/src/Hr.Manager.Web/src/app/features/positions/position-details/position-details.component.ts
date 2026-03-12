import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { GetPositionByIdRequest } from '../../../core/models/request-models/position-request.model';
import { NotificationService } from '../../../core/services/notification.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ErrorDisplayComponent } from '../../../shared/components/error-display/error-display.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { GetPositionEmployeesRequest } from '../../../core/models/request-models/position-request.model';

@Component({
  selector: 'app-position-details',
  imports: [LoadingSpinnerComponent, ErrorDisplayComponent, MatButtonModule, MatIcon, CommonModule],
  templateUrl: './position-details.component.html',
  styleUrl: './position-details.component.css'
})
export class PositionDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);

  position: PositionResponse | null = null;
  employees: EmployeesBriefResponse[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const positionId = this.route.snapshot.paramMap.get('id');
    if (positionId) {
      this.loadPositionDetails(positionId);
      this.loadPositionEmployees(positionId);
    }
  }

  loadPositionDetails(id: string): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.apiService.getPositionById({ id }).subscribe({
      next: (position: PositionResponse) => {
        this.position = position;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error?.message || 'Failed to load position details';
        this.isLoading = false;
        this.notificationService.handleError(error);
      }
    });
  }

  loadPositionEmployees(positionId: string): void {
    const request: GetPositionEmployeesRequest = { positionId };
    this.apiService.getPositionEmployees(request).subscribe({
      next: (employees: EmployeesBriefResponse[]) => {
        this.employees = employees;
      },
      error: (error) => {
        console.error('Error loading position employees:', error);
      }
    });
  }

  onEdit(): void {
    if (this.position?.id) {
      this.router.navigate(['/positions', this.position.id, 'edit']);
    }
  }

  onBack(): void {
    this.router.navigate(['/positions']);
  }
}

