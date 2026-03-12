import { Component, inject, OnInit } from '@angular/core';
import { MatFormField, MatLabel, MatError } from "@angular/material/form-field";
import { NotificationService } from '../../../../core/services/notification.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { ApiService } from '../../../../core/services/api.service';
import { CreateDepartmentRequest, UpdateDepartmentRequest } from '../../../../core/models/request-models/departmet-request.model';
import { EmployeesBriefResponse } from '../../../../core/models/response-models/employee-response.model';
import { MatDialog } from '@angular/material/dialog';
import { ManagerSelectionDialog } from '../../manager-selection-dialog/manager-selection-dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { DepartmentDetailsResponse } from '../../../../core/models/response-models/department-details-response.model';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-new-department',
  imports: [MatFormField, MatLabel, MatError, ReactiveFormsModule, MatInput, MatButton, MatIcon],
  templateUrl: './new-department.component.html',
  styleUrl: './new-department.component.css'
})
export class NewDepartmentComponent implements OnInit {
  private notificationService = inject(NotificationService)
  private apiService = inject(ApiService)
  private dialog = inject(MatDialog);
  private route = inject(ActivatedRoute);
  private router = inject(Router)
  selectedManager: EmployeesBriefResponse | null = null;
  isEditMode = false;
  departmentId: string | null = null;

  departmentForm = new FormGroup({
    name: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(100), Validators.minLength(3)]
    }),
    description: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(250)]
    }
    ),
    managerId: new FormControl<string | null>(null),
  });


  get nameIsRequired(): boolean {
    const nameControl = this.departmentForm.controls.name;
    return nameControl.touched && nameControl.hasError('required');
  }

  get nameHasLengthError(): boolean {
    const nameControl = this.departmentForm.controls.name;
    return nameControl.touched &&
      (nameControl.hasError('minlength') || nameControl.hasError('maxlength'));
  }

  get descriptionIsRequired(): boolean {
    const descControl = this.departmentForm.controls.description;
    return descControl.touched && descControl.hasError('required');
  }

  get descriptionHasLengthError(): boolean {
    const descControl = this.departmentForm.controls.description;
    return descControl.touched && descControl.hasError('maxlength');
  }
  ngOnInit(): void {
    this.departmentId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.departmentId;

    if (this.isEditMode && this.departmentId) {
      this.loadDepartmentData(this.departmentId);
    }
  }
  loadDepartmentData(id: string): void {
    this.apiService.getDepartmentById({ id }).subscribe({
      next: (department: DepartmentDetailsResponse) => {
        this.departmentForm.patchValue({
          name: department.name,
          description: department.description,
          managerId: department.manager?.id || null
        });

        if (department.manager) {
          this.selectedManager = {
            id: department.manager.id,
            fullName: department.manager.fullName
          } as EmployeesBriefResponse;
        }
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error loading department:', error);
      }
    });
  }
  updateDepartment(): void {
    if (!this.departmentId) return;

    const formValue = this.departmentForm.getRawValue();
    const updateRequest: UpdateDepartmentRequest = {
      id: this.departmentId,
      name: formValue.name!,
      description: formValue.description!,
      managerId: formValue.managerId
    };

    this.apiService.updateDepartment(updateRequest).subscribe({
      next: (response) => {
        this.departmentForm.reset();
        this.departmentForm.markAsPristine();
        this.departmentForm.markAsUntouched();
        this.notificationService.success('Department updated successfully!');
        this.router.navigate(['/departments']);
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error updating department:', error);
      }
    });
  }

  createDepartment(): void {
    const formValue = this.departmentForm.getRawValue() as CreateDepartmentRequest;
    this.apiService.createDepartment(formValue).subscribe({
      next: (response) => {
        this.departmentForm.reset();
        this.departmentForm.markAsPristine();
        this.departmentForm.markAsUntouched();
        this.notificationService.success('Department created successfully!');
        this.router.navigate(['/departments']);
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error creating department:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.departmentForm.invalid) {
      return;
    }

    if (this.isEditMode && this.departmentId) {
      this.updateDepartment();
    } else {
      this.createDepartment();
    }
  }

  assignManager(): void {
    const dialogRef = this.dialog.open(ManagerSelectionDialog, {
      width: '600px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result) {
        this.selectedManager = result;
        this.departmentForm.patchValue({ managerId: result.id });
      }
    });
  }

  removeManager(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: { message: 'Are you sure you want to remove the manager from this department?' }
    });

    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed && this.isEditMode && this.departmentId) {
        this.apiService.removeDepartmentManager({ departmentId: this.departmentId }).subscribe({
          next: () => {
            this.notificationService.success('Manager removed successfully!');
            this.selectedManager = null;
            this.departmentForm.patchValue({ managerId: null });

            if (this.departmentId) {
              this.loadDepartmentData(this.departmentId);
            }
          },
          error: (error) => {
            this.notificationService.handleError(error);
            console.error('Error removing manager:', error);
          }
        });
      } else if (confirmed && !this.isEditMode) {
        this.selectedManager = null;
        this.departmentForm.patchValue({ managerId: null });
        this.notificationService.info('Manager selection cleared');
      }
    });
  }
}

