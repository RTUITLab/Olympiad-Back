import { Component, OnInit } from '@angular/core';
import { UsersService } from 'src/app/services/users.service';
import { UserInfo } from 'src/app/models/Responses/UserInfo';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {

  constructor(private usersService: UsersService,
    private router: Router,
    ) { }

  public users: Array<UserInfo> = [];

  ngOnInit() {
    this.usersService.getUsers().subscribe(u => {
      this.users = u;
    }, error => console.log(error));
  }
  userPage(userId: string) {
    console.log(userId);
    // this.router.navigate(['', userId ]);
  }
}
