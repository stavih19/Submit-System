import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ApprovalService } from 'src/app/approval.service';
import { LoginComponent } from '../login/login.component';

@Component({
  selector: 'app-home-component',
  templateUrl: './home-component.component.html',
  styleUrls: ['./home-component.component.css']
})
export class HomeComponentComponent implements OnInit {
  logedIn: boolean;

  constructor(
    private appService: ApprovalService,
  ) {}

  ngOnInit() {
    this.appService.logedInStorage.subscribe(status => this.logedIn = status);
  }
}
