import { Component, inject, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DocumentType } from '../../../core/models/enums/document-type.enum';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';
import { EmployeeSelectionDialogComponent } from '../employee-selection-dialog/employee-selection-dialog.component';

export interface DocumentFilterData {
  documentType?: DocumentType | null;
  selectedEmployeeId?: string | null;
  selectedEmployee?: EmployeesBriefResponse | null;
}

@Component({
  selector: 'app-document-filter-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './document-filter-dialog.component.html',
  styleUrl: './document-filter-dialog.component.css'
})
export class DocumentFilterDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<DocumentFilterDialogComponent>);
  private dialog = inject(MatDialog);

  selectedEmployeeId: string | null = null;
  selectedEmployee: EmployeesBriefResponse | null = null;
  documentType: DocumentType | null = null;

  documentTypes = [
    { value: null, label: 'All Types' },
    { value: DocumentType.Contract, label: 'Contract' },
    { value: DocumentType.Certificate, label: 'Certificate' },
    { value: DocumentType.IdentityDocument, label: 'Identity Document' },
    { value: DocumentType.MilitaryId, label: 'Military ID' }
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public data: DocumentFilterData) {
    if (data) {
      this.documentType = data.documentType ?? null;
      this.selectedEmployeeId = data.selectedEmployeeId || null;
      this.selectedEmployee = data.selectedEmployee || null;
    }
  }

  ngOnInit(): void {
    setTimeout(() => {
      const inputs = document.querySelectorAll('input');
      inputs.forEach(input => {
        if (input.closest('mat-dialog-container') && !input.classList.contains('selection-input')) {
          input.blur();
        }
      });
      const activeElement = document.activeElement as HTMLElement;
      if (activeElement && activeElement.tagName === 'INPUT' && !activeElement.classList.contains('selection-input')) {
        activeElement.blur();
      }
    }, 100);
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
        this.selectedEmployeeId = result.id;
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
    this.selectedEmployeeId = null;
  }

  onApply(): void {
    const filterData: DocumentFilterData = {
      documentType: this.documentType,
      selectedEmployeeId: this.selectedEmployeeId,
      selectedEmployee: this.selectedEmployee
    };
    this.dialogRef.close(filterData);
  }

  onClear(): void {
    this.documentType = null;
    this.selectedEmployeeId = null;
    this.selectedEmployee = null;
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}
