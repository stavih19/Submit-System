import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './homeApp/login/login.component';
import { HomeComponentComponent } from './homeApp/home-component/home-component.component';
import { NavHomeComponent } from './homeApp/nav-home/nav-home.component';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { StudentComponent } from './homeApp/student/student.component';
import { CheckerComponent } from './homeApp/checker/checker.component';
import { TeacherComponent } from './homeApp/teacher/teacher.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    LoginComponent,
    HomeComponentComponent,
    NavHomeComponent,
    StudentComponent,
    CheckerComponent,
    TeacherComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    RouterModule.forRoot([
      { path: 'homeApp/student', component: StudentComponent, pathMatch: 'full' },
      { path: 'homeApp/checker', component: CheckerComponent },
      { path: 'homeApp/teacher', component: TeacherComponent },
    ]),
    FormsModule,
    BrowserAnimationsModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
