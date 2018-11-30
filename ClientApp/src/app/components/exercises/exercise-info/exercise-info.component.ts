import { Component, OnInit } from '@angular/core';
import { SolutionViewModel } from '../../../models/ViewModels/SolutionViewModel';
import { ExerciseService } from '../../../services/exercise.service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/';
import { ParamMap } from '@angular/router/src/shared';
import { Solution } from '../../../models/Solution';
import { SolutionStatus } from '../../../models/SolutionStatus';
import { SolutionStatusConverter } from '../../../models/Common/SolutionStatusConverter';
import { LanguageConverter } from '../../../models/Common/LanguageConverter';
import { ExerciseInfo } from '../../../models/Responses/ExerciseInfo';
import { Subject } from 'rxjs';
import { LoadingComponent } from '../../helpers/loading-component';
import { UserStateService } from '../../../services/user-state.service';
import { Router } from '@angular/router';
import { Exercise } from 'src/app/models/Exercise';
import { TaskEditService } from 'src/app/services/task-edit.service';



@Component({
  selector: 'app-exercise-info',
  templateUrl: './exercise-info.component.html',
  styleUrls: ['./exercise-info.component.css']
})
export class ExerciseInfoComponent extends LoadingComponent implements OnInit {



  constructor(
    private usersService: UserStateService,
    private exercisesService: ExerciseService,
    private taskEditServise:TaskEditService,
    private route: ActivatedRoute,
    private router: Router,) {
    super();
  }

  exerciseInfo: ExerciseInfo;
  availableLanguages = LanguageConverter.languages();

  //variable for sending data to the server
  EditedTask:Exercise;

  //variables for task data
  task_name:string;
  task_text:string;
  task_score:number;
  
  //variables for task html elements
  edit_task_btn_doc:HTMLElement;
  task_name_doc:HTMLElement;
  task_score_doc:HTMLElement;
  task_text_doc:HTMLElement;
  //variable for task_text view
  task_text_edit:boolean; 

  get submitDisabled() {
    return !this.model.File || !this.model.File.name.endsWith(LanguageConverter.fileExtension(this.model.Language));
  }
  model: SolutionViewModel = new SolutionViewModel();
  ngOnInit() {
     //set default value to variable for task_text view
    this.task_text_edit=false;
    //
    this.EditedTask={

    }
    this.model.Language = 'Java';
    this.route.paramMap
      .subscribe((params: ParamMap) => {
        this.model.ExerciseId = params.get('ExerciseID');
        console.log(this.model.ExerciseId);
        this.startLoading();
        this.exercisesService.getExercise(this.model.ExerciseId)
          .subscribe(
          exInfo => {
            exInfo.Solutions = exInfo.Solutions.reverse();
            this.exerciseInfo = exInfo;
            this.exerciseInfo
              .Solutions
              .filter(s => s.Status === SolutionStatus.InProcessing || s.Status === SolutionStatus.InQueue)
              .forEach(s => this.solutionCheckLoop(s.Id));
            // console.log(exInfo);
            this.stopLoading();
          },
          fail => {
            console.log(fail);
          }
          );
      });
  }
  setFile(event) {
    this.model.File = event.srcElement.files[0];
  }
  editTask() {
    // this.router.navigate(['edit-task']);
    console.log("EditTask()")
  //set variables value of documentID to interact with them
  this.edit_task_btn_doc = document.getElementById("editTask");
  this.task_name_doc = document.getElementById("task_name");
  this.task_score_doc = document.getElementById("task_score");
  this.task_text_doc = document.getElementById("task_text");

  if(this.task_name_doc.contentEditable == 'true'){
    
    //set default values for function for editing task content
    this.task_name_doc.contentEditable = 'false';
    this.task_score_doc.contentEditable = 'false';
    this.task_text_doc.contentEditable = 'false';
    
    //change button text
    this.edit_task_btn_doc.innerHTML="Изменить данные задания";
    this.edit_task_btn_doc.style.color="#c300ff";
    this.edit_task_btn_doc.style.borderColor="#c300ff";

    //change border of blocks with task data
    this.task_name_doc.style.border="unset";
    this.task_score_doc.style.border="unset";
    this.task_text_doc.style.border="unset";
    
    //get value from site to send them to the server
    this.task_name=this.task_name_doc.innerHTML.toString();
    this.task_score=parseInt(this.task_score_doc.innerHTML.toString(),10)
    this.task_text=this.task_text_doc.innerHTML.toString();
    console.log(this.task_name);
    console.log(this.task_score);
    console.log(this.task_text);
    this.EditedTask.ExerciseName=this.task_name;
    this.EditedTask.ExerciseTask=this.task_text;
    this.EditedTask.Score=this.task_score;
    console.log(this.EditedTask);
    //send EditedTask to the server
    this.taskEditServise.SendEditedTask(this.EditedTask);
    
  }
  else
  {
    //allow editing task data
    this.task_name_doc.contentEditable = 'true';
    this.task_score_doc.contentEditable = 'true';
    this.task_text_doc.contentEditable = 'true';
    
    //change button text
    this.edit_task_btn_doc.innerHTML="Сохранить";
    this.edit_task_btn_doc.style.color="#16c210";
    this.edit_task_btn_doc.style.borderColor="#16c210";

    //change border of blocks with task data
    this.task_name_doc.style.border="1px solid #00eeffe7";
    this.task_score_doc.style.border="1px solid #00eeffe7";
    this.task_text_doc.style.border="1px solid #00eeffe7";

    
    
  }
  
  
  
    

  }
  onSubmit() {
    this.exercisesService.sendSolution(this.model)
      .subscribe(
      success => {
        const f = () => this.solutionCheckLoop(success);
        f();
      }
      );
  }
  solutionCheckLoop(solutionId: string) {
    // console.log(this);
    this.exercisesService.checkSolution(solutionId).subscribe(
      solution => {
        const target = this.exerciseInfo.Solutions.find(s => s.Id === solution.Id);
        if (!target) {
          this.exerciseInfo.Solutions.unshift(solution);
        } else {
          target.Status = solution.Status;
        }
        if (solution.Status === SolutionStatus.InQueue ||
          solution.Status === SolutionStatus.InProcessing) {
          setTimeout(() => this.solutionCheckLoop(solutionId), 800);
        }
      });
  }

  solutionStatusPresent(status: SolutionStatus): string {
    return SolutionStatusConverter.convertToPretty(status);
  }

  prettyTime(time: string): string {
    return Solution.prettyTime(time);
  }

  normalLang(lang: string): string {
    return LanguageConverter.normalFromWeb(lang);
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}

