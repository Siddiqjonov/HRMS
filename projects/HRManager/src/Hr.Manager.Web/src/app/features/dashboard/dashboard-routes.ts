import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { DashboardMainComponent } from "./dashboard-main/dashboard-main.component";

export const routes: Routes = [
    {
        path: '',
        component: DashboardMainComponent,
        canActivate: [AuthGuard],
        data: { roles: ['Admin', 'HrManager', 'Employee'] }
    }
];

