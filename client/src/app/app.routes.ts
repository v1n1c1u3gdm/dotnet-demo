import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member/member-list/member-list.component';
import { MemberDetailComponent } from './member/member-detail/member-detail.component';
import { MessagesComponent } from './messages/messages.component';
import { WorkshopsComponent } from './workshops/workshops.component';
import { BikesComponent } from './bikes/bikes.component';
import { TravelPlanComponent } from './travel-plan/travel-plan.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'members', component: MemberListComponent},
    {path: 'members/:id', component: MemberDetailComponent},
    {path: 'messages', component: MessagesComponent},
    {path: 'workshops', component: WorkshopsComponent},
    {path: 'bikes', component: BikesComponent},
    {path: 'travel-plan', component: TravelPlanComponent},
    {path: '**', component: HomeComponent, pathMatch: "full"},
];
