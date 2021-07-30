import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { Course } from 'src/Modules/course';
import { Router } from '@angular/router';
import { ApprovalService } from 'src/app/approval.service';
import { ThrowStmt } from '@angular/compiler';
import { HttpClient } from '@angular/common/http';
import { SubmitTable } from 'src/Modules/submit-table';
import { GradeTable } from 'src/Modules/grade-table';
import { ExeToSubmit } from 'src/Modules/exe-to-submit';
import { ExeToCheck } from 'src/Modules/Checker/exe-to-check';
import { GradeTableChecker } from 'src/Modules/Checker/grade-table-checker';
import { ElementRef } from '@angular/core';

@Component({
  selector: 'app-checker',
  templateUrl: './checker.component.html',
  styleUrls: ['./checker.component.css']
})
export class CheckerComponent implements OnInit {
  exeStatus: string;
  selectedCourse: Course = {
    id: "",
    name: "",
    year: 0,
    number: 0
  }
  selectExe: any;
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

  submitionDataSource: ExeToCheck[] = [{
      courseID: "132564",  
      courseName: "תכנות מתקדם 1",
      courseNumber: 3652,
      date: new Date(),
      id: "1234569865312",
      name: "exe 1",
      exeAmount: 5,
    },
    {
      courseID: "2574325",  
      courseName: "תכנות מתקדם 1",
      courseNumber: 3652,
      date: new Date(),
      id: "54874545",
      name: "exe 2",
      exeAmount: 3,
    }
  ];

  gradeDataSource: GradeTableChecker[] = [
    {
      courseID: "fdsf",
      courseName: "תכנות מתקדם 1",
      courseNumber: "12354",
      exID: "1812",
      exName: "exe 2",
      exeAmount: 20
    },
    {
      courseID: "fdsf",
      courseName: "תכנות מתקדם 1",
      courseNumber: "12354",
      exID: "1812",
      exName: "exe 3",
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
    this.submitionLoad();
    this.gradeLoad();
  }

  getCourseList() {
    let url = 'https://localhost:5001/Student/CourseList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
      }, error => {
        console.log(error);
      }
    )
  }

  submitionLoad() {
    this.submitionColumns = ['courseName', 'courseNumber', 'exeName', 'exeAmount'];
    let url = 'https://localhost:5001/Student/ExerciseList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        //this.submitionDataSource = JSON.parse(data);
        console.log(data);
      }, error => {
        console.log(error);
      }
    )
  }

  gradeLoad() {
    this.gradeColumns = ['courseName', 'courseNumber', 'exeName', 'gradeAmount'];
    let url = 'https://localhost:5001/Student/GradesList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        //this.gradeDataSource = JSON.parse(data);
        console.log(data);
      }, error => {
        console.log(error);
      }
    )
  }

  onSelect(course: Course): void {
    this.selectedCourse = course;
    this.appService.updateExeStatus("exeMain");
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

  getExeDetails(row: any) {
    this.selectedCourse.name = row.courseName;
    this.selectExe = row;
    this.appService.updateExeStatus("exeDetails");
  }
}
