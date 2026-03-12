import { PaginationRequest } from "../common/pagination-request.model";
import { RequestStatus } from "../enums/request-status.enum";
import { RequestType } from "../enums/request-type.enum";

export interface CreateAbsenceRequest {
    employeeId: string;     
    requestType: RequestType;
    startDate: string;       
    endDate: string;         
    reason?: string | null;
}

export interface ProcessAbsenceRequest {
    id: string;              
    approved: boolean;
    reason?: string | null;
}

export interface GetAbsenceRequestsWithPaginationRequest extends PaginationRequest {
    employeeId?: string | null;
    status?: RequestStatus | null;
    type?: RequestType | null;
    startDate?: string | null;  
    endDate?: string | null;    
}