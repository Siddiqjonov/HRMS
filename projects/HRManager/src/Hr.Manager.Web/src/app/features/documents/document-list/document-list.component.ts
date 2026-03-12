import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { skip } from 'rxjs/operators';
import { DocumentService } from '../../../core/services/document.service';
import { ApiService } from '../../../core/services/api.service';
import { NotificationService } from '../../../core/services/notification.service';
import { EmployeeDocumentsResponse } from '../../../core/models/response-models/employee-document-response.model';
import { DocumentType } from '../../../core/models/enums/document-type.enum';
import { DocumentUploadDialogComponent } from '../document-upload-dialog/document-upload-dialog.component';
import { DocumentFilterDialogComponent, DocumentFilterData } from '../document-filter-dialog/document-filter-dialog.component';
import { EmployeeSelectionDialogComponent } from '../employee-selection-dialog/employee-selection-dialog.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { ErrorDisplayComponent } from '../../../shared/components/error-display/error-display.component';
import { DataTableComponent } from '../../../shared/components/data-table/data-table.component';
import { PaginatedModel } from '../../../core/models/common/paginated.model';
import { EmployeesBriefResponse } from '../../../core/models/response-models/employee-response.model';

interface DocumentWithEmployee extends EmployeeDocumentsResponse {
  documentTypeLabel?: string;
  fileSizeDisplay?: string;
  uploadedAtDisplay?: string;
}

@Component({
  selector: 'app-document-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    ErrorDisplayComponent,
    DataTableComponent
  ],
  templateUrl: './document-list.component.html',
  styleUrl: './document-list.component.css'
})
export class DocumentListComponent implements OnInit {
  private documentService = inject(DocumentService);
  private apiService = inject(ApiService);
  private notificationService = inject(NotificationService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private location = inject(Location);

  dataSource = new MatTableDataSource<DocumentWithEmployee>([]);
  displayedColumns = [
    { key: 'fileName', label: 'File Name' },
    { key: 'documentTypeLabel', label: 'Type' },
    { key: 'fileSizeDisplay', label: 'Size' },
    { key: 'uploadedAtDisplay', label: 'Upload Date' },
    { key: 'employeeName', label: 'Employee' },
    { key: 'actions', label: 'Actions' }
  ];
  
  paginationResult: PaginatedModel<DocumentWithEmployee> | null = null;
  
  isLoading = false;
  errorMessage: string | null = null;
  
  // Filter and pagination properties
  currentPage = 1;
  pageSize = 10;
  selectedEmployeeId: string | null = null;
  selectedEmployee: EmployeesBriefResponse | null = null;
  selectedDocumentType: DocumentType | null = null;
  hasActiveFilters = false;

  ngOnInit(): void {
    this.loadDocuments();
    
    // Subscribe to document updates - skip(1) to avoid double loading on init
    this.documentService.documentUpdated$.pipe(skip(1)).subscribe(() => {
      this.loadDocuments();
    });
  }

  loadDocuments(): void {
    this.isLoading = true;
    this.errorMessage = null;
    
    // Load documents from backend with filters and pagination
    this.documentService.getAllDocuments(
      this.currentPage,
      this.pageSize,
      this.selectedEmployeeId,
      this.selectedDocumentType
    ).subscribe({
      next: (response) => {
        this.paginationResult = {
          ...response,
          items: response.items.map(doc => this.transformDocument(doc))
        };
        this.dataSource.data = this.paginationResult.items;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load documents';
        this.notificationService.handleError(error);
        this.isLoading = false;
      }
    });
  }

  private transformDocument(doc: any): DocumentWithEmployee {
    return {
      ...doc,
      documentTypeLabel: this.documentService.getDocumentTypeLabel(doc.documentType),
      fileSizeDisplay: this.documentService.formatFileSize(doc.fileSizeInMb),
      uploadedAtDisplay: this.formatDate(doc.uploadedAt)
    };
  }

  getDocumentTypeClass(type: DocumentType): string {
    switch (type) {
      case DocumentType.Contract:
        return 'doc-type-contract';
      case DocumentType.Certificate:
        return 'doc-type-certificate';
      case DocumentType.IdentityDocument:
        return 'doc-type-identity';
      case DocumentType.MilitaryId:
        return 'doc-type-military';
      default:
        return 'doc-type-default';
    }
  }


  applyFilters(): void {
    // Update hasActiveFilters flag
    this.hasActiveFilters = !!this.selectedEmployeeId || this.selectedDocumentType !== null;
    
    // Reset to first page when filters change
    this.currentPage = 1;
    
    // Reload documents from backend with new filters
    this.loadDocuments();
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1; // PageEvent is 0-based, backend is 1-based
    this.pageSize = event.pageSize;
    
    // Load documents from backend with new pagination
    this.loadDocuments();
  }

  openFilterDialog(): void {
    const dialogRef = this.dialog.open(DocumentFilterDialogComponent, {
      width: '380px',
      maxWidth: '90vw',
      data: {
        documentType: this.selectedDocumentType,
        selectedEmployeeId: this.selectedEmployeeId,
        selectedEmployee: this.selectedEmployee
      }
    });

    dialogRef.afterClosed().subscribe((result: DocumentFilterData | null) => {
      if (result !== null) {
        this.selectedEmployeeId = result.selectedEmployeeId || null;
        this.selectedEmployee = result.selectedEmployee || null;
        this.selectedDocumentType = result.documentType ?? null;
        this.applyFilters();
      }
    });
  }

  onClearFilters(): void {
    this.selectedEmployeeId = null;
    this.selectedEmployee = null;
    this.selectedDocumentType = null;
    this.applyFilters();
  }

  clearEmployee(): void {
    this.selectedEmployeeId = null;
    this.selectedEmployee = null;
    this.applyFilters();
  }

  clearDocumentType(): void {
    this.selectedDocumentType = null;
    this.applyFilters();
  }

  onUploadDocument(): void {
    // Update URL to show upload route
    this.location.go('/documents/upload');

    const dialogRef = this.dialog.open(DocumentUploadDialogComponent, {
      width: '500px',
      maxWidth: '90vw',
      data: { employeeId: null, employeeName: null }
    });

    dialogRef.afterClosed().subscribe(result => {
      // Navigate back to documents list
      this.location.go('/documents');
      
      if (result) {
        this.loadDocuments();
      }
    });
  }

  onViewDocument(document: DocumentWithEmployee): void {
    // View is now handled by download
    this.onDownloadDocument(document);
  }

  onDownloadDocument(doc: DocumentWithEmployee): void {
    this.apiService.getDocumentDownloadUrl({ documentId: doc.id }).subscribe({
      next: (response) => {
        // Create a hidden link and trigger download
        const link = window.document.createElement('a');
        link.href = response.sasUri;
        link.download = doc.fileName;
        link.style.display = 'none';
        window.document.body.appendChild(link);
        link.click();
        window.document.body.removeChild(link);
        
        this.notificationService.success('Downloading document...');
      },
      error: (error) => {
        this.notificationService.handleError(error);
      }
    });
  }

  onDeleteDocument(document: DocumentWithEmployee): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        message: `Are you sure you want to delete "${document.fileName}"? This action cannot be undone.`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isLoading = true;
        this.documentService.deleteDocument(document.id).subscribe({
          next: () => {
            this.notificationService.success('Document deleted successfully');
            this.loadDocuments();
          },
          error: (error) => {
            this.notificationService.handleError(error);
            this.isLoading = false;
          }
        });
      }
    });
  }

  getDocumentTypeLabel(type: DocumentType): string {
    return this.documentService.getDocumentTypeLabel(type);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric'
    });
  }
}
