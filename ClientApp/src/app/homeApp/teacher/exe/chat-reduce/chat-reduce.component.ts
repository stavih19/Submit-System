import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { ReduceLine } from 'src/Modules/Reduce/reduce-line';
import { FormBuilder } from '@angular/forms';
import { TeacherDateDisplay } from 'src/Modules/Reduce/teacher-date-display';
import { SubmitDate } from 'src/Modules/Reduce/submit-date';
import { MatDatepickerInputEvent } from '@angular/material';

@Component({
  selector: 'app-chat-reduce',
  templateUrl: './chat-reduce.component.html',
  styleUrls: ['./chat-reduce.component.css']
})
export class ChatReduceComponent implements OnInit {
  selectedCourse: Course;
  selectExeId: string;
  date: Date;
  checkoutForm = this.formBuilder.group({
    date: [''],
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
    let params: SubmitDate = { } as SubmitDate;
    params.date = this.date;
    params.id = 0;
    params.exerciseID = this.selectExeId;
    params.group = 5;
    params.reduction = this.checkoutForm.value.reducePoints;
    
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
  }

  dateChanged(event: { value: Date; }) {
    this.date = event.value;
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
