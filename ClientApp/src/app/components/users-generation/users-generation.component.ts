import { Component, OnInit } from '@angular/core';
import { UserStateService } from 'src/app/services/user-state.service';
import { User } from 'src/app/models/User';
import { ToastrService } from 'ngx-toastr';
import { UsersGenerateService } from '../../services/users-generate.service';
import * as XLSX from 'xlsx';
import { LoadingComponent } from '../helpers/loading-component';
import { FormControl, FormGroup } from '@angular/forms';
import { Title } from '@angular/platform-browser';




@Component({
  selector: 'app-users-generation',
  templateUrl: './users-generation.component.html',
  styleUrls: ['./users-generation.component.scss']
})
export class UsersGenerationComponent extends LoadingComponent implements OnInit {
  constructor(
    private usersService: UserStateService,
    private toastr: ToastrService,
    private userGenService: UsersGenerateService,
    private titleService: Title,


  ) {
    super();
   }
  usersGenForm = new FormGroup({
    usersIdFormContol: new FormControl()
  });
  allNewUsers?: Array<User> = [];
  usersId?: Array<string> = [];
  ngOnInit() {
    this.titleService.setTitle('Генерация пользователей');

  }
  genUsers() {
    this.usersId = this.usersGenForm.value['usersIdFormContol'].split('\n');
    this.userGenService.generateUsers(this.usersId).subscribe(
      data => {
        this.allNewUsers = data;
        this.toastr.success(`Пользователи успешно сгенерированы`);
        this.downloadUsers();
      },
      err => {
        this.toastr.error(`Пользователи не сгенерированы`);
      }
    );
  }
  downloadUsers() {
    /* generate worksheet */
    const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(this.allNewUsers);
    /* generate workbook and add the worksheet */
    const wb: XLSX.WorkBook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet 1');
    /* save to file */
    XLSX.writeFile(wb, `Generated-users.xlsx`);
  }
  isAdmin(): boolean {
    return this.usersService.IsAdmin();
  }
}
