import { Component, OnInit, ViewChild } from '@angular/core';
import { Course } from 'src/Modules/course';
import { Router } from '@angular/router';
import { ApprovalService } from 'src/app/approval.service';
import { ThrowStmt } from '@angular/compiler';
import { HttpClient } from '@angular/common/http';
import { SubmitTable } from 'src/Modules/submit-table';
import { GradeTable } from 'src/Modules/grade-table';
import { HomeSubmitionExeComponent } from './home-submition-exe/home-submition-exe.component';

@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  styleUrls: ['./student.component.css']
})
export class StudentComponent implements OnInit {
  exeStatus: string;
  selectedCourse: Course;
  selectExe: any;
  teacherName: string;
  coursesList: Course[];
  submitionColumns: any;
  submitionDataSource: SubmitTable[];
  gradeColumns: any;
  gradeDataSource: GradeTable[];
  token: string;

  @ViewChild(HomeSubmitionExeComponent, {static: false}) child: HomeSubmitionExeComponent;

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

  onSelect(course: Course): void {
    this.selectedCourse = course;
  }

  SelectFirstExe(course: Course) {
    //this.getBeforeEXE(course)
  }

  getBeforeEXE(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.appService.updateExeStatus("before");
    //this.child.onSelect(row);
  }

  getAfterEXE(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    //this.child.onSelect(row);
    this.appService.updateExeStatus("before");
  }

  getExeInfo() { }

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
    this.submitionColumns = ['courseName', 'courseNumber', 'exeName', 'teacherName'];
    let url = 'https://localhost:5001/Student/ExerciseList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.submitionDataSource = JSON.parse(data);
      }, error => {
        console.log(error);
      }
    )
  }

  gradeLoad() {
    this.gradeColumns = ['courseName', 'courseNumber', 'exeName', 'gradeNumber'];
    let url = 'https://localhost:5001/Student/GradesList?token=' + this.token;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.gradeDataSource = JSON.parse(data);
      }, error => {
        console.log(error);
      }
    )
  }
};