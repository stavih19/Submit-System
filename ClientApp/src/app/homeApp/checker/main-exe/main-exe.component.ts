import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { MossData } from 'src/Modules/AutoChecks/moss-data';
import { CheckerExInfo } from 'src/Modules/Checker/checker-ex-info';
import { ExeToCheck } from 'src/Modules/Checker/exe-to-check';
import { SubmissionLabel } from 'src/Modules/Checker/submission-label';
import { Course } from 'src/Modules/course';
import { Exercise } from 'src/Modules/Teacher/exercise';
import { UserLabel } from 'src/Modules/Teacher/user-label';

@Component({
  selector: 'app-main-exe',
  templateUrl: './main-exe.component.html',
  styleUrls: ['./main-exe.component.css']
})
export class MainExeComponent implements OnInit {
  exeToCheck: SubmissionLabel[];
  alreadyChecked: SubmissionLabel[];
  reChecks: SubmissionLabel[];
  exeStatus: string;
  token: string;
  names: Map<string, string> = new Map();

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
    this.getExereChecked();
    this.getExereReChecked();
  }

  getExeToCheck() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeToCheck = JSON.parse(data)["Unchecked"];
        this.exeToCheck.forEach(element => {
          this.names.set(element.id, this.toStringNames(element.submitters));
        });
        console.log(this.exeToCheck);
      }, error => {
        console.log(error);
      }
    );
  }

  toStringNames(submitters: UserLabel[]) {
    let names: string = "";
    submitters.forEach(element => {
      if(names === "") {
        names += element.name;
      } else {
        names +=  "," + element.name;
      }
    });
    return names;
  }

  getExereChecked() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.alreadyChecked = JSON.parse(data)["Checked"];
        this.alreadyChecked.forEach(element => {
          this.names.set(element.id, this.toStringNames(element.submitters));
        });
        console.log(this.alreadyChecked);
      }, error => {
        console.log(error);
      }
    );
  }

  getExereReChecked() {
    let url = 'https://localhost:5001/Checker/SubmissionLabels?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.reChecks = JSON.parse(data)["Appeal"];
        this.reChecks.forEach(element => {
          this.names.set(element.id, this.toStringNames(element.submitters));
        });
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

  copyCheck() {
    let url = 'https://localhost:5001/Teacher/ExerciseDetails?exerciseId=' + this.selectExe.exID;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        let details: Exercise = JSON.parse(data);
        console.log(details);
        this.copyCheckSend(details)
      }, error => {
        console.log(error);
      }
    );
  }

  copyCheckSend(details: Exercise) {
    let data: MossData = {
      comment: "",
      exerciseID: this.selectExe.exID,
      result: "",
      maxFound: details.mossMaxTimesMatch,
      matchesShow: details.mossMaxTimesMatch
    };

    let url = 'https://localhost:5001/Teacher/CopyCheck';
    this.httpClient.post(url, data, 
    {responseType: 'text'}).toPromise().then(
      data => {
        window.open(data.toString(), '_blank');
      }, error => {
        console.log(error);
      }
    );
  }
}
