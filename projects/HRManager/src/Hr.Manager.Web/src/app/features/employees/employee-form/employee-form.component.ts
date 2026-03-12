import { Component, inject, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, AbstractControl, ValidationErrors, ReactiveFormsModule, AsyncValidatorFn } from '@angular/forms';
import { Gender } from '../../../core/models/enums/gender.enum';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentDetailsResponse } from '../../../core/models/response-models/department-details-response.model';
import { ScheduleResponse } from '../../../core/models/response-models/schedule-response.model';
import { CreateEmployeeRequest, UpdateEmployeeRequest } from '../../../core/models/request-models/employee-request.model';
import { ActivatedRoute, Router } from '@angular/router';
import { DepartmentSelectionDialogComponent } from '../../departments/department-selection-dialog/department-selection-dialog';
import { ScheduleSelectionDialogComponent } from '../../schedules/schedule-selection-dialog/schedule-selection-dialog';
import { PositionSelectionDialogComponent } from '../../positions/position-selection-dialog/position-selection-dialog';
import { PositionResponse } from '../../../core/models/response-models/position-response.model';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { EmployeeResponse } from '../../../core/models/response-models/employee-response.model';
import { Observable, of, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-employee-form',
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
    MatCheckboxModule,
    MatRadioModule,
    MatTabsModule,
    MatTableModule,
  ],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css'
})
export class EmployeeFormComponent implements OnInit {
  private apiService = inject(ApiService)
  private notificationService = inject(NotificationService)
  private dialog = inject(MatDialog);
  selectedDepartment: DepartmentDetailsResponse | null = null;
  selectedSchedule: ScheduleResponse | null = null;
  selectedPosition: PositionResponse | null = null;
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  isEditMode = false;
  employeeId: string | null = null;

  employeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    middleName: new FormControl('', [Validators.maxLength(50)]),
    email: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(50), this.emailValidator.bind(this)],
      asyncValidators: [this.emailDomainValidator()],
      updateOn: 'blur'
    }),
    passportNumber: new FormControl('', [Validators.required, this.passportValidator.bind(this)]),
    dateOfBirth: new FormControl('', [Validators.required, this.pastDateValidator]),
    nationality: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    gender: new FormControl<Gender | null>(null, [Validators.required]),
    pinfl: new FormControl('', [Validators.required, this.pinflValidator.bind(this)]),
    pensionFundNumber: new FormControl('', [this.pensionFundValidator.bind(this)]),
    taxIdentificationNumber: new FormControl('', [this.tinValidator.bind(this)]),
    phoneNumber: new FormControl('', [this.phoneValidator.bind(this)]),
    address: new FormGroup({
      region: new FormControl('', Validators.required),
      street: new FormControl('', Validators.required),
      house: new FormControl('', Validators.required),
      apartment: new FormControl('', Validators.required),
      fullAddress: new FormControl('', Validators.required),
    }),
    hireDate: new FormControl('', [Validators.required, this.notFutureDateValidator]),
    terminationDate: new FormControl(null),
    departmentId: new FormControl('', Validators.required),
    positionId: new FormControl('', Validators.required),
    salary: new FormControl({ value: 0, disabled: true }, [Validators.required, Validators.min(1)]),
    scheduleId: new FormControl('', Validators.required)
  });

  get firstNameRequired(): boolean {
    const control = this.employeeForm.get('firstName');
    return !!(control?.touched && control?.hasError('required'));
  }

  get firstNameMaxLength(): boolean {
    const control = this.employeeForm.get('firstName');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  get lastNameRequired(): boolean {
    const control = this.employeeForm.get('lastName');
    return !!(control?.touched && control?.hasError('required'));
  }

  get lastNameMaxLength(): boolean {
    const control = this.employeeForm.get('lastName');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  get emailRequired(): boolean {
    const control = this.employeeForm.get('email');
    return !!(control?.touched && control?.hasError('required'));
  }

  get emailInvalid(): boolean {
    const control = this.employeeForm.get('email');
    return !!(control?.touched && (control?.hasError('email') || control?.hasError('maxlength')));
  }

  get passportRequired(): boolean {
    const control = this.employeeForm.get('passportNumber');
    return !!(control?.touched && control?.hasError('required'));
  }

  get passportMaxLength(): boolean {
    const control = this.employeeForm.get('passportNumber');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  get dobRequired(): boolean {
    const control = this.employeeForm.get('dateOfBirth');
    return !!(control?.touched && control?.hasError('required'));
  }

  get dobInvalid(): boolean {
    const control = this.employeeForm.get('dateOfBirth');
    return !!(control?.touched && control?.hasError('pastDate'));
  }

  get nationalityRequired(): boolean {
    const control = this.employeeForm.get('nationality');
    return !!(control?.touched && control?.hasError('required'));
  }

  get genderRequired(): boolean {
    const control = this.employeeForm.get('gender');
    return !!(control?.touched && control?.hasError('required'));
  }

  get pinflRequired(): boolean {
    const control = this.employeeForm.get('pinfl');
    return !!(control?.touched && control?.hasError('required'));
  }

  get pinflMaxLength(): boolean {
    const control = this.employeeForm.get('pinfl');
    return !!(control?.touched && control?.hasError('maxlength'));
  }

  get phoneInvalid(): boolean {
    const control = this.employeeForm.get('phoneNumber');
    return !!(control?.touched && control?.hasError('pattern'));
  }

  get hireDateRequired(): boolean {
    const control = this.employeeForm.get('hireDate');
    return !!(control?.touched && control?.hasError('required'));
  }

  get hireDateInvalid(): boolean {
    const control = this.employeeForm.get('hireDate');
    return !!(control?.touched && control?.hasError('futureDate'));
  }

  get salaryRequired(): boolean {
    const control = this.employeeForm.get('salary');
    return !!(control?.touched && control?.hasError('required'));
  }

  get salaryInvalid(): boolean {
    const control = this.employeeForm.get('salary');
    return !!(control?.touched && control?.hasError('min'));
  }

  get departmentRequired(): boolean {
    const control = this.employeeForm.get('departmentId');
    return !!(control?.touched && control?.hasError('required'));
  }

  get positionRequired(): boolean {
    const control = this.employeeForm.get('positionId');
    return !!(control?.touched && control?.hasError('required'));
  }

  get scheduleRequired(): boolean {
    const control = this.employeeForm.get('scheduleId');
    return !!(control?.touched && control?.hasError('required'));
  }

  get emailDomainInvalid(): boolean {
    const control = this.employeeForm.get('email');
    return !!(control?.touched && control?.hasError('invalidDomain'));
  }

  get passportInvalid(): boolean {
    const control = this.employeeForm.get('passportNumber');
    return !!(control?.touched && control?.hasError('invalidPassport'));
  }

  get pinflInvalid(): boolean {
    const control = this.employeeForm.get('pinfl');
    return !!(control?.touched && control?.hasError('invalidPinfl'));
  }

  get pensionFundInvalid(): boolean {
    const control = this.employeeForm.get('pensionFundNumber');
    return !!(control?.touched && control?.hasError('invalidPensionFund'));
  }

  get tinInvalid(): boolean {
    const control = this.employeeForm.get('taxIdentificationNumber');
    return !!(control?.touched && control?.hasError('invalidTin'));
  }

  get phoneNumberInvalid(): boolean {
    const control = this.employeeForm.get('phoneNumber');
    return !!(control?.touched && control?.hasError('invalidPhone'));
  }

  get salaryOutOfRange(): boolean {
    const control = this.employeeForm.get('salary');
    return !!(control?.touched && (control?.hasError('salaryTooLow') || control?.hasError('salaryTooHigh')));
  }

  get salaryRangeMessage(): string {
    if (!this.selectedPosition) return '';
    return `Range: ${this.selectedPosition.salaryMin} - ${this.selectedPosition.salaryMax}`;
  }

  // Custom Validators
  emailValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailPattern.test(control.value)) {
      return { email: true };
    }
    // Check for fake patterns
    const fakeDomains = ['test.com', 'example.com', 'fake.com', 'temp.com', 'dummy.com'];
    const domain = control.value.split('@')[1]?.toLowerCase();
    if (fakeDomains.includes(domain)) {
      return { invalidDomain: true };
    }
    return null;
  }

  emailDomainValidator(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<ValidationErrors | null> => {
      if (!control.value) return of(null);
      
      const email = control.value;
      const domain = email.split('@')[1];
      
      if (!domain) return of({ invalidDomain: true });
      
      // Common valid domains
      const commonDomains = ['gmail.com', 'yahoo.com', 'outlook.com', 'hotmail.com', 
                            'icloud.com', 'mail.ru', 'yandex.ru', 'protonmail.com'];
      
      if (commonDomains.includes(domain.toLowerCase())) {
        return of(null);
      }
      
      // For other domains, do a basic check
      return timer(500).pipe(
        switchMap(() => {
          // Check if domain has valid structure
          const domainPattern = /^[a-zA-Z0-9][a-zA-Z0-9-]{1,61}[a-zA-Z0-9]\.[a-zA-Z]{2,}$/;
          if (!domainPattern.test(domain)) {
            return of({ invalidDomain: true });
          }
          return of(null);
        })
      );
    };
  }

  passportValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString().toUpperCase().trim();
    
    // Uzbekistan passport format: 2 letters + 7 digits (e.g., AA1234567)
    const uzbekPassportPattern = /^[A-Z]{2}\d{7}$/;
    
    if (!uzbekPassportPattern.test(value)) {
      return { invalidPassport: true };
    }
    
    // Check for obviously fake patterns
    if (/^AA0000000$/.test(value) || /^[A-Z]{2}1111111$/.test(value)) {
      return { invalidPassport: true };
    }
    
    return null;
  }

  pinflValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString().trim();
    
    // PINFL must be exactly 14 digits
    if (!/^\d{14}$/.test(value)) {
      return { invalidPinfl: true };
    }
    
    // Check for obviously fake patterns
    if (/^0+$/.test(value) || /^1{14}$/.test(value) || /^9{14}$/.test(value)) {
      return { invalidPinfl: true };
    }
    
    return null;
  }

  pensionFundValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString().trim();
    
    // Pension fund number: typically 14-20 alphanumeric characters
    if (!/^[A-Z0-9]{10,20}$/i.test(value)) {
      return { invalidPensionFund: true };
    }
    
    // Check for obviously fake patterns
    if (/^0+$/.test(value) || /^1+$/.test(value)) {
      return { invalidPensionFund: true };
    }
    
    return null;
  }

  tinValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString().trim();
    
    // TIN: 9 digits for Uzbekistan
    if (!/^\d{9}$/.test(value)) {
      return { invalidTin: true };
    }
    
    // Check for obviously fake patterns
    if (/^0+$/.test(value) || /^1{9}$/.test(value) || /^9{9}$/.test(value)) {
      return { invalidTin: true };
    }
    
    return null;
  }

  phoneValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    
    const value = control.value.toString().trim();
    
    // Phone must start with + and country code, then 9-15 digits
    // Uzbekistan: +998 followed by 9 digits
    const phonePattern = /^\+998\d{9}$/;
    
    if (!phonePattern.test(value)) {
      return { invalidPhone: true };
    }
    
    // Check for obviously fake patterns
    if (/\+9980{9}$/.test(value) || /\+9981{9}$/.test(value)) {
      return { invalidPhone: true };
    }
    
    return null;
  }

  salaryRangeValidator(): ValidationErrors | null {
    if (!this.selectedPosition) return null;
    
    const salary = this.employeeForm.get('salary')?.value;
    if (!salary) return null;
    
    if (salary < this.selectedPosition.salaryMin) {
      return { salaryTooLow: true };
    }
    
    if (salary > this.selectedPosition.salaryMax) {
      return { salaryTooHigh: true };
    }
    
    return null;
  }

  ngOnInit(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.employeeId;

    if (this.isEditMode && this.employeeId) {
      this.loadEmployeeData(this.employeeId);
    }

    this.setupAddressAutoPopulation();
    this.setupSalaryValidation();
  }

  private setupSalaryValidation(): void {
    // Watch for salary changes to validate against range
    this.employeeForm.get('salary')?.valueChanges.subscribe(() => {
      const salaryControl = this.employeeForm.get('salary');
      if (salaryControl && this.selectedPosition) {
        const errors = this.salaryRangeValidator();
        if (errors) {
          salaryControl.setErrors({ ...salaryControl.errors, ...errors });
        } else {
          // Remove range errors but keep other errors
          const currentErrors = salaryControl.errors;
          if (currentErrors) {
            delete currentErrors['salaryTooLow'];
            delete currentErrors['salaryTooHigh'];
            const hasOtherErrors = Object.keys(currentErrors).length > 0;
            salaryControl.setErrors(hasOtherErrors ? currentErrors : null);
          }
        }
      }
    });
  }

  private setupAddressAutoPopulation(): void {
    const addressGroup = this.employeeForm.get('address') as FormGroup;

    const fieldsToWatch = ['region', 'street', 'house', 'apartment'];

    fieldsToWatch.forEach(fieldName => {
      addressGroup.get(fieldName)?.valueChanges.subscribe(() => {
        this.updateFullAddress();
      });
    });
  }

  private updateFullAddress(): void {
    const addressGroup = this.employeeForm.get('address') as FormGroup;

    const region = (addressGroup.get('region')?.value || '').toString().trim();
    const street = (addressGroup.get('street')?.value || '').toString().trim();
    const house = (addressGroup.get('house')?.value || '').toString().trim();
    const apartment = (addressGroup.get('apartment')?.value || '').toString().trim();

    const parts: string[] = [];

    if (street) parts.push(`Street ${street}`);
    if (house) parts.push(`House ${house}`);
    if (apartment) parts.push(`Apartment ${apartment}`);
    if (region) parts.push(`Region ${region}`);

    const fullAddress = parts.join(', ');

    if (document.activeElement?.getAttribute('formControlName') !== 'fullAddress') {
      addressGroup.get('fullAddress')?.setValue(fullAddress, { emitEvent: false });
    }
  }

  loadEmployeeData(id: string): void {
    this.apiService.getEmployeeById({ id }).subscribe({
      next: (employee: EmployeeResponse) => {
        this.employeeForm.patchValue({
          firstName: employee.firstName,
          lastName: employee.lastName,
          middleName: employee.middleName,
          email: employee.email,
          passportNumber: employee.passportNumber,
          dateOfBirth: employee.dateOfBirth,
          nationality: employee.nationality,
        });
        this.employeeForm.patchValue({
          gender: employee.gender,
          pinfl: employee.pinfl,
          pensionFundNumber: employee.pensionFundNumber,
          taxIdentificationNumber: employee.taxIdentificationNumber,
          phoneNumber: employee.phoneNumber,
          address: {
            region: employee.address.region,
            street: employee.address.street,
            house: employee.address.house,
            apartment: employee.address.apartment,
            fullAddress: employee.address.fullAddress,
          },
          hireDate: employee.hireDate,
          departmentId: employee.departmentId,
          positionId: employee.positionId,
          salary: employee.salary,
          scheduleId: employee.scheduleId,
        });

        // Set selected objects for display
        this.selectedDepartment = {
          id: employee.departmentId,
          name: employee.departmentName
        } as DepartmentDetailsResponse;

        // Load full position details to get salary range
        if (employee.positionId) {
          this.apiService.getPositionById({ id: employee.positionId }).subscribe({
            next: (position) => {
              this.selectedPosition = position;
              // Enable salary field in edit mode
              const salaryControl = this.employeeForm.get('salary');
              if (salaryControl) {
                salaryControl.enable();
                salaryControl.updateValueAndValidity();
              }
            },
            error: (err) => {
              console.error('Failed to load position:', err);
              // Fallback: still set basic position info
              this.selectedPosition = {
                id: employee.positionId,
                title: employee.positionName
              } as PositionResponse;
            }
          });
        }

        // Load schedule details to display
        if (employee.scheduleId) {
          this.apiService.getScheduleById({ id: employee.scheduleId }).subscribe({
            next: (schedule) => {
              this.selectedSchedule = schedule;
            },
            error: (err) => {
              console.error('Failed to load schedule:', err);
            }
          });
        }
      },
      error: (error: any) => {
        this.notificationService.handleError(error);
      }
    });
  }

  pastDateValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    const date = new Date(value);
    return date < new Date() ? null : { pastDate: true };
  }

  notFutureDateValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    const date = new Date(value);
    return date <= new Date() ? null : { futureDate: true };
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      console.log('Form is invalid:', this.employeeForm.errors);
      console.log('Invalid fields:', this.getInvalidFields());
      this.employeeForm.markAllAsTouched();
      return;
    }

    if (this.isEditMode && this.employeeId) {
      this.updateEmployee();
    } else {
      this.createEmployee();
    }
  }

  private createEmployee(): void {
    const formValue = this.employeeForm.getRawValue();
    console.log('Form value being sent:', formValue);

    const request: CreateEmployeeRequest = {
      ...formValue,
      dateOfBirth: this.formatDateToString(formValue.dateOfBirth),
      hireDate: this.formatDateToString(formValue.hireDate),
      terminationDate: formValue.terminationDate ? this.formatDateToString(formValue.terminationDate) : null,
      address: formValue.address as any
    } as CreateEmployeeRequest;

    console.log('Create request payload:', request);

    this.apiService.createEmployee(request).subscribe({
      next: (_response: void) => {
        this.notificationService.success("Employee created successfully!");
        this.router.navigate(['/employees']);
      },
      error: (error: any) => {
        console.error('API Error:', error);
        this.notificationService.handleError(error);
      }
    });
  }

  private updateEmployee(): void {
    const formValue = this.employeeForm.getRawValue();
    console.log('Form value being sent:', formValue);

    const request: UpdateEmployeeRequest = {
      id: this.employeeId!,
      ...formValue,
      dateOfBirth: this.formatDateToString(formValue.dateOfBirth),
      hireDate: this.formatDateToString(formValue.hireDate),
      terminationDate: formValue.terminationDate ? this.formatDateToString(formValue.terminationDate) : null,
      address: formValue.address as any
    } as UpdateEmployeeRequest;

    console.log('Update request payload:', request);

    this.apiService.updateEmployee(request).subscribe({
      next: (_response: void) => {
        this.notificationService.success("Employee updated successfully!");
        this.router.navigate(['/employees']);
      },
      error: (error: any) => {
        console.error('API Error:', error);
        this.notificationService.handleError(error);
      }
    });
  }

  private formatDateToString(date: any): string {
    if (!date) return '';
    const d = new Date(date);
    const year = d.getFullYear();
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private getInvalidFields(): string[] {
    const invalid: string[] = [];
    const controls = this.employeeForm.controls;
    for (const name in controls) {
      if (controls[name as keyof typeof controls].invalid) {
        invalid.push(name);
      }
    }
    return invalid;
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
        // Check if department has changed
        const departmentChanged = this.selectedDepartment?.id !== result.id;
        
        this.selectedDepartment = result;
        this.employeeForm.patchValue({ departmentId: result.id });
        
        // If department changed, clear position and salary
        if (departmentChanged && this.selectedPosition) {
          this.selectedPosition = null;
          this.employeeForm.patchValue({ positionId: '', salary: 0 });
          
          // Disable salary field again
          const salaryControl = this.employeeForm.get('salary');
          if (salaryControl) {
            salaryControl.disable();
          }
        }
      }
    });
  }

  assignSchedule(): void {
    const dialogRef = this.dialog.open(ScheduleSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true
    });
    dialogRef.afterClosed().subscribe((result: ScheduleResponse | null) => {
      if (result) {
        this.selectedSchedule = result;
        this.employeeForm.patchValue({ scheduleId: result.id });
      }
    });
  }

  assignPosition(): void {
    // Check if department is selected
    if (!this.selectedDepartment) {
      this.notificationService.warning('Please select a department first');
      return;
    }

    const dialogRef = this.dialog.open(PositionSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true,
      data: { departmentId: this.selectedDepartment.id }
    });
    dialogRef.afterClosed().subscribe((result: PositionResponse | null) => {
      if (result) {
        this.selectedPosition = result;
        this.employeeForm.patchValue({ positionId: result.id });
        
        // Enable salary field and trigger validation
        const salaryControl = this.employeeForm.get('salary');
        if (salaryControl) {
          salaryControl.enable();
          // Reset salary to minimum if current value is outside range
          const currentSalary = salaryControl.value || 0;
          if (currentSalary < result.salaryMin || currentSalary > result.salaryMax) {
            salaryControl.setValue(null);
          }
          salaryControl.updateValueAndValidity();
        }
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/employees']);
  }
}
