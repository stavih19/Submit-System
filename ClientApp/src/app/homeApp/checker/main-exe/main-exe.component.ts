import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { CheckerExInfo } from 'src/Modules/Checker/checker-ex-info';
import { ExeToCheck } from 'src/Modules/Checker/exe-to-check';
import { SubmissionLabel } from 'src/Modules/Checker/submission-label';
import { Course } from 'src/Modules/course';

@Component({
  selector: 'app-main-exe',
  templateUrl: './main-exe.component.html',
  styleUrls: ['./main-exe.component.css']
})
export class MainExeComponent implements OnInit {
  exeToCheck: SubmissionLabel[];
  exeToCheckNames: string[][];
  alreadyChecked: string[] = ["שמחה", "יוסי"];
  reChecks: string[] = [];
  exeStatus: string;
  token: string;

  @Input() selectedCourse: Course;
  @Input() selectExe: CheckerExInfo;
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
    this.getExeCheckerd();
    this.getExereChecked();
  }

  getExeToCheck() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeToCheckNames = [];
        this.exeToCheck = JSON.parse(data)["Unchecked"];
        this.exeToCheck.forEach(exe => {
          //this.exeToCheckNames.push(exe.submitters)
        });
        console.log(this.exeToCheck);
      }, error => {
        console.log(error);
      }
    );
  }

  getExeCheckerd() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.alreadyChecked = JSON.parse(data)["Checked"];
        console.log(this.alreadyChecked);
      }, error => {
        console.log(error);
      }
    );
  }

  getExereChecked() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.reChecks = JSON.parse(data)["Appeal"];
        console.log(this.reChecks);
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
