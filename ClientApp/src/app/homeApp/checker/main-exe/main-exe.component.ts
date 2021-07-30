import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-main-exe',
  templateUrl: './main-exe.component.html',
  styleUrls: ['./main-exe.component.css']
})
export class MainExeComponent implements OnInit {
  exeNameList: string[] = ["חגי", "נדב", "דוד"];
  alreadyChecked: string[] = ["שמחה", "יוסי"];
  reChecks: string[] = [];
  selectExe: any;
  exeStatus: string;
  token: string;

  @Input() selectedCourse: any;
  @ViewChild('teacherID', { static: false}) teacherID: ElementRef;
  @ViewChild('checkerID', { static: false}) checkerID: ElementRef;
  @Output() isToShowAlert = new EventEmitter<boolean>();
  @Output() color = new EventEmitter<string>();
  @Output() errorMessageText = new EventEmitter<string>();

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {

  }

  getExeDetails(exe: any) {
    this.selectExe = exe;
    this.appService.updateExeStatus("exeDetails");
  }  
}
