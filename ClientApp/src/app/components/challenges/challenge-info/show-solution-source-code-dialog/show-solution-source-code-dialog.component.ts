import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { LanguageConverter } from 'src/app/models/Common/LanguageConverter';
import { ExerciseService } from 'src/app/services/exercise.service';
import { SolutionLog } from 'src/app/models/SolutionLog';
import { CheckedSolution } from 'src/app/models/CheckedSolution';

@Component({
  selector: 'app-show-solution-source-code-dialog',
  templateUrl: './show-solution-source-code-dialog.component.html',
  styleUrls: ['./show-solution-source-code-dialog.component.scss']
})
export class ShowSolutionSourceCodeDialogComponent implements OnInit {
  solutionData: CheckedSolution;
  solutionLogs: SolutionLog[];
  constructor(
    public dialogRef: MatDialogRef<ShowSolutionSourceCodeDialogComponent>,
    public exerciseService: ExerciseService,
    @Inject(MAT_DIALOG_DATA) public data: object,
    ) {}
   async ngOnInit() {
      this.solutionData = this.data['solutionData'];
      this.solutionData['Language'] = LanguageConverter.normalFromWeb(this.data['solutionData'].Language);
      this.solutionLogs = await this.exerciseService.checkSolutionLogs(this.data['solutionData'].Id);
      this.solutionLogs.forEach(element => {
        element.CheckedTime = new Date(element.CheckedTime);
      });
      this.solutionLogs.sort((a,b) => {
        const timeA=a.CheckedTime.getTime();
        const timeB=b.CheckedTime.getTime();
        if (timeA<timeB){
          return -1;
        }else {
          return 1;
        }
      });      
    }
  onNoClick(): void {
    this.dialogRef.close();
  }
}
