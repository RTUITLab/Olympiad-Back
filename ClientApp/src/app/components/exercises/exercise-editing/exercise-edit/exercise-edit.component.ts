import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';

import { ActivatedRoute } from '@angular/router';
import { ParamMap } from '@angular/router/src/shared';
import { ExerciseEditService } from 'src/app/services/exercise-edit.service';
import { UserStateService } from 'src/app/services/user-state.service';
import { LoadingComponent } from 'src/app/components/helpers/loading-component';
import { SolutionViewModel } from 'src/app/models/ViewModels/SolutionViewModel';
import { ExerciseInfo } from 'src/app/models/Responses/ExerciseInfo';
import { ExerciseNewCondition } from 'src/app/models/ExerciseNewCondition';
import { ExerciseService } from 'src/app/services/exercise.service';
import { ToastrService } from 'ngx-toastr';
import { routerNgProbeToken } from '@angular/router/src/router_module';

@Component({
  selector: 'app-exercise-edit',
  templateUrl: './exercise-edit.component.html',
  styleUrls: ['./exercise-edit.component.scss']
})
export class ExerciseEditComponent extends LoadingComponent implements OnInit {

  //  variable for sending data to the server
  EditedExercise?: ExerciseInfo;
  constructor(
    private exerciseEditServise: ExerciseEditService,
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private router: Router,
    private toastr: ToastrService,
    private route: ActivatedRoute,
  ) {
    super();
  }

  ngOnInit() {
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.startLoading();
        this.exercisesService.getExercise(params.get('ExerciseID'))
          .subscribe(
            exInfo => {
              this.EditedExercise = exInfo;
              this.stopLoading();
            },
            fail => {
              console.log(fail);
              this.stopLoading();
            }
          );
      });
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
  sendEditedExercise() {
    this.startLoading();
    console.log(this.loading);
    this.exerciseEditServise.SendEditedExercise(this.EditedExercise).subscribe(
      _ => {
        this.toastr.success(`Задание изменено успешно`);
        this.stopLoading();
        this.router.navigate(['exercises', this.EditedExercise.Id]);
      },
      error => {
        this.toastr.error(error, `Ошибка добавления задания`);
        this.stopLoading();
      },
    );
  }
}
