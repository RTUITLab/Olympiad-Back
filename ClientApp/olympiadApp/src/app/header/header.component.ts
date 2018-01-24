import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor( private router: Router) { }
  login() {
    this.router.navigate(['login']);
  }
  register() {
    this.router.navigate(['register']);
  }
  ngOnInit() {
  }

}
