import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Title } from '@angular/platform-browser';



@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {

  constructor(
    private titleService: Title,
  ) { }
  public get buildNumber(): string { return environment.buildNumber; }
  ngOnInit() {
    this.titleService.setTitle('О сайте');
  }

}
