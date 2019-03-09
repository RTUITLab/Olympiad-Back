import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgxLoadingModule, ngxLoadingAnimationTypes } from 'ngx-loading';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule, MatAutocompleteModule } from '@angular/material';
import { MatMenuModule } from '@angular/material/menu';
import {MatSelectModule} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatFormFieldModule} from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import {MatDividerModule} from '@angular/material/divider';







import { AppComponent } from './components/app.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './components/login/login.component';
import { UserStateService } from './services/user-state.service';
import { ExerciseService } from './services/exercise.service';
import { AuthGuardService } from './services/ComponentActivators/auth-guard.service';
import { ExerciseInfoComponent } from './components/exercises/exercise-info/exercise-info.component';
import { OverviewComponent } from './components/overview/overview.component';
import { MarkdownModule, MarkedRenderer, MarkedOptions } from 'ngx-markdown';
import { ExerciseInoutComponent } from './components/exercises/exercise-inout/exercise-inout.component';
import { UserInfoComponent } from './components/user-info/user-info.component';

import { AddExerciseComponent } from './components/exercises/add-exercise/add-exercise.component';
import { ChallengesService } from './services/challenges.service';
import { ExerciseStateService } from './services/exercise-state.service';
import { ChallengeInfoComponent } from './components/challenges/challenge-info/challenge-info.component';
import { ExerciseEditComponent } from './components/exercises/exercise-editing/exercise-edit/exercise-edit.component';
import { ConditionEditComponent } from './components/exercises/exercise-editing/condition-edit/condition-edit.component';
import { ReCaptchaModule } from 'angular2-recaptcha';
import { NotAuthGuardGuard as NotAuthGuard } from './services/ComponentActivators/not-auth.guard';
import { ChallengeAddComponent } from './components/challenges/challenge-add/challenge-add.component';
import { UsersComponent } from './components/users/users.component';
import { PersonalPageComponent } from './components/personal-page/personal-page.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { ChallengeEditComponent } from './components/challenges/challenge-edit/challenge-edit.component';
import { SelectChallengeComponent } from './components/select-challenge/select-challenge.component';
import { AdminFunctionsComponent } from './components/admin-functions/admin-functions.component';
import { AboutComponent } from './components/about/about.component';
import { UsersGenerationComponent } from './components/users-generation/users-generation.component';
import { ChallengeDeadlineComponent } from './components/select-challenge/challenge-deadline/challenge-deadline.component';

const routes: Route[] = [
  {
    path: 'register',
    component: RegistrationComponent,
    canActivate: [NotAuthGuard]
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [NotAuthGuard]
  },
  {
    path: 'about',
    component: AboutComponent
  },
  {
    path: 'overview',
    component: OverviewComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'admin-functions',
    component: AdminFunctionsComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'add-exercise',
    component: AddExerciseComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'exercises/edit/:ExerciseID',
    component: ExerciseEditComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'exercises/:ExerciseID',
    component: ExerciseInfoComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'challenges/:ChallengeId',
    component: ChallengeInfoComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'add-challenge',
    component: ChallengeAddComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'edit-challenge/:ChallengeId',
    component: ChallengeEditComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'user/:userId',
    component: PersonalPageComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'users-list',
    component: UsersComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'users-generation',
    component: UsersGenerationComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: '/overview'
  },
  {
    path: '**',
    redirectTo: '/overview'
  }
];

@NgModule({
  declarations: [
    AppComponent,
    RegistrationComponent,
    HeaderComponent,
    LoginComponent,
    ExerciseInfoComponent,
    OverviewComponent,
    ExerciseInoutComponent,
    UserInfoComponent,
    AddExerciseComponent,
    ExerciseEditComponent,
    ChallengeInfoComponent,
    ConditionEditComponent,
    ChallengeAddComponent,
    UsersComponent,
    ChallengeEditComponent,
    PersonalPageComponent,
    SelectChallengeComponent,
    AdminFunctionsComponent,
    AboutComponent,
    UsersGenerationComponent,
    ChallengeDeadlineComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    FormsModule,
    MatTooltipModule,
    MatDividerModule,
    MatSelectModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot({
    }), // ToastrModule added
    HttpClientModule,
    NgxLoadingModule.forRoot({
      animationType: ngxLoadingAnimationTypes.cubeGrid
    }),
    MarkdownModule.forRoot({
      markedOptions: {
        provide: MarkedOptions,
        useFactory: markedOptionsFactory,
      },
    }),
    MatTableModule,
    MatMenuModule,
    MatButtonModule,
    MatInputModule,
    MatAutocompleteModule,
    ReCaptchaModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
  ],
  providers: [
    UserStateService,
    ExerciseStateService,
    ExerciseService,
    ChallengesService,
    AuthGuardService,
    Title,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
export function markedOptionsFactory(): MarkedOptions {
  const renderer = new MarkedRenderer();

  renderer.blockquote = (text: string) => {
    return '<blockquote style="border-color:rgb(0,140,186); color:black">' + text + '</blockquote>';
  };

  return {
    renderer: renderer,
    gfm: true,
    tables: true,
    breaks: false,
    pedantic: false,
    sanitize: false,
    smartLists: true,
    smartypants: false,
  };
}
