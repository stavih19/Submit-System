import { Component, OnInit } from '@angular/core';
import { ApprovalService } from 'src/app/approval.service';

@Component({
  selector: 'app-home-submition-exe',
  templateUrl: './home-submition-exe.component.html',
  styleUrls: ['./home-submition-exe.component.css']
})
export class HomeSubmitionExeComponent implements OnInit {
  exeStatus: string;

  constructor(
    private appService: ApprovalService,
  ) { 
    this.appService.exeStatusStorage.subscribe(exeStat => this.exeStatus = exeStat);
  }

  ngOnInit() {
  }
}
