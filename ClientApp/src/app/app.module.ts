import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoadingModule, ANIMATION_TYPES } from 'ngx-loading';

import { AppComponent } from './components/app.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './components/login/login.component';
import { UserStateService } from './services/user-state.service';
import { ExercisesListComponent } from './components/exercises/exercises-list/exercises-list.component';
import { ExerciseService } from './services/exercise.service';
import { AuthGuardService } from './services/auth-guard.service';
import { ExerciseInfoComponent } from './components/exercises/exercise-info/exercise-info.component';

const routes: Route[] = [
  {
    path: 'register',
    component: RegistrationComponent
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'exercises',
    component: ExercisesListComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'exercises/:ExerciseID',
    component: ExerciseInfoComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: '',
    pathMatch: 'prefix',
    redirectTo: '/register'
  },
];

@NgModule({
  declarations: [
    AppComponent,
    RegistrationComponent,
    HeaderComponent,
    LoginComponent,
    ExercisesListComponent,
    ExerciseInfoComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    FormsModule,
    HttpClientModule,
    LoadingModule.forRoot({
      animationType: ANIMATION_TYPES.cubeGrid
    })
  ],
  providers: [UserStateService, ExerciseService, AuthGuardService],
  bootstrap: [AppComponent]
})
export class AppModule { }
