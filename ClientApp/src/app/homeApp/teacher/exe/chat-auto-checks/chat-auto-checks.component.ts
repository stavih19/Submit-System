import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApprovalService } from 'src/app/approval.service';
import { Course } from 'src/Modules/course';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ChatAdvancedChecksComponent } from './chat-advanced-checks/chat-advanced-checks.component';
import { AutoCheck } from 'src/Modules/AutoChecks/auto-check';

@Component({
  selector: 'app-chat-auto-checks',
  templateUrl: './chat-auto-checks.component.html',
  styleUrls: ['./chat-auto-checks.component.css']
})
export class ChatAutoChecksComponent implements OnInit {
  selectedCourse: Course;
  checkoutForm = this.formBuilder.group({
    name: [''],
    input: [''],
    output: [''],
    advanced: ['']
  });

  constructor(
    private httpClient: HttpClient,
    private formBuilder: FormBuilder,
    private appService: ApprovalService,
    public dialog: MatDialog
  ) { }

  lines: AutoCheck[] = [{
    name: "בדיקה 1",
    input: "קלט",
    output: "פלט",
    advencedConfiguration: {
      destination: "",
      timeout: 0,
      cmdArgs: [],
      fileName: "",
      extraFiles: []
    }
  }, {
    name: "בדיקה 2",
    input: "קלט",
    output: "פלט",
    advencedConfiguration: {
      destination: "",
      timeout: 0,
      cmdArgs: [],
      fileName: "",
      extraFiles: []
    }
  }
  ];

  displayedColumns: string[] = [ "weight", "symbol"];

  ngOnInit() {

  }

  onSubmit() {
    let params = { 
      "name": this.checkoutForm.value.name,
      "input": this.checkoutForm.value.input,
      "output": this.checkoutForm.value.output,
      "advanced": this.getAdvanced(this.checkoutForm.value.name)
    };
    this.lines.push(
      this.checkoutForm.value.name
    );
    this.lines = [...this.lines];
  }

  openAdvancedConfig(checkName: string) {
    const modalRef = this.dialog.open(ChatAdvancedChecksComponent);
    modalRef.componentInstance.selectedCourse = this.selectedCourse;
    //modalRef.componentInstance.checkName = checkName;
  }

  getAdvanced(checkName: string) {

  }
}
