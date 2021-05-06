import { Component, OnInit } from '@angular/core';
import { Course } from 'src/app/Course';
import { Router } from '@angular/router';
import { ApprovalService } from 'src/app/approval.service';

export interface SubmitTable {
  courseName: string;
  courseNumber: string;
  exeName: string;
  teacherName: string;
}

export interface GradeTable {
  courseName: string;
  courseNumber: string;
  exeName: string;
  gradeNumber: number;
}

const SUBMITION_DATA: SubmitTable[] = [
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", teacherName: 'אסנת'},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", teacherName: 'פבל'},
];

const GRADE_DATA: GradeTable[] = [
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 70},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 60},
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 80},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 100},
  {courseName: "מבוא לרשתות תקשורת", courseNumber: '89-355', exeName: "ex 1", gradeNumber: 95},
  {courseName: "תכנות מתקדם 1", courseNumber: '89-357', exeName: "ex 3", gradeNumber: 76},
];

@Component({
  selector: 'app-student',
  templateUrl: './student.component.html',
  styleUrls: ['./student.component.css']
})
export class StudentComponent implements OnInit {
  exeStatus: string;
  selectedCourse: Course;
  coursesList: Course[];
  submitionColumns: any;
  submitionDataSource: SubmitTable[];
  gradeColumns: any;
  gradeDataSource: GradeTable[];

  constructor(
    private router: Router,
    private appService: ApprovalService,
  ) { 
    this.coursesList = [
    {
      name: "מבוא לרשתות תקשורת",
      courseId: "89-355",
    },
    {
      name: "תכנות מתקדם 1",
      courseId: "89-357",
    }];

    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
    this.submitionLoad();
    this.gradeLoad();
  }

  ngOnInit() {
    this.appService.updateExeStatus("");
  }

  onSelect(course: Course): void {
    this.selectedCourse = course;
  }

  submitionLoad() {
    this.submitionColumns = ['courseName', 'courseNumber', 'exeName', 'teacherName'];
    this.submitionDataSource = SUBMITION_DATA;
  }

  gradeLoad() {
    this.gradeColumns = ['courseName', 'courseNumber', 'exeName', 'gradeNumber'];
    this.gradeDataSource = GRADE_DATA;
  }

  getExe() {
    this.appService.updateExeStatus("before");
  }
}
