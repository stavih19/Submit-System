import { Component, Input, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { ReduceLine } from 'src/Modules/Reduce/reduce-line';
import { FormBuilder } from '@angular/forms';
import { TeacherDateDisplay } from 'src/Modules/Reduce/teacher-date-display';
import { SubmitDate } from 'src/Modules/Reduce/submit-date';
import { MatDatepickerInputEvent } from '@angular/material';
import { Exercise } from 'src/Modules/Teacher/exercise';
import { NewExe } from 'src/Modules/Teacher/new-exe';

@Component({
  selector: 'app-chat-reduce',
  templateUrl: './chat-reduce.component.html',
  styleUrls: ['./chat-reduce.component.css']
})
export class ChatReduceComponent implements OnInit {
  selectedCourse: Course;
  selectExeId: string;
  date: Date;
  exeObject: NewExe;
  checkoutForm = this.formBuilder.group({
    reducePoints: ['']
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService
  ) { }

  lines: TeacherDateDisplay[];

  displayedColumns: string[] = ["delete", "weight", "symbol"];

  ngOnInit() {
    this.getReduceTeams();
  }

  getReduceTeams() {
    let url = 'https://localhost:5001/Teacher/GetDates?exerciseId=' + this.selectExeId;
    this.httpClient.get(url, 
    {responseType: 'text'}).toPromise().then(
      data => {
        this.lines = JSON.parse(data);
        console.log(this.lines);
      }, error => {
        console.log(error);
      }
    );
  }

  onSubmit() {
    let reduceList: string[] = this.checkoutForm.value.reducePoints.split(',');
    for(let i = 0; i < reduceList.length; i++) {
      if(reduceList[i][0] === ' ') {
        reduceList[i] = reduceList[i].slice(1, reduceList[i].length);
      }
      if(reduceList[i][reduceList[i].length - 1] === ' ') {
        reduceList[i] = reduceList[i].slice(0, reduceList[i].length - 1);
      }
    }

    let jump = 1;
    const sleep = (delay: number) => new Promise((resolve) => setTimeout(resolve, delay));
    reduceList.forEach(async reduceNumber => {
      let params: SubmitDate = { } as SubmitDate;
      params.date = new Date();
      params.date.setDate(this.exeObject.MainDate.getDate() + jump);
      params.id = 0;
      params.exerciseID = this.selectExeId;
      params.group = 5;
      params.reduction = parseInt(reduceNumber);
      
      let url = 'https://localhost:5001/Teacher/AddDate?exerciseId=' + this.selectExeId;
      this.httpClient.post(url, params,
      {responseType: 'text'}).toPromise().then(
        data => {
          console.log(data.toString());
          this.getReduceTeams();
        }, error => {
          console.log(error);
        }
      );
      await sleep(100);
      jump++;
    });
    this.checkoutForm.reset();
  }

  deleteReduction(id: number) {
    let url = 'https://localhost:5001/Teacher/DeleteDate?dateId=' + id;
    this.httpClient.delete(url,
    {responseType: 'text'}).toPromise().then(
      data => {
        console.log(data.toString());
        this.getReduceTeams();
      }, error => {
        console.log(error);
      }
    );
  }
}
