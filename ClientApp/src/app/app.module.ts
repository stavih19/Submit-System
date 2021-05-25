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
import { HomeSubmitionExeComponent } from './homeApp/student/home-submition-exe/home-submition-exe.component';
import { AfterSubmitionExeComponent } from './homeApp/student/after-submition-exe/after-submition-exe.component';
import { MeanwhileSubmitionExeComponent } from './homeApp/student/meanwhile-submition-exe/meanwhile-submition-exe.component';
import { BeforeSubmitionExeComponent } from './homeApp/student/before-submition-exe/before-submition-exe.component';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { MatDialogModule } from '@angular/material/dialog';

import { StudentComponent } from './homeApp/student/student.component';
import { CheckerComponent } from './homeApp/checker/checker.component';
import { TeacherComponent } from './homeApp/teacher/teacher.component';
import { ChatDialogComponent } from './homeApp/student/before-submition-exe/chat-dialog/chat-dialog.component';

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
    TeacherComponent,
    HomeSubmitionExeComponent,
    AfterSubmitionExeComponent,
    MeanwhileSubmitionExeComponent,
    BeforeSubmitionExeComponent,
    ChatDialogComponent
  ],
  entryComponents: [
    ChatDialogComponent
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
    MatTableModule,
    DragDropModule,
    MatDialogModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
