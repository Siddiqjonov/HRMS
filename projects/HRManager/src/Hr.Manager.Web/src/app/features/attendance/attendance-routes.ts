import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { AttendanceListComponent } from "./attendance-list/attendance-list.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['Admin', 'HrManager'] },
        component: AttendanceListComponent
    }
];

