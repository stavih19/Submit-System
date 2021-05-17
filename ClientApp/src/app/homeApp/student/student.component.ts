import { Component, OnInit } from '@angular/core';
import { Course } from 'src/Modules/Course';
import { Router } from '@angular/router';
import { ApprovalService } from 'src/app/approval.service';
import { ThrowStmt } from '@angular/compiler';
import { HttpClient } from '@angular/common/http';
import { SubmitTable } from 'src/Modules/SubmitTable';
import { GradeTable } from 'src/Modules/GradeTable';

@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  styleUrls: ['./student.component.css']
})
export class StudentComponent implements OnInit {
  exeStatus: string;
  selectedCourse: Course;
  selectExe: any;
  coursesList: Course[];
  submitionColumns: any;
  submitionDataSource: SubmitTable[];
  gradeColumns: any;
  gradeDataSource: GradeTable[];
  token: string;


  constructor(
    private router: Router,
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);

    this.getCourseList();
    this.submitionLoad();
    this.gradeLoad();
  }

  ngOnInit() {
    this.appService.updateExeStatus("");
    this.appService.userNameStorage.subscribe(token => this.token = token);
  }

  onSelect(course: Course): void {
    this.selectedCourse = course;
  }

  getBeforeEXE(row) {
    this.selectExe = row;
    this.appService.updateExeStatus("before");
  }

  getAfterEXE(row) {
    this.selectExe = row;
    this.appService.updateExeStatus("after");
  }

  getExeInfo() { }

  submitionLoad() {
    this.submitionColumns = ['courseName', 'courseNumber', 'exeName', 'teacherName'];
    let url = 'https://localhost:5001/Student/SubmissionList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
      }, error => {
        console.log(error);

        this.submitionDataSource = [
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
        ];
      }
    )
  }

  getCourseList() {
    let url = 'https://localhost:5001/Student/CourseList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
      }, error => {
        console.log(error);

        this.coursesList = [
          {
            ID: "89-355",
            name: "מבוא לרשתות תקשורת",
            year: 2021,
            number: 2
          },
          {
            ID: "89-357",
            name: "תכנות מתקדם 1",
            year: 2021,
            number: 2
          }];
      }
    )
  }

  gradeLoad() {
    this.gradeColumns = ['courseName', 'courseNumber', 'exeName', 'gradeNumber'];
    let url = 'https://localhost:5001/Student/SubmissionList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.coursesList = JSON.parse(data);
      }, error => {
        console.log(error);

        this.gradeDataSource = [
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 70},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 60},
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 80},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 100},
          {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 95},
          {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 76},
        ];
      }
    )
  }
}