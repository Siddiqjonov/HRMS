import { Injectable, inject } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { PaginatedModel } from '../models/common/paginated.model';
import { EmployeeDocumentsResponse } from '../models/response-models/employee-document-response.model';
import { GetEmployeeDocumentsRequest, UploadEmployeeDocumentRequest, DeleteEmployeeDocumentRequest } from '../models/request-models/employee-documents-request.model';
import { DocumentType } from '../models/enums/document-type.enum';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiService = inject(ApiService);
  
  // Subject to notify when documents are updated
  private documentUpdatedSubject = new BehaviorSubject<void>(undefined);
  documentUpdated$ = this.documentUpdatedSubject.asObservable();

  uploadDocument(request: UploadEmployeeDocumentRequest): Observable<EmployeeDocumentsResponse> {
    return this.apiService.uploadEmployeeDocument(request).pipe(
      tap(() => this.documentUpdatedSubject.next())
    );
  }

  getAllDocuments(pageNumber: number = 1, pageSize: number = 10, employeeId?: string | null, documentType?: DocumentType | null): Observable<PaginatedModel<EmployeeDocumentsResponse>> {
    const request: GetEmployeeDocumentsRequest = {
      employeeId: employeeId || null,
      pageNumber,
      pageSize,
      documentType
    };
    
    return this.apiService.getDocuments(request);
  }

  downloadDocument(documentId: string): Observable<void> {
    return new Observable(observer => {
      this.apiService.getDocumentDownloadUrl({ documentId }).subscribe({
        next: (response) => {
          // Open the SAS URI in a new window to download
          window.open(response.sasUri, '_blank');
          observer.next();
          observer.complete();
        },
        error: (error) => observer.error(error)
      });
    });
  }

  deleteDocument(documentId: string): Observable<void> {
    return this.apiService.deleteEmployeeDocument({ documentId }).pipe(
      tap(() => this.documentUpdatedSubject.next())
    );
  }

  getDocumentTypeLabel(type: DocumentType): string {
    switch (type) {
      case DocumentType.Contract:
        return 'Contract';
      case DocumentType.Certificate:
        return 'Certificate';
      case DocumentType.IdentityDocument:
        return 'Identity Document';
      case DocumentType.MilitaryId:
        return 'Military ID';
      default:
        return 'Unknown';
    }
  }

  getDocumentTypeIcon(type: DocumentType): string {
    switch (type) {
      case DocumentType.Contract:
        return 'description';
      case DocumentType.Certificate:
        return 'card_membership';
      case DocumentType.IdentityDocument:
        return 'badge';
      case DocumentType.MilitaryId:
        return 'military_tech';
      default:
        return 'insert_drive_file';
    }
  }

  formatFileSize(sizeInMb: number): string {
    if (sizeInMb < 0.001) {
      return `${(sizeInMb * 1024).toFixed(2)} KB`;
    }
    return `${sizeInMb.toFixed(2)} MB`;
  }

  validateFile(file: File): { valid: boolean; error?: string } {
    const maxSizeInMB = 10;
    const allowedTypes = [
      'application/pdf',
      'image/jpeg',
      'image/jpg',
      'image/png',
      'application/msword',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    ];

    if (!allowedTypes.includes(file.type)) {
      return { 
        valid: false, 
        error: 'Invalid file type. Allowed types: PDF, JPG, PNG, DOC, DOCX' 
      };
    }

    const fileSizeInMB = file.size / (1024 * 1024);
    if (fileSizeInMB > maxSizeInMB) {
      return { 
        valid: false, 
        error: `File size exceeds ${maxSizeInMB}MB limit` 
      };
    }

    return { valid: true };
  }

  canPreviewFile(contentType: string | null | undefined): boolean {
    if (!contentType) return false;
    return ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png'].includes(contentType);
  }
}

