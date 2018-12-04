import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { Routes, RouterModule, Route } from '@angular/router';
import { FormsModule , ReactiveFormsModule  } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgxLoadingModule, ngxLoadingAnimationTypes  } from 'ngx-loading';
import {MatTableModule} from '@angular/material/table';
import { MatButtonModule } from '@angular/material';
import {MatInputModule} from '@angular/material/input';




import { AppComponent } from './components/app.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { HeaderComponent } from './components/header/header.component';
import { LoginComponent } from './components/login/login.component';
import { UserStateService } from './services/user-state.service';
import { ExercisesListComponent } from './components/exercises/exercises-list/exercises-list.component';
import { ExerciseService } from './services/exercise.service';
import { AuthGuardService } from './services/auth-guard.service';
import { ExerciseInfoComponent } from './components/exercises/exercise-info/exercise-info.component';
import { MenuComponent } from './components/menu/menu.component';
import { OverviewComponent } from './components/overview/overview.component';
import { MarkdownModule, MarkedRenderer, MarkedOptions } from 'ngx-markdown';
import { ExerciseInoutComponent } from './components/exercises/exercise-inout/exercise-inout.component';
import { UserInfoComponent } from './components/user-info/user-info.component';

import { ExerciseEditComponent } from './components/exercises/exercise-edit/exercise-edit.component';
import { AddExerciseInOutComponent } from './components/exercises/add-exercise-in-out/add-exercise-in-out.component';
import { AddExerciseComponent } from './components/exercises/add-exercise/add-exercise.component';


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
    path: 'overview',
    component: OverviewComponent
  },
  {
    path: 'exercises',
    component: ExercisesListComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'add-exercise',
    component: AddExerciseComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'exercises/edit-task/:ExerciseID',
    component: ExerciseEditComponent,
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
    redirectTo: '/overview'
  },
];

@NgModule({
  declarations: [
    AppComponent,
    RegistrationComponent,
    HeaderComponent,
    LoginComponent,
    ExercisesListComponent,
    ExerciseInfoComponent,
    MenuComponent,
    OverviewComponent,
    ExerciseInoutComponent,
    UserInfoComponent,
    AddExerciseComponent,
    ExerciseEditComponent,
    AddExerciseInOutComponent,

  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    FormsModule,
    ReactiveFormsModule,
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
    MatButtonModule,
    MatInputModule
  ],
  providers: [UserStateService, ExerciseService, AuthGuardService],
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
