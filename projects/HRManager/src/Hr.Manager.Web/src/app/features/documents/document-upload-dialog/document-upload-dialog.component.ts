import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DocumentService } from '../../../core/services/document.service';
import { NotificationService } from '../../../core/services/notification.service';
import { DocumentType } from '../../../core/models/enums/document-type.enum';
import { EmployeeSelectionDialogComponent } from '../employee-selection-dialog/employee-selection-dialog.component';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { finalize } from 'rxjs';
import { MatInputModule } from '@angular/material/input';

export interface DocumentUploadDialogData {
  employeeId: string | null;
  employeeName?: string | null;
}

@Component({
  selector: 'app-document-upload-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatInputModule
  ],
  templateUrl: './document-upload-dialog.component.html',
  styleUrl: './document-upload-dialog.component.css'
})
export class DocumentUploadDialogComponent {
  private fb = inject(FormBuilder);
  private documentService = inject(DocumentService);
  private notificationService = inject(NotificationService);
  private dialogRef = inject(MatDialogRef<DocumentUploadDialogComponent>);
  private dialog = inject(MatDialog);

  uploadForm: FormGroup;
  selectedFile: File | null = null;
  selectedEmployee: EmployeesBriefResponse | null = null;
  isDragging = false;
  isUploading = false;
  uploadProgress = 0;

  documentTypes = [
    { value: DocumentType.Contract, label: 'Contract' },
    { value: DocumentType.Certificate, label: 'Certificate' },
    { value: DocumentType.IdentityDocument, label: 'Identity Document' },
    { value: DocumentType.MilitaryId, label: 'Military ID' }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public data: DocumentUploadDialogData) {
    this.uploadForm = this.fb.group({
      documentType: ['', Validators.required]
    });

    if (data.employeeId && data.employeeName) {
      this.selectedEmployee = {
        id: data.employeeId,
        fullName: data.employeeName
      } as EmployeesBriefResponse;
    }
  }

  openEmployeeSelectionDialog(): void {
    const dialogRef = this.dialog.open(EmployeeSelectionDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      disableClose: false,
      autoFocus: true
    });

    dialogRef.afterClosed().subscribe((result: EmployeesBriefResponse | null) => {
      if (result) {
        this.selectedEmployee = result;
      }
    });
  }

  onEmployeeInputClick(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.openEmployeeSelectionDialog();
  }

  onEmployeeInputKeydown(event: KeyboardEvent): void {
    event.preventDefault();
    if (event.key === 'Enter' || event.key === ' ') {
      this.openEmployeeSelectionDialog();
    }
  }

  clearEmployee(event?: Event): void {
    if (event) {
      event.preventDefault();
      event.stopPropagation();
    }
    this.selectedEmployee = null;
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFileSelection(files[0]);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.handleFileSelection(input.files[0]);
    }
  }

  handleFileSelection(file: File): void {
    const validation = this.documentService.validateFile(file);
    
    if (!validation.valid) {
      this.notificationService.handleError({ error: { message: validation.error || 'Invalid file' } });
      return;
    }

    this.selectedFile = file;
  }

  removeFile(): void {
    this.selectedFile = null;
  }

  getFileIcon(file: File): string {
    const type = file.type;
    if (type.includes('pdf')) return 'picture_as_pdf';
    if (type.includes('image')) return 'image';
    if (type.includes('word')) return 'description';
    return 'insert_drive_file';
  }

  formatFileSize(bytes: number): string {
    const mb = bytes / (1024 * 1024);
    return this.documentService.formatFileSize(mb);
  }

  onSubmit(): void {
    if (this.uploadForm.invalid || !this.selectedFile || !this.selectedEmployee) {
      this.notificationService.handleError({ 
        error: { message: 'Please select an employee, file, and document type' } 
      });
      return;
    }

    this.isUploading = true;
    this.uploadProgress = 0;

    // Simulate progress (since we don't have actual progress from the API)
    const progressInterval = setInterval(() => {
      if (this.uploadProgress < 90) {
        this.uploadProgress += 10;
      }
    }, 200);

    const request = {
      employeeId: this.selectedEmployee.id,
      file: this.selectedFile,
      documentType: this.uploadForm.value.documentType
    };

    this.documentService.uploadDocument(request)
      .pipe(finalize(() => {
        clearInterval(progressInterval);
        this.isUploading = false;
      }))
      .subscribe({
        next: (response) => {
          this.uploadProgress = 100;
          this.notificationService.success('Document uploaded successfully');
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.uploadProgress = 0;
          this.notificationService.handleError(error);
        }
      });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
