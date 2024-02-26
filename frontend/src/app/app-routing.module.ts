import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './shared/auth.guard';
import { JobPageComponent } from './job-page/job-page.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: '/login' },
  { path: 'registration', pathMatch: 'full', redirectTo: '/registration' },
  // { path: 'jobs', pathMatch: 'full', redirectTo: '/jobs' },
  {path: 'jobs', component: JobPageComponent, canActivate: [AuthGuard]}
  // { path: '**', pathMatch: 'full', redirectTo: '/login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
