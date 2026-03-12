import { Component, inject, OnInit } from '@angular/core';
import { MatFormField, MatLabel, MatError } from "@angular/material/form-field";
import { NotificationService } from '../../../core/services/notification.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ApiService } from '../../../core/services/api.service';
import { CreatePositionRequest, UpdatePositionRequest } from '../../../core/models/request-models/position-request.model';
import { ActivatedRoute, Router } from '@angular/router';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentSelectionDialogComponent } from '../../departments/department-selection-dialog/department-selection-dialog';

@Component({
  selector: 'app-position-form',
  imports: [MatFormField, MatLabel, MatError, ReactiveFormsModule, MatInput, MatButton, MatIconModule],
  templateUrl: './position-form.component.html',
  styleUrl: './position-form.component.css'
})
export class PositionFormComponent implements OnInit {
  private notificationService = inject(NotificationService)
  private apiService = inject(ApiService)
  private route = inject(ActivatedRoute);
  private router = inject(Router)
  private dialog = inject(MatDialog);
  isEditMode = false;
  positionId: string | null = null;
  selectedDepartment: DepartmentDetailsResponse | null = null;

  positionForm = new FormGroup({
    title: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(100), Validators.minLength(3)]
    }),
    departmentId: new FormControl('', {
      validators: [Validators.required]
    }),
    salaryMin: new FormControl(0, {
      validators: [Validators.required, Validators.min(0.01)]
    }),
    salaryMax: new FormControl(0, {
      validators: [Validators.required, Validators.min(0.01)]
    })
  });

  get titleIsRequired(): boolean {
    const titleControl = this.positionForm.controls.title;
    return (titleControl.touched || titleControl.dirty) && titleControl.hasError('required');
  }

  get titleHasMinLengthError(): boolean {
    const titleControl = this.positionForm.controls.title;
    return (titleControl.touched || titleControl.dirty) && titleControl.hasError('minlength');
  }

  get titleHasMaxLengthError(): boolean {
    const titleControl = this.positionForm.controls.title;
    return (titleControl.touched || titleControl.dirty) && titleControl.hasError('maxlength');
  }

  get departmentIsRequired(): boolean {
    const departmentControl = this.positionForm.controls.departmentId;
    return (departmentControl.touched || departmentControl.dirty) && departmentControl.hasError('required');
  }

  get salaryMinIsRequired(): boolean {
    const salaryMinControl = this.positionForm.controls.salaryMin;
    return (salaryMinControl.touched || salaryMinControl.dirty) && salaryMinControl.hasError('required');
  }

  get salaryMinHasError(): boolean {
    const salaryMinControl = this.positionForm.controls.salaryMin;
    return (salaryMinControl.touched || salaryMinControl.dirty) && salaryMinControl.hasError('min');
  }

  get salaryMaxIsRequired(): boolean {
    const salaryMaxControl = this.positionForm.controls.salaryMax;
    return (salaryMaxControl.touched || salaryMaxControl.dirty) && salaryMaxControl.hasError('required');
  }

  get salaryMaxHasError(): boolean {
    const salaryMaxControl = this.positionForm.controls.salaryMax;
    return (salaryMaxControl.touched || salaryMaxControl.dirty) && salaryMaxControl.hasError('min');
  }

  get salaryRangeError(): boolean {
    const salaryMinControl = this.positionForm.controls.salaryMin;
    const salaryMaxControl = this.positionForm.controls.salaryMax;
    const salaryMin = salaryMinControl.value;
    const salaryMax = salaryMaxControl.value;
    
    // Show error on max salary field when max < min
    return (salaryMaxControl.touched || salaryMaxControl.dirty) && 
           salaryMin != null && salaryMax != null &&
           salaryMin > 0 && salaryMax > 0 && 
           salaryMax < salaryMin;
  }

  get salaryMinRangeError(): boolean {
    const salaryMinControl = this.positionForm.controls.salaryMin;
    const salaryMaxControl = this.positionForm.controls.salaryMax;
    const salaryMin = salaryMinControl.value;
    const salaryMax = salaryMaxControl.value;
    
    // Show error on min salary field when min > max
    return (salaryMinControl.touched || salaryMinControl.dirty) && 
           salaryMin != null && salaryMax != null &&
           salaryMin > 0 && salaryMax > 0 && 
           salaryMin > salaryMax;
  }

  ngOnInit(): void {
    this.positionId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.positionId;

    if (this.isEditMode && this.positionId) {
      this.loadPositionData(this.positionId);
    }

    // Add real-time validation for salary range
    // When min salary changes, mark max salary as dirty to trigger validation
    this.positionForm.controls.salaryMin.valueChanges.subscribe(() => {
      this.positionForm.controls.salaryMax.markAsDirty();
      this.positionForm.controls.salaryMax.updateValueAndValidity();
    });

    // When max salary changes, mark min salary as dirty to trigger validation
    this.positionForm.controls.salaryMax.valueChanges.subscribe(() => {
      this.positionForm.controls.salaryMin.markAsDirty();
      this.positionForm.controls.salaryMin.updateValueAndValidity();
    });
  }

  assignDepartment(): void {
    const dialogRef = this.dialog.open(DepartmentSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true
    });

    dialogRef.afterClosed().subscribe((result: DepartmentDetailsResponse | null) => {
      if (result) {
        this.selectedDepartment = result;
        this.positionForm.patchValue({ departmentId: result.id });
      }
    });
  }

  loadPositionData(id: string): void {
    this.apiService.getPositionById({ id }).subscribe({
      next: (position: PositionResponse) => {
        this.positionForm.patchValue({
          title: position.title,
          departmentId: position.departmentId,
          salaryMin: position.salaryMin,
          salaryMax: position.salaryMax
        });

        // Set selected department for display
        this.selectedDepartment = {
          id: position.departmentId,
          name: position.departmentName
        } as DepartmentDetailsResponse;
      },
      error: (error: any) => {
        this.notificationService.handleError(error);
      }
    });
  }

  updatePosition(): void {
    if (!this.positionId) return;

    const formValue = this.positionForm.getRawValue();
    const updateRequest: UpdatePositionRequest = {
      id: this.positionId,
      title: formValue.title!,
      departmentId: formValue.departmentId!,
      salaryMin: formValue.salaryMin!,
      salaryMax: formValue.salaryMax!
    };

    this.apiService.updatePosition(updateRequest).subscribe({
      next: (_response: void) => {
        this.notificationService.success('Position updated successfully!');
        this.router.navigate(['/positions']);
      },
      error: (error: any) => {
        this.notificationService.handleError(error);
      }
    });
  }

  createPosition(): void {
    const formValue = this.positionForm.getRawValue() as CreatePositionRequest;
    this.apiService.createPosition(formValue).subscribe({
      next: (_response: void) => {
        this.notificationService.success('Position created successfully!');
        this.router.navigate(['/positions']);
      },
      error: (error: any) => {
        this.notificationService.handleError(error);
      }
    });
  }

  onSubmit(): void {
    // Mark all fields as touched to show validation errors
    this.positionForm.markAllAsTouched();
    
    if (this.positionForm.invalid) {
      this.notificationService.warning('Please fill in all required fields correctly');
      return;
    }

    const salaryMin = this.positionForm.controls.salaryMin.value || 0;
    const salaryMax = this.positionForm.controls.salaryMax.value || 0;

    // Check salary range
    if (salaryMin > salaryMax) {
      this.notificationService.warning('Minimum salary cannot be greater than maximum salary');
      return;
    }

    if (salaryMax < salaryMin) {
      this.notificationService.warning('Maximum salary must be greater than or equal to minimum salary');
      return;
    }

    if (this.isEditMode && this.positionId) {
      this.updatePosition();
    } else {
      this.createPosition();
    }
  }

  onCancel(): void {
    this.router.navigate(['/positions']);
  }
}

