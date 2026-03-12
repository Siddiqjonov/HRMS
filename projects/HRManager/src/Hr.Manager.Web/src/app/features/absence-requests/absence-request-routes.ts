import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { AbsenceRequestListComponent } from "./absence-request-list/absence-request-list.component";
import { MyAbsenceComponent } from "./my-absence/my-absence.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['HrManager'] },
        children: [
            { path: '', component: AbsenceRequestListComponent },
            { path: 'new', component: MyAbsenceComponent }
        ]
    }
];

