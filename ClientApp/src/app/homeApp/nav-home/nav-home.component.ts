import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-nav-home',
  templateUrl: './nav-home.component.html',
  styleUrls: ['./nav-home.component.css']
})
export class NavHomeComponent implements OnInit, AfterViewInit {
  userName: string[];

  @ViewChild("tabs", {static: false}) tabs: ElementRef;

  constructor(
    private appService: ApprovalService
  ) {}

  ngOnInit() {
    this.appService.userNameStorage.subscribe(userName => this.userName = userName);
  }

  ngAfterViewInit(): void {
    this.tabs.nativeElement.firstChild.firstChild.click();
  }
  
  isExpanded = false;

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  onClick(id) {
    document.getElementById('student').style.backgroundColor = "";
    document.getElementById('checker').style.backgroundColor = "";
    document.getElementById('teacher').style.backgroundColor = "";

    document.getElementById(id).style.backgroundColor = "#eee";
    this.appService.updateExeStatus("");
  }
}
