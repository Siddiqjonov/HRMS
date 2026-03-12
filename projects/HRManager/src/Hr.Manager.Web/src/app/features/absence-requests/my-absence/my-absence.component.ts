import { Component, inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Router } from '@angular/router';
import { CreateAbsenceRequest } from '../../../core/models/request-models/absence-request.model';
import { RequestType } from '../../../core/models/enums/request-type.enum';
import { AuthenticationService } from '../../../core/services/authentication.service';
import { GetEmployeeByEmailRequest } from '../../../core/models/request-models/employee-request.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-my-absence',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './my-absence.component.html',
  styleUrl: './my-absence.component.css'
})
export class MyAbsenceComponent implements OnInit {
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private router = inject(Router);
  private authService = inject(AuthenticationService);

  isLoading = false;
  employeeId: string | null = null;

  absenceForm = new FormGroup({
    requestType: new FormControl<RequestType | null>(null, [Validators.required]),
    startDate: new FormControl<Date | null>(null, [Validators.required, this.futureDateValidator]),
    endDate: new FormControl<Date | null>(null, [Validators.required, this.futureDateValidator]),
    reason: new FormControl<string | null>(null, [Validators.required, Validators.maxLength(500)])
  }, { validators: this.dateRangeValidator });

  requestTypeOptions = [
    { value: RequestType.Vacation, label: 'Vacation' },
    { value: RequestType.Sick, label: 'Sick Leave' },
    { value: RequestType.Remote, label: 'Remote Work' },
    { value: RequestType.Unpaid, label: 'Unpaid Leave' }
  ];

  ngOnInit(): void {
    this.loadCurrentUserEmployeeId();
  }

  loadCurrentUserEmployeeId(): void {
    const userEmail = this.authService.userEmail;
    if (!userEmail) {
      this.notificationService.warning('Unable to get user email. Please try logging in again.');
      return;
    }

    const request: GetEmployeeByEmailRequest = { email: userEmail };
    this.apiService.getEmployeeByEmail(request).subscribe({
      next: (employee) => {
        this.employeeId = employee.id;
      },
      error: (error) => {
        this.notificationService.handleError(error);
      }
    });
  }

  futureDateValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }
    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    selectedDate.setHours(0, 0, 0, 0);
    
    if (selectedDate < today) {
      return { pastDate: true };
    }
    return null;
  }

  dateRangeValidator(control: AbstractControl): ValidationErrors | null {
    const startDate = control.get('startDate')?.value;
    const endDate = control.get('endDate')?.value;

    if (!startDate || !endDate) {
      return null;
    }

    const start = new Date(startDate);
    const end = new Date(endDate);
    start.setHours(0, 0, 0, 0);
    end.setHours(0, 0, 0, 0);

    if (end < start) {
      return { dateRangeInvalid: true };
    }

    return null;
  }

  get requestTypeRequired(): boolean {
    const control = this.absenceForm.get('requestType');
    return !!(control?.touched && control?.hasError('required'));
  }

  get startDateRequired(): boolean {
    const control = this.absenceForm.get('startDate');
    return !!(control?.touched && control?.hasError('required'));
  }

  get startDateInvalid(): boolean {
    const control = this.absenceForm.get('startDate');
    return !!(control?.touched && control?.hasError('pastDate'));
  }

  get endDateRequired(): boolean {
    const control = this.absenceForm.get('endDate');
    return !!(control?.touched && control?.hasError('required'));
  }

  get endDateInvalid(): boolean {
    const control = this.absenceForm.get('endDate');
    return !!(control?.touched && control?.hasError('pastDate'));
  }

  get dateRangeInvalid(): boolean {
    return !!(this.absenceForm.touched && this.absenceForm.hasError('dateRangeInvalid'));
  }

  get reasonRequired(): boolean {
    const control = this.absenceForm.get('reason');
    return !!(control?.touched && control?.hasError('required'));
  }

  get reasonMaxLength(): boolean {
    const control = this.absenceForm.get('reason');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  formatDateForApi(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  onSubmit(): void {
    if (this.absenceForm.invalid || !this.employeeId) {
      this.absenceForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.absenceForm.value;

    const request: CreateAbsenceRequest = {
      employeeId: this.employeeId,
      requestType: formValue.requestType!,
      startDate: this.formatDateForApi(formValue.startDate!),
      endDate: this.formatDateForApi(formValue.endDate!),
      reason: formValue.reason!
    };

    this.apiService.createAbsenceRequest(request).subscribe({
      next: () => {
        this.notificationService.success('Absence request created successfully!');
        this.router.navigate(['/my-absences']);
      },
      error: (error) => {
        this.isLoading = false;
        this.notificationService.handleError(error);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/my-absences']);
  }
}

