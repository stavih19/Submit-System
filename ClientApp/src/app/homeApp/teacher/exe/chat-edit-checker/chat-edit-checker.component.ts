import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/Course';
import { CheckerLabel } from 'src/Modules/Reduce/checker-label';

@Component({
  selector: 'app-chat-edit-checker',
  templateUrl: './chat-edit-checker.component.html',
  styleUrls: ['./chat-edit-checker.component.css']
})
export class ChatEditCheckerComponent implements OnInit {
  selectedCourse: Course;
  checkoutForm = this.formBuilder.group({
    id: [''],
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService,
  ) { }

  lines: CheckerLabel[] = [
    {
      name: "ישראל",
      id: 123456789
    },{
      name: "אהוד",
      id: 789456123
    },{
      name: "שני",
      id: 456789123
    }
  ];

  displayedColumns: string[] = ["weight"];

  ngOnInit() {

  }

  onSubmit() {
    let params = { 
      "name": "Name", 
      "id": parseInt(this.checkoutForm.value.id)
    };
    this.lines.push({
      name: "Name", 
      id: parseInt(this.checkoutForm.value.id)
    });
    this.lines = [...this.lines];
  }
}
