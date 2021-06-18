import { HttpClient } from '@angular/common/http';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/Course';
import { GradeTable } from 'src/Modules/GradeTable';
import { SubmitTable } from 'src/Modules/SubmitTable';

@Component({
  selector: 'app-teacher',
  templateUrl: './teacher.component.html',
  styleUrls: ['./teacher.component.css']
})
export class TeacherComponent implements OnInit {
  exeStatus: string;
  theacherStatus: string;
  selectedCourse: Course;
  selectExe: any;
  teacherName: string;
  coursesList: Course[];
  submitionColumns: any;
  submitionDataSource: SubmitTable[];
  gradeColumns: any;
  gradeDataSource: GradeTable[];
  token: string;
  isToShowAlert: boolean;
  color: string;
  errorMessage: string;

  @ViewChild("alert", {static: false}) alert: ElementRef;

  constructor(
    private appService: ApprovalService,
    private httpClient: HttpClient,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.appService.theacherStatusStorage.subscribe(theacherStatus => this.theacherStatus = theacherStatus);
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
    this.appService.updateTheacherStatus("course");
  }

  getAppeals(row) {
    console.log("appeal");
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.appService.updateTheacherStatus("appeal");
  }

  getExtenstions(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    this.appService.updateTheacherStatus("extenstion");
  }

  getLastExe(row) {
    this.selectExe = row;
    this.teacherName = row.teacherName;
    //this.appService.updateTheacherStatus("extenstion");
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
}