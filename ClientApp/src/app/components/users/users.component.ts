import { Component, OnInit } from '@angular/core';
import { UsersService } from 'src/app/services/users.service';
import { UserInfo } from 'src/app/models/Responses/UserInfo';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  constructor(private usersService: UsersService) { }

  public users: Array<UserInfo> = [];

  ngOnInit() {
    this.usersService.getUsers().subscribe(u => {
      this.users = u;
    }, error => console.log(error));
  }
}
