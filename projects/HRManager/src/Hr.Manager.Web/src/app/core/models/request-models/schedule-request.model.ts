import { DaysOfWeek } from "../enums/days-of-week.enum";

export interface CreateScheduleRequest {
    name: string;
    description: string;
    startTime: string;
    endTime: string;
    daysOfWeek: DaysOfWeek;
}

export interface DeleteScheduleRequest {
    id: string;
}

export interface GetScheduleByIdRequest {
    id: string;
}

export interface GetScheduleEmployeesRequest {
    scheduleId: string;
}

export interface UpdateScheduleRequest {
    id: string;
    name: string;
    description: string;
    startTime: string;
    endTime: string;
    daysOfWeek: DaysOfWeek;
}