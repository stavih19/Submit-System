import { Component, OnInit } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-nav-home',
  templateUrl: './nav-home.component.html',
  styleUrls: ['./nav-home.component.css']
})
export class NavHomeComponent implements OnInit {
  userName: string;

  constructor(
    private appService: ApprovalService
  ) {}

  ngOnInit() {
    this.appService.userNameStorage.subscribe(userName => this.userName = userName);
  }
  
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
