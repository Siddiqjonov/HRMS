import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { ScheduleListComponent } from "./schedule-list/schedule-list.component";
import { NewScheduleComponent } from "./new-schedule/new-schedule.component/new-schedule.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['HrManager'] },
        children: [
            { path: '', component: ScheduleListComponent },
            {
                path: 'new',
                component: NewScheduleComponent
            },
            {
                path: ':id/edit',
                component: NewScheduleComponent
            }
        ]
    }
];

