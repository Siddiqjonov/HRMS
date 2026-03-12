import { DocumentType } from "../enums/document-type.enum";

export interface GetDocumentDownloadUrlResponse {
    sasUri: string;
}

export interface EmployeeDocumentsResponse {
    id: string;
    fileName: string;
    documentType: DocumentType;
    fileSizeInMb: number;
    uploadedAt: string;
    contentType?: string | null;
    blobUrl?: string | null;
    employeeName: string
    uploadedBy: string;
}