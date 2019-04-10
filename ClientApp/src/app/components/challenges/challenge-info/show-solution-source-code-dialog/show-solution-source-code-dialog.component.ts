import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { LanguageConverter } from 'src/app/models/Common/LanguageConverter';
import { ExerciseService } from 'src/app/services/exercise.service';
import { SolutionLog } from 'src/app/models/SolutionLog';
import { CheckedSolution } from 'src/app/models/CheckedSolution';
import { element } from '@angular/core/src/render3';

@Component({
  selector: 'app-show-solution-source-code-dialog',
  templateUrl: './show-solution-source-code-dialog.component.html',
  styleUrls: ['./show-solution-source-code-dialog.component.scss']
})
export class ShowSolutionSourceCodeDialogComponent implements OnInit {
  solutionData: CheckedSolution;
  solutionLogs: SolutionLog[];
  allSolutions: CheckedSolution[];
  constructor(
    public dialogRef: MatDialogRef<ShowSolutionSourceCodeDialogComponent>,
    public exerciseService: ExerciseService,
    @Inject(MAT_DIALOG_DATA) public data: CheckedSolution,
    ) {}
   ngOnInit() {
      this.getBestSolution();
      this.getAllSolutions();
    }
  async getAllSolutions(){
    this.allSolutions = await this.exerciseService.getUserExerciseSolutions(this.solutionData.ExerciseId,this.solutionData.UserId);
    this.allSolutions = this.solutionTimeConverter(this.allSolutions);
    this.allSolutions = this.solutionTimeSort(this.allSolutions); 
  }
  getBestSolution(){
    this.solutionData = this.data['solutionData'];
    this.solutionData['Language'] = LanguageConverter.normalFromWeb(this.data['solutionData'].Language);
    this.getSolutionLogs(this.data['solutionData'].Id);
  }
  async getSolutionLogs(solutionId: string){
    this.solutionLogs = await this.exerciseService.checkSolutionLogs(solutionId);
    this.solutionLogs = this.solutionTimeConverter(this.solutionLogs);
    this.solutionLogs = this.solutionTimeSort(this.solutionLogs);  
  }
  solutionTimeConverter(array:any){
    array.forEach(element => {
      element.CheckedTime = new Date(element.CheckedTime);
     });
     return array;
  }
  solutionTimeSort(array:any){
    array.sort((a,b) => {
      const timeA=a.CheckedTime.getTime();
      const timeB=b.CheckedTime.getTime();
      if (timeA<timeB){
        return -1;
      }else {
        return 1;
      }
    });
    return array;
  }
  changeSolution(solutionId:string) {
     this.solutionData = this.allSolutions.find( element => element.Id === solutionId );
     if(LanguageConverter.normalFromWeb(this.solutionData['Language'])){
       this.solutionData['Language'] = LanguageConverter.normalFromWeb(this.solutionData['Language']);
     }
     this.getSolutionLogs(solutionId);
  }
  onNoClick(): void {
    this.dialogRef.close();
  }
}
