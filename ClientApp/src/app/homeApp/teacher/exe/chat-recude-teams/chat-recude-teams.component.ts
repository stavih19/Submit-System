import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';

@Component({
  selector: 'app-chat-recude-teams',
  templateUrl: './chat-recude-teams.component.html',
  styleUrls: ['./chat-recude-teams.component.css']
})
export class ChatRecudeTeamsComponent implements OnInit {
  selectedCourse: Course;
  checkoutForm = this.formBuilder.group({
    name: [''],
    date: ['']
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService,
  ) { }

  displayedColumns: string[] = [ "weight", "symbol"];

  ngOnInit() {

  }

  onSubmit() {
    let params = { 
      "name": this.checkoutForm.value.name,
      "reducePoints": this.checkoutForm.value.reducePoints,
    };

    
  }
}
