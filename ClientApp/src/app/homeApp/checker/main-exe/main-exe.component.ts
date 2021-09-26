import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { ExeToCheck } from 'src/Modules/Checker/exe-to-check';
import { SubmissionLabel } from 'src/Modules/Checker/submission-label';
import { Course } from 'src/Modules/course';

@Component({
  selector: 'app-main-exe',
  templateUrl: './main-exe.component.html',
  styleUrls: ['./main-exe.component.css']
})
export class MainExeComponent implements OnInit {
  exeToCheck: Map<string, SubmissionLabel[]>;
  alreadyChecked: string[] = ["שמחה", "יוסי"];
  reChecks: string[] = [];
  exeStatus: string;
  token: string;

  @Input() selectedCourse: Course;
  @Input() selectExe: ExeToCheck;
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
    this.getExeToCheck();
  }

  getExeToCheck() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId' + this.selectExe.id;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeToCheck = JSON.parse(data);
        console.log(this.exeToCheck);
      }, error => {
        console.log(error);
      }
    );
  }

  getExeDetails(exe: any) {
    this.appService.updateExeStatus("exeDetails");
  }  

  autoCheck() {

  }

  styleCheck() {

  }
}
