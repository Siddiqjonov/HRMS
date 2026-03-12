import { Routes } from "@angular/router";
import { AuthGuard } from "../../core/guards/auth.guard";
import { DocumentListComponent } from "./document-list/document-list.component";

export const routes: Routes = [
    {
        path: '',
        canActivate: [AuthGuard],
        data: { roles: ['Admin', 'HrManager'] },
        children: [
            { path: '', component: DocumentListComponent }
        ]
    }
]

