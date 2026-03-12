import { Component, inject, OnInit } from '@angular/core';
import { MatFormField, MatLabel, MatError, MatSuffix } from "@angular/material/form-field";
import { NotificationService } from '../../../../core/services/notification.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { MatIcon } from "@angular/material/icon";
import { ApiService } from '../../../../core/services/api.service';
import { CreateScheduleRequest, UpdateScheduleRequest } from '../../../../core/models/request-models/schedule-request.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ScheduleResponse } from '../../../../core/models/response-models/schedule-response.model';
import { DaysOfWeek } from '../../../../core/models/enums/days-of-week.enum';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { DateAdapter, provideNativeDateAdapter } from '@angular/material/core';

@Component({
  selector: 'app-new-schedule',
  imports: [
    MatFormField,
    MatLabel,
    MatError,
    MatSuffix,
    ReactiveFormsModule,
    MatInput,
    MatButton,
    MatIcon,
    MatCheckboxModule,
    CommonModule,
    MatTimepickerModule
  ],
  providers: [provideNativeDateAdapter()],
  templateUrl: './new-schedule.component.html',
  styleUrl: './new-schedule.component.css'
})
export class NewScheduleComponent implements OnInit {
  private readonly adapter = inject<DateAdapter<unknown, unknown>>(DateAdapter);
  private notificationService = inject(NotificationService);
  private apiService = inject(ApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  
  isEditMode = false;
  scheduleId: string | null = null;

  scheduleForm = new FormGroup({
    name: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(100), Validators.minLength(3)]
    }),
    description: new FormControl('', {
      validators: [Validators.required, Validators.maxLength(250)]
    }),
    startTime: new FormControl<Date | string | null>(null, {
      validators: [Validators.required]
    }),
    endTime: new FormControl<Date | string | null>(null, {
      validators: [Validators.required]
    }),
    monday: new FormControl(false),
    tuesday: new FormControl(false),
    wednesday: new FormControl(false),
    thursday: new FormControl(false),
    friday: new FormControl(false),
    saturday: new FormControl(false),
    sunday: new FormControl(false),
  });

  get nameIsRequired(): boolean {
    const nameControl = this.scheduleForm.controls.name;
    return nameControl.touched && nameControl.hasError('required');
  }

  get nameHasLengthError(): boolean {
    const nameControl = this.scheduleForm.controls.name;
    return nameControl.touched &&
      (nameControl.hasError('minlength') || nameControl.hasError('maxlength'));
  }

  get descriptionIsRequired(): boolean {
    const descControl = this.scheduleForm.controls.description;
    return descControl.touched && descControl.hasError('required');
  }

  get descriptionHasLengthError(): boolean {
    const descControl = this.scheduleForm.controls.description;
    return descControl.touched && descControl.hasError('maxlength');
  }

  get startTimeIsRequired(): boolean {
    const control = this.scheduleForm.controls.startTime;
    return control.touched && control.hasError('required');
  }

  get endTimeIsRequired(): boolean {
    const control = this.scheduleForm.controls.endTime;
    return control.touched && control.hasError('required');
  }

  ngOnInit(): void {
    this.adapter.setLocale('en-GB');
    
    this.scheduleId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.scheduleId;

    if (this.isEditMode && this.scheduleId) {
      this.loadScheduleData(this.scheduleId);
    }
  }

  loadScheduleData(id: string): void {
    this.apiService.getScheduleById({ id }).subscribe({
      next: (schedule: ScheduleResponse) => {
        this.scheduleForm.patchValue({
          name: schedule.name,
          description: schedule.description,
          startTime: this.parseTimeStringToDate(schedule.startTime),
          endTime: this.parseTimeStringToDate(schedule.endTime),
          monday: !!(schedule.daysOfWeek & DaysOfWeek.Monday),
          tuesday: !!(schedule.daysOfWeek & DaysOfWeek.Tuesday),
          wednesday: !!(schedule.daysOfWeek & DaysOfWeek.Wednesday),
          thursday: !!(schedule.daysOfWeek & DaysOfWeek.Thursday),
          friday: !!(schedule.daysOfWeek & DaysOfWeek.Friday),
          saturday: !!(schedule.daysOfWeek & DaysOfWeek.Saturday),
          sunday: !!(schedule.daysOfWeek & DaysOfWeek.Sunday),
        });
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error loading schedule:', error);
      }
    });
  }

  parseTimeStringToDate(timeString: string): Date | null {
    if (!timeString) return null;
    
    // Parse HH:mm:ss or HH:mm format
    const timeParts = timeString.split(':');
    if (timeParts.length >= 2) {
      const hours = parseInt(timeParts[0], 10);
      const minutes = parseInt(timeParts[1], 10);
      const seconds = timeParts.length > 2 ? parseInt(timeParts[2], 10) : 0;
      
      const date = new Date();
      date.setHours(hours, minutes, seconds, 0);
      return date;
    }
    
    return null;
  }

  getDaysOfWeekValue(): DaysOfWeek {
    let daysValue = DaysOfWeek.None;
    const formValue = this.scheduleForm.getRawValue();

    if (formValue.monday) daysValue |= DaysOfWeek.Monday;
    if (formValue.tuesday) daysValue |= DaysOfWeek.Tuesday;
    if (formValue.wednesday) daysValue |= DaysOfWeek.Wednesday;
    if (formValue.thursday) daysValue |= DaysOfWeek.Thursday;
    if (formValue.friday) daysValue |= DaysOfWeek.Friday;
    if (formValue.saturday) daysValue |= DaysOfWeek.Saturday;
    if (formValue.sunday) daysValue |= DaysOfWeek.Sunday;

    return daysValue;
  }

  formatTimeForApi(time: Date | string | null): string {
    if (!time) return '';
    
    if (typeof time === 'string') {
      if (/^\d{2}:\d{2}(:\d{2})?$/.test(time)) {
        return time;
      }
    }
    
    if (time instanceof Date) {
      const hours = time.getHours().toString().padStart(2, '0');
      const minutes = time.getMinutes().toString().padStart(2, '0');
      const seconds = time.getSeconds().toString().padStart(2, '0');
      return `${hours}:${minutes}:${seconds}`;
    }
    
    return '';
  }

  updateSchedule(): void {
    if (!this.scheduleId) return;

    const formValue = this.scheduleForm.getRawValue();
    const updateRequest: UpdateScheduleRequest = {
      id: this.scheduleId,
      name: formValue.name!,
      description: formValue.description!,
      startTime: this.formatTimeForApi(formValue.startTime),
      endTime: this.formatTimeForApi(formValue.endTime),
      daysOfWeek: this.getDaysOfWeekValue()
    };

    this.apiService.updateSchedule(updateRequest).subscribe({
      next: () => {
        this.scheduleForm.reset();
        this.scheduleForm.markAsPristine();
        this.scheduleForm.markAsUntouched();
        this.notificationService.success('Schedule updated successfully!');
        this.router.navigate(['/schedules']);
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error updating schedule:', error);
      }
    });
  }

  createSchedule(): void {
    const formValue = this.scheduleForm.getRawValue();
    const createRequest: CreateScheduleRequest = {
      name: formValue.name!,
      description: formValue.description!,
      startTime: this.formatTimeForApi(formValue.startTime),
      endTime: this.formatTimeForApi(formValue.endTime),
      daysOfWeek: this.getDaysOfWeekValue()
    };

    this.apiService.createSchedule(createRequest).subscribe({
      next: () => {
        this.scheduleForm.reset();
        this.scheduleForm.markAsPristine();
        this.scheduleForm.markAsUntouched();
        this.notificationService.success('Schedule created successfully!');
        this.router.navigate(['/schedules']);
      },
      error: (error) => {
        this.notificationService.handleError(error);
        console.error('Error creating schedule:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.scheduleForm.invalid) {
      return;
    }

    if (this.isEditMode && this.scheduleId) {
      this.updateSchedule();
    } else {
      this.createSchedule();
    }
  }
}

