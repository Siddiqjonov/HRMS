import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { PositionListComponent } from "./position-list/position-list.component";
import { PositionDetailsComponent } from "./position-details/position-details.component";
import { PositionFormComponent } from "./position-form/position-form.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['HrManager'] },
        children: [
            { path: '', component: PositionListComponent },
            {
                path: 'new',
                component: PositionFormComponent
            },
            {
                path: ':id',
                component: PositionDetailsComponent
            },
            {
                path: ':id/edit',
                component: PositionFormComponent
            }
        ]
    }
];




