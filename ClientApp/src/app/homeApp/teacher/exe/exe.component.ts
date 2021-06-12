import { Component, OnInit } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-exe',
  templateUrl: './exe.component.html',
  styleUrls: ['./exe.component.css']
})
export class ExeComponent implements OnInit {

  constructor(
    private appService: ApprovalService,
  ) { }

  ngOnInit() {
  }
}
