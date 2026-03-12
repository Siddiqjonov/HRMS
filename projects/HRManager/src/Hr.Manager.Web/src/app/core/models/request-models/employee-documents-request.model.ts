import { PaginationRequest } from "../common/pagination-request.model";
import { DocumentType } from "../enums/document-type.enum";

export interface DeleteEmployeeDocumentRequest {
    documentId: string;
}

export interface GetDocumentDownloadUrlRequest {
    documentId: string;
}

export interface GetEmployeeDocumentsRequest extends PaginationRequest {
    employeeId?: string | null;
    documentType?: DocumentType | null;
}

export interface UploadEmployeeDocumentRequest {
    employeeId: string;
    file: File;
    documentType: DocumentType;
}