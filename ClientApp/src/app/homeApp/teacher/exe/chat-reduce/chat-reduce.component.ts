import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/Course';
import { ReduceLine } from 'src/Modules/Reduce/reduce-line';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-chat-reduce',
  templateUrl: './chat-reduce.component.html',
  styleUrls: ['./chat-reduce.component.css']
})
export class ChatReduceComponent implements OnInit {
  selectedCourse: Course;
  checkoutForm = this.formBuilder.group({
    date: [''],
    reducePoints: ['']
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService
  ) { }

  lines: ReduceLine[] = [
    {
      date: new Date,
      reducePoints: 10
    },{
      date: new Date,
      reducePoints: 10
    },{
      date: new Date,
      reducePoints: 10
    }
  ];

  displayedColumns: string[] = [ "weight", "symbol"];

  ngOnInit() {

  }

  onSubmit() {
    let params = { 
      "date": this.checkoutForm.value.date,
      "reducePoints": this.checkoutForm.value.reducePoints,
    };
    this.lines.push({
      date: this.checkoutForm.value.date, 
      reducePoints: parseInt(this.checkoutForm.value.reducePoints)
    });
    this.lines = [...this.lines];
  }
}
