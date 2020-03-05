import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { LanguageConverter } from 'src/app/models/Common/LanguageConverter';
import { ExerciseService } from 'src/app/services/exercise.service';
import { SolutionLog } from 'src/app/models/SolutionLog';
import { CheckedSolution } from 'src/app/models/CheckedSolution';
import { ToastrService } from 'ngx-toastr';

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
    private toastr: ToastrService,
    @Inject(MAT_DIALOG_DATA) public data: CheckedSolution,
  ) { }
  ngOnInit() {
    this.getBestSolution();
    this.getAllSolutions();
  }
  async getAllSolutions() {
    this.allSolutions = await this.exerciseService.getUserExerciseSolutions(this.solutionData.ExerciseId, this.solutionData.UserId);
    this.allSolutions = this.solutionTimeConverter(this.allSolutions, 'SendingTime');
    this.allSolutions = this.solutionTimeSort(this.allSolutions, 'SendingTime');
  }
  getBestSolution() {
    this.solutionData = this.data['solutionData'];
    this.solutionData['Language'] = LanguageConverter.normalFromWeb(this.data['solutionData'].Language);
    this.getSolutionLogs(this.data['solutionData'].Id);
  }
  async getSolutionLogs(solutionId: string) {
    this.solutionLogs = await this.exerciseService.checkSolutionLogs(solutionId);
    this.solutionLogs = this.solutionTimeConverter(this.solutionLogs, 'CheckedTime');
    this.solutionLogs = this.solutionTimeSort(this.solutionLogs, 'CheckedTime');
  }
  solutionTimeConverter(array: any, key: string) {
    array.forEach(el => {
      el[key] = new Date(el[key]);
    });
    return array;
  }
  solutionTimeSort(array: any, key: string) {
    array.sort((firElem, secElem) => {
      const timeFirElem = firElem[key].getTime();
      const timeSecElem = secElem[key].getTime();
      if (timeFirElem < timeSecElem) {
        return -1;
      } else {
        return 1;
      }
    });
    return array;
  }
  changeSolution(solutionId: string) {
    this.solutionData = this.allSolutions.find(el => el.Id === solutionId);
    if (LanguageConverter.normalFromWeb(this.solutionData['Language'])) {
      this.solutionData['Language'] = LanguageConverter.normalFromWeb(this.solutionData['Language']);
    }
    this.getSolutionLogs(solutionId);
  }
  onNoClick(): void {
    this.dialogRef.close();
  }

  async recheck(id: string) {
    console.log(id);
    const count = await this.exerciseService.recheckSolution(id);

    this.toastr.success(`Будет перепроверено решений: ${count}`);
  }

  runResult(log: SolutionLog): string {
    if (log.ProgramErr) {
      return "Есть поток ошибок";
    }
    if (log.ExampleOut === log.ProgramOut) {
      return "Корректно";
    }
    return "Не совппадают вход и выход";

  }
}
