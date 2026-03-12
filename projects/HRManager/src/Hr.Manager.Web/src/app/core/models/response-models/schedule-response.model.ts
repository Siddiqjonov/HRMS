import { DaysOfWeek } from "../enums/days-of-week.enum";

export interface ScheduleResponse {
    id: string;
    name: string;
    description: string;
    startTime: string;
    endTime: string;
    daysOfWeek: DaysOfWeek;
}