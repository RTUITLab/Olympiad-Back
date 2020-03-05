import { Component, OnInit, ViewChild } from '@angular/core';
import { ChallengesService } from 'src/app/services/challenges.service';
import { ExerciseStateService } from 'src/app/services/exercise-state.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Challenge } from 'src/app/models/Responses/Challenges/Challenge';
import { DateHelpers } from 'src/app/Helpers/DateHelpers';
import { UserStateService } from 'src/app/services/user-state.service';
import { ChallengeHelpers } from 'src/app/Helpers/ChallengeHelpers';
import { Title } from '@angular/platform-browser';
import * as JSZip from 'jszip';
import { saveAs } from 'file-saver';
import { LanguageConverter } from 'src/app/models/Common/LanguageConverter';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ShowSolutionSourceCodeDialogComponent } from './show-solution-source-code-dialog/show-solution-source-code-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { ExerciseService } from 'src/app/services/exercise.service';
@Component({
  selector: 'app-challenge-info',
  templateUrl: './challenge-info.component.html',
  styleUrls: ['./challenge-info.component.scss']
})
export class ChallengeInfoComponent implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private usersService: UserStateService,
    private titleService: Title,
    private currentExerciseState: ExerciseStateService,
    private challengesService: ChallengesService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private exerciseService: ExerciseService
  ) { }
  public challenge?: Challenge;
  public dump: object;
  public exerciseNames: string[] = [];
  public statusMode = 'allPics';
  ngOnInit() {
    this.route.paramMap.subscribe(
      async params => {
        const id = params.get('ChallengeId');
        this.currentExerciseState.setChallengeId(id);
        this.challenge = await this.challengesService.getChallenge(id);
        this.currentExerciseState.setChallenge(this.challenge);
        this.titleService.setTitle(`${this.challenge.Name}`);
        const dump = await this.challengesService.getDump(id);
        for (const userId in dump) {
          if (dump.hasOwnProperty(userId)) {
            const userData = dump[userId];
            for (const exerciseName in userData) {
              if (userData.hasOwnProperty(exerciseName)) {
                const exercise = userData[exerciseName];
                if (this.exerciseNames.indexOf(exerciseName) === -1 && exerciseName !== 'summaryScore') {
                  this.exerciseNames.push(exerciseName);
                }

              }
            }
          }
        }
        this.exerciseNames.sort();
        this.dump = dump;
      }
    );
  }
  public challengeTime(challenge: Challenge): string {
    return ChallengeHelpers.ChallengeTime(challenge);
  }
  public prettyTime(time: string): string {
    return DateHelpers.prettyTime(time);
  }

  public editChallenge() {
    this.router.navigate(['edit-challenge', this.challenge.Id]);
  }

  public userIds(): string[] {
    return Object.keys(this.dump);
  }

  public solutionStatus(userId: string, exerciseName: string): number {
    if (this.dump[userId][exerciseName]) {
      return this.dump[userId][exerciseName].Status;
    }
    return -1;
  }

  public score(userId: string, exerciseName: string): number {
    if (this.dump[userId][exerciseName] && this.dump[userId][exerciseName].Status === 6) {
      return this.dump[userId][exerciseName].ExerciseScore;
    }
    return 0;
  }

  public downloadSolutions() {
    const zip = new JSZip();
    for (const userId in this.dump) {
      if (this.dump.hasOwnProperty(userId)) {
        const user = this.dump[userId];
        const userFolder = zip.folder(userId);
        for (const exerciseId in user) {
          if (user.hasOwnProperty(exerciseId)) {
            const exercise = user[exerciseId];
            if (typeof (exercise) === 'number') {
              continue;
            }
            userFolder.file(exercise.ExerciseName + LanguageConverter.fileExtensionByWebName(exercise.Language), exercise.Raw);
          }
        }
      }
    }
    zip.generateAsync({ type: 'blob' })
      .then(function (content) {
        saveAs(content, 'example.zip');
      });
  }

  showLogs(user: string, exercise: string) {
    const dialogRef = this.dialog.open(ShowSolutionSourceCodeDialogComponent, {
      width: '80vw', height: '90vh',
      data: { solutionData: this.dump[user][exercise] }
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  solutionId(user: string, exercise: string) {
    return this.dump[user][exercise].Id;
  }


  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }

  async deleteUser(studentId: string) {
    const result = await this.usersService.DeleteUser(studentId);
    this.toastr.show(`Удалено ${result} пользователей`);
  }

  async recheckSolutions(studentId: string) {
    const count = await this.exerciseService.recheckUserSolutions(studentId);

    this.toastr.success(`Будет перепроверено решений: ${count}`);
  }
}
