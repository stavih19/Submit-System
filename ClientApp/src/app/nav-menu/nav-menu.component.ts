import { Component } from '@angular/core';
import { ApprovalService } from '../approval.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;

  constructor(
    private appService: ApprovalService,
  ) { }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
  
  logOut() {
    this.appService.updateLoginStatus(false);
    this.appService.updateUserName(["", ""]);
    this.appService.updateTheacherStatus("");
  }
}
