import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Course } from 'src/Modules/course';
import { Router } from '@angular/router';
import { ApprovalService } from 'src/app/approval.service';
import { ThrowStmt } from '@angular/compiler';
import { HttpClient } from '@angular/common/http';
import { SubmitTable } from 'src/Modules/Teacher/submit-table';
import { GradeTable } from 'src/Modules/grade-table';
import { ExeToSubmit } from 'src/Modules/exe-to-submit';
import { ExeToCheck } from 'src/Modules/Checker/exe-to-check';
import { GradeTableChecker } from 'src/Modules/Checker/grade-table-checker';
import { ElementRef } from '@angular/core';
import { CheckerExInfo } from 'src/Modules/Checker/checker-ex-info';

@Component({
  selector: 'app-checker',
  templateUrl: './checker.component.html',
  styleUrls: ['./checker.component.css']
})
export class CheckerComponent implements OnInit {
  exeStatus: string;
  exeToCheckList: CheckerExInfo[];
  selectedCourse: Course = {
    id: "",
    name: "",
    year: 0,
    number: 0
  }
  selectExe: CheckerExInfo;
  teacherName: string;
  coursesList: Course[];
  submitionColumns: any;
  //submitionDataSource: ExeToCheck[];
  gradeColumns: any;
  //gradeDataSource: GradeTableChecker[];
  token: string;
  isToShowAlert: boolean;
  color: string;
  errorMessage: string;

  @ViewChild("alert", {static: false}) alert: ElementRef;

  gradeDataSource: GradeTableChecker[] = [
    {
      courseID: "fdsf",
      courseName: "תכנות מתקדם 1",
      courseNumber: "12354",
      exID: "1812",
      name: "exe 2",
      exeAmount: 20
    },
    {
      courseID: "fdsf",
      courseName: "תכנות מתקדם 1",
      courseNumber: "12354",
      exID: "1812",
      name: "exe 3",
      exeAmount: 15
    }
  ];

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.appService.tokenStorage.subscribe(token => this.token = token);
  }

  ngOnInit() {
    this.appService.updateExeStatus("");

    this.getCourseList();
    this.exeToCheck();
    this.exeToReCheck();
  }

  getCourseList() {
    let url = 'https://localhost:5001/Student/CourseList';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
        console.log(this.coursesList);
      }, error => {
        console.log(error);
      }
    );
  }

  exeToCheck() {
    this.submitionColumns = ['courseName', 'courseNumber', 'exeName', 'exeAmount'];
    let url = 'https://localhost:5001/Checker/ExerciseList';
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.exeToCheckList = JSON.parse(data);
        console.log(this.exeToCheckList);
      }, error => {
        console.log(error);
      }
    )
  }

  exeToReCheck() {
    this.gradeColumns = ['courseName', 'courseNumber', 'exeName', 'gradeAmount'];
    let url = 'https://localhost:5001/Student/GradesList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.gradeDataSource = JSON.parse(data);
        console.log(this.gradeDataSource);
      }, error => {
        console.log(error);
      }
    )
  }

  onSelect(course: Course): void {
    this.selectedCourse = course;
    if(this.exeToCheckList.length > 0) {
      this.selectExe = this.exeToCheckList[0];
      this.appService.updateExeStatus("exeMain");
    }
  }

  onMark(course: Course): void {
    this.selectedCourse = course;
  }

  changeIsToShowAlert(isToShowAlert: boolean) {
    this.isToShowAlert = isToShowAlert;
  }

  changeColorAlert(color: string) {
    this.color = color;
    
    this.alert.nativeElement.classList.remove("alert-success");
    this.alert.nativeElement.classList.remove("alert-danger");
    this.alert.nativeElement.classList.remove("alert-warning");
    this.alert.nativeElement.classList.add(color);
  }

  changeMessageAlert(errorMessage: string) {
    console.log(errorMessage);
    this.errorMessage = errorMessage;
  }

  getExeDetails(row: CheckerExInfo) {
    this.selectedCourse.name = row.courseName;
    this.selectExe = row;
    console.log(row);
    this.appService.updateExeStatus("exeMain");
  }

  updateExe(exe: CheckerExInfo) {
    this.selectExe = exe;
  }
}