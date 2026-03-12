import { PaginationRequest } from "../common/pagination-request.model";
import { RequestStatus } from "../enums/request-status.enum";
import { RequestType } from "../enums/request-type.enum";


export interface GetAbsenceRequestsWithPaginationRequest extends PaginationRequest {
    employeeId?: string | null;
    managerId?: string | null;
    status?: RequestStatus | null;
    type?: RequestType | null;
    startDate?: string | null;
    endDate?: string | null;
}

export interface AbsenceRequestBriefResponse {
    id: string;
    employeeId: string;
    employeeName?: string;
    type: RequestType;
    status: RequestStatus;
    startDate: string;
    endDate: string;
    daysRequested: number;
    managerName?: string | null;
}